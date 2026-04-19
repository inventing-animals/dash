using Dash.WidgetSdk.Abstractions;
using Dash.WidgetSdk.Serialization.Json;
using Dash.WidgetSdk.Server;

namespace Dash.Widgets.RandomNumber.Server;

public sealed class RandomNumberServerWidget : IServerWidget
{
    public WidgetDefinition Definition => RandomNumberWidget.Definition;

    public TimeSpan GetRefreshInterval(WidgetInstanceConfiguration instance)
        => TimeSpan.FromSeconds(5);

    public ValueTask<WidgetStateEnvelope> ExecuteAsync(
        ServerWidgetExecutionRequest request,
        CancellationToken cancellationToken)
    {
        var configuration =
            WidgetJsonSerializer.Deserialize<RandomNumberWidgetConfiguration>(request.Instance.Configuration)
            ?? RandomNumberWidgetConfiguration.Default;

        var value = Random.Shared.Next(configuration.Min, configuration.Max + 1);
        var state = new RandomNumberWidgetState(value, DateTimeOffset.UtcNow);

        return ValueTask.FromResult(
            WidgetJsonSerializer.CreateStateEnvelope(
                request.Instance.InstanceId,
                request.Instance.WidgetType,
                state));
    }
}
