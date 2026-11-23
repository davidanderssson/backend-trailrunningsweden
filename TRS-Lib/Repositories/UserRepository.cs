using Microsoft.EntityFrameworkCore;
using TRS_Lib.Data;
using TRS_Lib.Models;

namespace TRS_Lib.Repositories;

public class UserRepository(TrsDbContext db) : IUserRepository
{
    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        if (user.Id == Guid.Empty)
        {
            user.Id = Guid.CreateVersion7();
        }

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);
        return user;
    }

    public Task<User?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    // Returns false if no user with that id exists
    public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var existing = await db.Users.FindAsync([user.Id], cancellationToken);
        if (existing is null)
        {
            return false;
        }

        existing.FirstName = user.FirstName;
        existing.LastName = user.LastName;
        existing.Email = user.Email;

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    // Returns false if no user with that id exists
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await db.Users.Where(u => u.Id == id).ExecuteDeleteAsync(cancellationToken);
        return deleted > 0;
    }
}
