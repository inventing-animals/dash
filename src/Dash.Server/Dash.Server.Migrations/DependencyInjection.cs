using Dash.Server.Persistence;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace Dash.Server.Migrations;

public static class DependencyInjection
{
    public static IServiceCollection AddDashMigrations(
        this IServiceCollection services,
        DatabaseSettings databaseSettings)
    {
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(runner =>
            {
                switch (databaseSettings.Provider)
                {
                    case DatabaseProvider.Sqlite:
                        runner.AddSQLite();
                        break;
                    case DatabaseProvider.PostgreSql:
                        throw new NotSupportedException("Portable migrations are ready, but the PostgreSQL runner is not wired yet.");
                    default:
                        throw new InvalidOperationException($"Unsupported database provider '{databaseSettings.Provider}'.");
                }

                runner
                    .WithGlobalConnectionString(databaseSettings.ConnectionString)
                    .ScanIn(typeof(MigrationMarker).Assembly).For.Migrations();
            });

        services.AddSingleton<IMigrationCoordinator, MigrationCoordinator>();

        return services;
    }
}
