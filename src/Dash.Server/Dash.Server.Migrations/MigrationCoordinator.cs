using Dash.Server.Persistence;
using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dash.Server.Migrations;

public sealed class MigrationCoordinator(
    IServiceProvider serviceProvider,
    DatabaseSettings databaseSettings,
    IHostEnvironment hostEnvironment,
    ILogger<MigrationCoordinator> logger)
    : IMigrationCoordinator
{
    public async Task<MigrationStatusSnapshot> GetStatusAsync(CancellationToken cancellationToken)
    {
        switch (databaseSettings.Provider)
        {
            case DatabaseProvider.Sqlite:
                return await GetSqliteStatusAsync(cancellationToken);
            case DatabaseProvider.PostgreSql:
                throw new NotSupportedException("Portable migrations are ready, but PostgreSQL migration status is not implemented yet.");
            default:
                throw new InvalidOperationException($"Unsupported database provider '{databaseSettings.Provider}'.");
        }
    }

    public async Task<MigrationStatusSnapshot> MigrateUpAsync(TimeSpan lockTimeout, CancellationToken cancellationToken)
    {
        switch (databaseSettings.Provider)
        {
            case DatabaseProvider.Sqlite:
                return await MigrateSqliteUpAsync(lockTimeout, cancellationToken);
            case DatabaseProvider.PostgreSql:
                throw new NotSupportedException("Portable migrations are ready, but the PostgreSQL migration runner is not implemented yet.");
            default:
                throw new InvalidOperationException($"Unsupported database provider '{databaseSettings.Provider}'.");
        }
    }

    private async Task<MigrationStatusSnapshot> GetSqliteStatusAsync(CancellationToken cancellationToken)
    {
        await using var connection = new SqliteConnection(databaseSettings.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        using var scope = serviceProvider.CreateScope();
        var migrationLoader = scope.ServiceProvider.GetRequiredService<IMigrationInformationLoader>();
        var versionLoader = scope.ServiceProvider.GetRequiredService<IVersionLoader>();

        var metadata = versionLoader.VersionTableMetaData;
        var appliedVersions = await LoadAppliedVersionsAsync(connection, metadata, cancellationToken);
        var migrations = migrationLoader.LoadMigrations()
            .Select(entry => new MigrationStatusItem(
                entry.Key,
                GetDescription(entry.Value),
                appliedVersions.Contains(entry.Key)))
            .OrderBy(entry => entry.Version)
            .ToArray();

        return new MigrationStatusSnapshot(
            hostEnvironment.EnvironmentName,
            hostEnvironment.ContentRootPath,
            FormatSqliteDatabaseTarget(connection),
            migrations);
    }

    private async Task<MigrationStatusSnapshot> MigrateSqliteUpAsync(TimeSpan lockTimeout, CancellationToken cancellationToken)
    {
        var before = await GetSqliteStatusAsync(cancellationToken);
        if (before.PendingCount == 0)
        {
            logger.LogInformation("No pending migrations were found.");
            return before;
        }

        var lockFilePath = GetSqliteLockFilePath();
        logger.LogInformation(
            "Attempting to acquire the SQLite migration file lock at {LockFilePath} with timeout {Timeout}.",
            lockFilePath,
            lockTimeout);

        await using var lockHandle = await AcquireFileLockAsync(lockFilePath, lockTimeout, cancellationToken);
        logger.LogInformation("SQLite migration file lock acquired.");

        var lockedStatus = await GetSqliteStatusAsync(cancellationToken);
        if (lockedStatus.PendingCount == 0)
        {
            logger.LogInformation("Another process already applied the pending migrations.");
            return lockedStatus;
        }

        logger.LogInformation(
            "Applying {PendingCount} pending migration(s) against {DatabaseTarget}.",
            lockedStatus.PendingCount,
            lockedStatus.DatabaseTarget);

        using (var scope = serviceProvider.CreateScope())
        {
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }

        var after = await GetSqliteStatusAsync(cancellationToken);
        logger.LogInformation(
            "Migration completed successfully. Applied count is now {AppliedCount}, pending count is {PendingCount}.",
            after.AppliedCount,
            after.PendingCount);

        return after;
    }

    private static string GetDescription(object migrationInfo)
    {
        var migrationInfoType = migrationInfo.GetType();
        var description = migrationInfoType.GetProperty("Description")?.GetValue(migrationInfo) as string;
        if (!string.IsNullOrWhiteSpace(description))
        {
            return description;
        }

        var migration = migrationInfoType.GetProperty("Migration")?.GetValue(migrationInfo);
        return migration?.GetType().Name ?? migrationInfoType.Name;
    }

    private static async Task<HashSet<long>> LoadAppliedVersionsAsync(
        SqliteConnection connection,
        IVersionTableMetaData metadata,
        CancellationToken cancellationToken)
    {
        await using (var existsCommand = connection.CreateCommand())
        {
            existsCommand.CommandText =
                """
                SELECT EXISTS (
                    SELECT 1
                    FROM sqlite_master
                    WHERE type = 'table'
                      AND name = $tableName
                );
                """;
            existsCommand.Parameters.AddWithValue("$tableName", metadata.TableName);

            var exists = Convert.ToInt32(await existsCommand.ExecuteScalarAsync(cancellationToken) ?? 0) == 1;
            if (!exists)
            {
                return [];
            }
        }

        var versions = new HashSet<long>();
        await using var selectCommand = connection.CreateCommand();
        selectCommand.CommandText =
            $"SELECT {QuoteIdentifier(metadata.ColumnName)} FROM {QuoteIdentifier(metadata.TableName)} ORDER BY {QuoteIdentifier(metadata.ColumnName)};";

        await using var reader = await selectCommand.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            versions.Add(reader.GetInt64(0));
        }

        return versions;
    }

    private static async Task<FileStream> AcquireFileLockAsync(
        string lockFilePath,
        TimeSpan lockTimeout,
        CancellationToken cancellationToken)
    {
        var timeoutAt = DateTimeOffset.UtcNow.Add(lockTimeout);
        var directory = Path.GetDirectoryName(lockFilePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        while (true)
        {
            try
            {
                return new FileStream(
                    lockFilePath,
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite,
                    FileShare.None);
            }
            catch (IOException) when (DateTimeOffset.UtcNow < timeoutAt)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }

    private static string QuoteIdentifier(string identifier)
    {
        return "\"" + identifier.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";
    }

    private static string FormatSqliteDatabaseTarget(SqliteConnection connection)
    {
        var builder = new SqliteConnectionStringBuilder(connection.ConnectionString);
        return string.IsNullOrWhiteSpace(builder.DataSource) ? "(unknown)" : builder.DataSource;
    }

    private string GetSqliteLockFilePath()
    {
        var builder = new SqliteConnectionStringBuilder(databaseSettings.ConnectionString);
        var dataSource = builder.DataSource;

        if (string.IsNullOrWhiteSpace(dataSource) ||
            string.Equals(dataSource, ":memory:", StringComparison.OrdinalIgnoreCase))
        {
            return Path.Combine(hostEnvironment.ContentRootPath, "data", "dash.migrations.lock");
        }

        return dataSource + ".migrations.lock";
    }
}
