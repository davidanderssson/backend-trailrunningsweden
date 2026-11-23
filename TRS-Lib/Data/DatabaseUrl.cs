using Npgsql;

namespace TRS_Lib.Data;

public static class DatabaseUrl
{
    // Converts a URL like postgresql://user:password@host:5432/database (Railway's DATABASE_URL)
    // into the key=value connection string Npgsql expects
    public static string ToConnectionString(string databaseUrl)
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':', 2);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.IsDefaultPort || uri.Port < 0 ? 5432 : uri.Port,
            Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/')),
            Username = Uri.UnescapeDataString(userInfo[0]),
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : null
        };

        var sslMode = System.Web.HttpUtility.ParseQueryString(uri.Query)["sslmode"];
        if (sslMode is not null)
        {
            builder.SslMode = Enum.Parse<SslMode>(sslMode.Replace("-", ""), ignoreCase: true);
        }

        return builder.ConnectionString;
    }
}
