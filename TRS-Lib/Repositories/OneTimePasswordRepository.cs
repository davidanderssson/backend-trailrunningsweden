using Microsoft.EntityFrameworkCore;
using TRS_Lib.Data;
using TRS_Lib.Models;

namespace TRS_Lib.Repositories;

public class OneTimePasswordRepository(TrsDbContext db) : IOneTimePasswordRepository
{
    public async Task ReplaceForUserAsync(OneTimePassword oneTimePassword, CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        await db.OneTimePasswords
            .Where(o => o.UserId == oneTimePassword.UserId)
            .ExecuteDeleteAsync(cancellationToken);

        db.OneTimePasswords.Add(oneTimePassword);
        await db.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    public Task<OneTimePassword?> FindActiveForUserAsync(int userId, DateTimeOffset now, CancellationToken cancellationToken = default)
    {
        return db.OneTimePasswords
            .AsNoTracking()
            .Where(o => o.UserId == userId && o.UsedAt == null && o.ExpiresAt > now)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> TryRegisterAttemptAsync(int id, int maxAttempts, CancellationToken cancellationToken = default)
    {
        var updated = await db.OneTimePasswords
            .Where(o => o.Id == id && o.Attempts < maxAttempts)
            .ExecuteUpdateAsync(s => s.SetProperty(o => o.Attempts, o => o.Attempts + 1), cancellationToken);
        return updated > 0;
    }

    public async Task<bool> TryMarkUsedAsync(int id, DateTimeOffset now, CancellationToken cancellationToken = default)
    {
        var updated = await db.OneTimePasswords
            .Where(o => o.Id == id && o.UsedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(o => o.UsedAt, now), cancellationToken);
        return updated > 0;
    }
}
