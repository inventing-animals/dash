using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Dash.Server.Persistence;

public enum DatabaseProvider
{
    Sqlite,
    PostgreSql,
}

public sealed record DatabaseSettings(
    DatabaseProvider Provider,
    string ConnectionString);

public static class DatabaseSettingsResolver
{
    public static DatabaseSettings Resolve(IConfiguration configuration, IHostEnvironment environment)
    {
        var providerName = configuration["Database:Provider"] ?? nameof(DatabaseProvider.Sqlite);
        if (!Enum.TryParse<DatabaseProvider>(providerName, ignoreCase: true, out var provider))
        {
            throw new InvalidOperationException(
                $"Unsupported database provider '{providerName}'. Supported values are: {string.Join(", ", Enum.GetNames<DatabaseProvider>())}.");
        }

        var rawConnectionString =
            configuration.GetConnectionString(DatabaseDefaults.ConnectionStringName)
            ?? GetDefaultConnectionString(provider);

        var connectionString = provider switch
        {
            DatabaseProvider.Sqlite => NormalizeSqliteConnectionString(rawConnectionString, environment),
            DatabaseProvider.PostgreSql => rawConnectionString,
            _ => throw new InvalidOperationException($"Unsupported database provider '{provider}'."),
        };

        return new DatabaseSettings(provider, connectionString);
    }

    private static string GetDefaultConnectionString(DatabaseProvider provider)
    {
        return provider switch
        {
            DatabaseProvider.Sqlite => "Data Source=data/dash.db",
            DatabaseProvider.PostgreSql => string.Empty,
            _ => string.Empty,
        };
    }

    private static string NormalizeSqliteConnectionString(string rawConnectionString, IHostEnvironment environment)
    {
        var builder = new SqliteConnectionStringBuilder(rawConnectionString);

        if (string.IsNullOrWhiteSpace(builder.DataSource) ||
            string.Equals(builder.DataSource, ":memory:", StringComparison.OrdinalIgnoreCase))
        {
            return builder.ToString();
        }

        if (!Path.IsPathRooted(builder.DataSource))
        {
            builder.DataSource = Path.GetFullPath(builder.DataSource, environment.ContentRootPath);
        }

        var directory = Path.GetDirectoryName(builder.DataSource);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return builder.ToString();
    }
}
