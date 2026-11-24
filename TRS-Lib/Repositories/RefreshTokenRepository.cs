using Microsoft.EntityFrameworkCore;
using TRS_Lib.Data;
using TRS_Lib.Models;

namespace TRS_Lib.Repositories;

public class RefreshTokenRepository(TrsDbContext db) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        if (refreshToken.Id == Guid.Empty)
        {
            refreshToken.Id = Guid.CreateVersion7();
        }

        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public Task<RefreshToken?> FindByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return db.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(r => r.TokenHash == tokenHash, cancellationToken);
    }

    public async Task<bool> TryMarkUsedAsync(Guid id, DateTimeOffset now, CancellationToken cancellationToken = default)
    {
        var updated = await db.RefreshTokens
            .Where(r => r.Id == id && r.UsedAt == null && r.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.UsedAt, now), cancellationToken);
        return updated > 0;
    }

    public Task RevokeAsync(Guid id, DateTimeOffset now, CancellationToken cancellationToken = default)
    {
        return db.RefreshTokens
            .Where(r => r.Id == id && r.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.RevokedAt, now), cancellationToken);
    }

    public Task RevokeAllForUserAsync(Guid userId, DateTimeOffset now, CancellationToken cancellationToken = default)
    {
        return db.RefreshTokens
            .Where(r => r.UserId == userId && r.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.RevokedAt, now), cancellationToken);
    }
}
