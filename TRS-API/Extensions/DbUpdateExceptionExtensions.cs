using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace TRS_API.Extensions;

public static class DbUpdateExceptionExtensions
{
    // True when the save failed because a unique index (e.g. a duplicate name or email) was violated
    public static bool IsUniqueViolation(this DbUpdateException exception)
    {
        return exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
    }
}
