using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace TRS_Lib.Services;

public class JwtSettings
{
    public static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(60);
    public static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(30);

    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required SymmetricSecurityKey SigningKey { get; init; }

    // Reads the Jwt section. Throws at startup if something is missing, rather than issuing unusable tokens.
    public static JwtSettings FromConfiguration(IConfiguration configuration)
    {
        var signingKey = configuration["Jwt:SigningKey"];
        if (string.IsNullOrWhiteSpace(signingKey) || Encoding.UTF8.GetByteCount(signingKey) < 32)
        {
            throw new InvalidOperationException("The Jwt:SigningKey setting must be at least 32 characters.");
        }

        return new JwtSettings
        {
            Issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("The Jwt:Issuer setting is not configured."),
            Audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("The Jwt:Audience setting is not configured."),
            SigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
        };
    }
}
