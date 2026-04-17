using Dash.Server.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dash.Server.Persistence.Sqlite;

public static class SqlitePersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddDashSqlitePersistence(
        this IServiceCollection services,
        DatabaseSettings databaseSettings)
    {
        services.AddDbContext<DashDbContext>(options => options.UseSqlite(databaseSettings.ConnectionString));
        services.AddScoped<IDatabaseReadinessProbe, SqliteDatabaseReadinessProbe>();

        return services;
    }
}
