namespace Dash.Server.Migrations;

public interface IMigrationCoordinator
{
    Task<MigrationStatusSnapshot> GetStatusAsync(CancellationToken cancellationToken);

    Task<MigrationStatusSnapshot> MigrateUpAsync(TimeSpan lockTimeout, CancellationToken cancellationToken);
}
