using Dash.Server.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Dash.Server.Persistence.PostgreSql;

public static class PostgreSqlPersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddDashPostgreSqlPersistence(
        this IServiceCollection services,
        DatabaseSettings databaseSettings)
    {
        throw new NotSupportedException(
            $"Database provider '{databaseSettings.Provider}' is planned but not implemented yet.");
    }
}
