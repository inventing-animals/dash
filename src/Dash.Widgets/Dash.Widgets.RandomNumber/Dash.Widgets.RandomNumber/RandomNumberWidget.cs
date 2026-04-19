using Dash.WidgetSdk.Abstractions;

namespace Dash.Widgets.RandomNumber;

public sealed record RandomNumberWidgetConfiguration(int Min, int Max)
{
    public static RandomNumberWidgetConfiguration Default { get; } = new(1, 100);
}

public sealed record RandomNumberWidgetState(int Value, DateTimeOffset GeneratedAtUtc);

public static class RandomNumberWidget
{
    public const string Type = "dash.widgets.random-number";

    public static WidgetDefinition Definition { get; } = new(
        Type,
        WidgetExecutionMode.ClientAndServer,
        new WidgetDisplayMetadata(
            "Random Number",
            "Server-executed widget that pushes a random number - used to test SignalR state sync."),
        typeof(RandomNumberWidgetConfiguration).FullName ?? nameof(RandomNumberWidgetConfiguration),
        typeof(RandomNumberWidgetState).FullName ?? nameof(RandomNumberWidgetState));
}
