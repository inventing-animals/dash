using Dash.Server.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Dash.Server.Persistence.Sqlite;

public sealed class SqliteDatabaseReadinessProbe : IDatabaseReadinessProbe
{
    private readonly DashDbContext _dbContext;

    public SqliteDatabaseReadinessProbe(DashDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken)
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);
            return canConnect
                ? HealthCheckResult.Healthy("SQLite database is ready.")
                : HealthCheckResult.Unhealthy("SQLite database is not reachable.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("SQLite database check failed.", exception);
        }
    }
}
