using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace Dash.Server.Api.Config;

public static class OpenApiConfig
{
    public static void Register(IServiceCollection services)
    {
        services.AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "Dash Server API",
                    Version = "v1",
                };

                return Task.CompletedTask;
            });
        });
    }

    public static void Use(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi("/openapi/{documentName}.json");
            app.MapScalarApiReference();
            app.MapGet("/", () => Results.Redirect("/scalar")).AllowAnonymous();
        }
    }
}
