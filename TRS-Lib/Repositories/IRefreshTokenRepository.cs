using TRS_Lib.Models;

namespace TRS_Lib.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task<RefreshToken?> FindByHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    // Atomically marks the token as used. Returns false if it was already used or revoked.
    Task<bool> TryMarkUsedAsync(int id, DateTimeOffset now, CancellationToken cancellationToken = default);

    Task RevokeAsync(int id, DateTimeOffset now, CancellationToken cancellationToken = default);
    Task RevokeAllForUserAsync(int userId, DateTimeOffset now, CancellationToken cancellationToken = default);
}
