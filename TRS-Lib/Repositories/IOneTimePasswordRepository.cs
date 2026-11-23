using TRS_Lib.Models;

namespace TRS_Lib.Repositories;

public interface IOneTimePasswordRepository
{
    // Replaces any codes the user already has, so only the newest one is valid
    Task ReplaceForUserAsync(OneTimePassword oneTimePassword, CancellationToken cancellationToken = default);

    // Returns the user's newest code that is unused and not expired, or null
    Task<OneTimePassword?> FindActiveForUserAsync(Guid userId, DateTimeOffset now, CancellationToken cancellationToken = default);

    // Atomically counts one attempt. Returns false if the code already has maxAttempts attempts.
    Task<bool> TryRegisterAttemptAsync(Guid id, int maxAttempts, CancellationToken cancellationToken = default);

    // Atomically marks the code as used. Returns false if it was already used.
    Task<bool> TryMarkUsedAsync(Guid id, DateTimeOffset now, CancellationToken cancellationToken = default);
}
