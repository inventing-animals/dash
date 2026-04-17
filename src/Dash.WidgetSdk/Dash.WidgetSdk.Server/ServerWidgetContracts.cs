using Dash.WidgetSdk.Abstractions;

namespace Dash.WidgetSdk.Server;

public sealed record ServerWidgetExecutionRequest(
    WidgetInstanceConfiguration Instance);

public interface IServerWidget
{
    WidgetDefinition Definition { get; }

    ValueTask<WidgetStateEnvelope> ExecuteAsync(
        ServerWidgetExecutionRequest request,
        CancellationToken cancellationToken);
}
