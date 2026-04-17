using Dash.Server.Observability;

var builder = WebApplication.CreateBuilder(args);

HealthChecksConfig.Register(builder.Services);

var app = builder.Build();

HealthChecksConfig.Map(app);

app.Run();

public partial class Program;
