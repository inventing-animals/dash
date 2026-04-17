using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Dash.Server.Persistence;

public sealed class DatabaseReadinessHealthCheck : IHealthCheck
{
    private readonly IDatabaseReadinessProbe _probe;

    public DatabaseReadinessHealthCheck(IDatabaseReadinessProbe probe)
    {
        _probe = probe;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return _probe.CheckHealthAsync(cancellationToken);
    }
}
