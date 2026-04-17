using Dash.Server.Api.Config;
using Dash.Server.Api.Endpoints;
using Dash.Server.Observability;
using Dash.Server.Persistence;
using Dash.Server.Persistence.PostgreSql;
using Dash.Server.Persistence.Sqlite;

var builder = WebApplication.CreateBuilder(args);

var databaseSettings = builder.Services.AddDashPersistenceCore(builder.Configuration, builder.Environment);
switch (databaseSettings.Provider)
{
    case DatabaseProvider.Sqlite:
        builder.Services.AddDashSqlitePersistence(databaseSettings);
        break;
    case DatabaseProvider.PostgreSql:
        builder.Services.AddDashPostgreSqlPersistence(databaseSettings);
        break;
    default:
        throw new InvalidOperationException($"Unsupported database provider '{databaseSettings.Provider}'.");
}

MigrationConfig.Register(builder.Services, databaseSettings);
HealthChecksConfig.Register(builder.Services);
OpenApiConfig.Register(builder.Services);

var app = builder.Build();

await MigrationConfig.EnsureUpToDateAsync(app);

OpenApiConfig.Use(app);
HealthChecksConfig.Map(app);
app.MapUserEndpoints();

app.Run();

public partial class Program;
