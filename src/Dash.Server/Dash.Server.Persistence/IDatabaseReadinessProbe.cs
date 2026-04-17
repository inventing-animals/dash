using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Dash.Server.Persistence;

public interface IDatabaseReadinessProbe
{
    Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken);
}
