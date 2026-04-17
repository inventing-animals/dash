using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dash.Server.Persistence;

public static class PersistenceServiceCollectionExtensions
{
    public static DatabaseSettings AddDashPersistenceCore(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var databaseSettings = DatabaseSettingsResolver.Resolve(configuration, environment);
        services.AddSingleton(databaseSettings);
        return databaseSettings;
    }
}
