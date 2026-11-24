using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TRS_Lib.Dtos;
using TRS_Lib.Models;
using TRS_Lib.Repositories;

namespace TRS_Lib.Services;

public class TokenService(JwtSettings settings, IRefreshTokenRepository refreshTokens, IUserRepository users)
{
    private static readonly JsonWebTokenHandler Handler = new();

    // Issues a new access token and refresh token for the user
    public async Task<TokenResponseDto> IssueAsync(User user, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var accessTokenExpiresAt = now + JwtSettings.AccessTokenLifetime;

        var accessToken = Handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = settings.Issuer,
            Audience = settings.Audience,
            IssuedAt = now.UtcDateTime,
            NotBefore = now.UtcDateTime,
            Expires = accessTokenExpiresAt.UtcDateTime,
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ]),
            SigningCredentials = new SigningCredentials(settings.SigningKey, SecurityAlgorithms.HmacSha256)
        });

        // The refresh token is 256 random bits, so a plain SHA-256 hash is enough to store it safely
        var refreshToken = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));
        var refreshTokenExpiresAt = now + JwtSettings.RefreshTokenLifetime;

        await refreshTokens.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = Hash(refreshToken),
            CreatedAt = now,
            ExpiresAt = refreshTokenExpiresAt
        }, cancellationToken);

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshTokenExpiresAt
        };
    }

    // Exchanges a refresh token for a new token pair. Each refresh token works once; if a used one is
    // presented again it has probably been stolen, so all of the user's refresh tokens are revoked.
    public async Task<TokenResponseDto?> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var stored = await refreshTokens.FindByHashAsync(Hash(refreshToken), cancellationToken);
        if (stored is null || stored.RevokedAt is not null || stored.ExpiresAt <= now)
        {
            return null;
        }

        if (stored.UsedAt is not null)
        {
            await refreshTokens.RevokeAllForUserAsync(stored.UserId, now, cancellationToken);
            return null;
        }

        if (!await refreshTokens.TryMarkUsedAsync(stored.Id, now, cancellationToken))
        {
            return null;
        }

        var user = await users.FindAsync(stored.UserId, cancellationToken);
        return user is null ? null : await IssueAsync(user, cancellationToken);
    }

    // Revokes the refresh token, if it exists. Does nothing for unknown tokens.
    public async Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var stored = await refreshTokens.FindByHashAsync(Hash(refreshToken), cancellationToken);
        if (stored is not null)
        {
            await refreshTokens.RevokeAsync(stored.Id, DateTimeOffset.UtcNow, cancellationToken);
        }
    }

    private static string Hash(string refreshToken)
    {
        return Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(refreshToken)));
    }
}
