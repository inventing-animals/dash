using Dash.Server.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Dash.Server.Observability;

public static class HealthChecksConfig
{
    public static void Register(IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
            .AddCheck<DatabaseReadinessHealthCheck>("database", tags: ["ready"]);
    }

    public static void Map(WebApplication app)
    {
        app.MapHealthChecks(
                "/healthz/live",
                new HealthCheckOptions
                {
                    Predicate = registration => registration.Tags.Contains("live"),
                })
            .AllowAnonymous();

        app.MapHealthChecks(
                "/healthz/ready",
                new HealthCheckOptions
                {
                    Predicate = registration => registration.Tags.Contains("ready"),
                })
            .AllowAnonymous();
    }
}
