using Dash.WidgetSdk.Abstractions;

namespace Dash.WidgetSdk.Server;

public sealed record ServerWidgetExecutionRequest(
    WidgetInstanceConfiguration Instance);

public interface IServerWidget
{
    WidgetDefinition Definition { get; }

    TimeSpan GetRefreshInterval(WidgetInstanceConfiguration instance)
        => TimeSpan.FromMinutes(1);

    ValueTask<WidgetStateEnvelope> ExecuteAsync(
        ServerWidgetExecutionRequest request,
        CancellationToken cancellationToken);
}
