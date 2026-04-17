using Dash.Server.Migrations;
using Dash.Server.Persistence;

namespace Dash.Server.Api.Config;

public static class MigrationConfig
{
    private const string SectionName = "Migrations";

    public static void Register(
        IServiceCollection services,
        DatabaseSettings databaseSettings)
    {
        services.AddDashMigrations(databaseSettings);
    }

    public static async Task EnsureUpToDateAsync(WebApplication app)
    {
        var settings = GetSettings(app.Configuration);
        if (!settings.RunOnStartup)
        {
            return;
        }

        var coordinator = app.Services.GetRequiredService<IMigrationCoordinator>();
        await coordinator.MigrateUpAsync(
            TimeSpan.FromSeconds(settings.LockTimeoutSeconds),
            app.Lifetime.ApplicationStopping);
    }

    private static MigrationStartupSettings GetSettings(IConfiguration configuration)
    {
        var section = configuration.GetSection(SectionName);
        var options = section.Get<MigrationStartupOptions>() ?? new MigrationStartupOptions();
        return new MigrationStartupSettings(options.RunOnStartup, options.LockTimeoutSeconds);
    }
}

internal sealed class MigrationStartupOptions
{
    public bool RunOnStartup { get; init; } = true;

    public int LockTimeoutSeconds { get; init; } = 30;
}

public sealed record MigrationStartupSettings(bool RunOnStartup, int LockTimeoutSeconds);
