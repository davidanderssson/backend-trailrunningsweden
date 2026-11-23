using System.Security.Cryptography;
using TRS_Lib.Models;
using TRS_Lib.Repositories;

namespace TRS_Lib.Services;

public class OneTimePasswordService(IOneTimePasswordRepository repository)
{
    public static readonly TimeSpan TimeToLive = TimeSpan.FromMinutes(10);
    public const int MaxAttempts = 5;

    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    // Creates a new 6 digit code for the user and returns it in plain text. Only the hash is stored.
    public async Task<(string Code, DateTimeOffset ExpiresAt)> CreateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        var now = DateTimeOffset.UtcNow;

        var oneTimePassword = new OneTimePassword
        {
            UserId = userId,
            CodeHash = Hash(code),
            CreatedAt = now,
            ExpiresAt = now + TimeToLive
        };

        await repository.ReplaceForUserAsync(oneTimePassword, cancellationToken);
        return (code, oneTimePassword.ExpiresAt);
    }

    // Returns true if the code matches the user's active code. A code can be used once and tried MaxAttempts times.
    public async Task<bool> VerifyAsync(Guid userId, string code, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var oneTimePassword = await repository.FindActiveForUserAsync(userId, now, cancellationToken);
        if (oneTimePassword is null)
        {
            return false;
        }

        // Count the attempt before checking the code, so parallel requests can't get extra guesses
        if (!await repository.TryRegisterAttemptAsync(oneTimePassword.Id, MaxAttempts, cancellationToken))
        {
            return false;
        }

        if (!Verify(code, oneTimePassword.CodeHash))
        {
            return false;
        }

        return await repository.TryMarkUsedAsync(oneTimePassword.Id, now, cancellationToken);
    }

    // Format: {salt}.{hash}, both base64
    public static string Hash(string code)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(code, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string code, string codeHash)
    {
        var parts = codeHash.Split('.');
        if (parts.Length != 2)
        {
            return false;
        }

        var salt = Convert.FromBase64String(parts[0]);
        var expected = Convert.FromBase64String(parts[1]);
        var actual = Rfc2898DeriveBytes.Pbkdf2(code, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
