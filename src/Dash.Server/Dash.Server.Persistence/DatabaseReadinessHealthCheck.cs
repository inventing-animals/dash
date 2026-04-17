using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Dash.Server.Persistence;

public sealed class DatabaseReadinessHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("No database configured yet."));
    }
}
