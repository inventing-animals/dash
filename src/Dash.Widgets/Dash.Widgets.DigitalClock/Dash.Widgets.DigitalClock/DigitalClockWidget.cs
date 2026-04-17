using Dash.WidgetSdk.Abstractions;

namespace Dash.Widgets.DigitalClock;

public sealed record DigitalClockWidgetConfiguration(
    string TimeZoneId,
    string Format)
{
    public static DigitalClockWidgetConfiguration Default { get; } =
        new("UTC", "HH:mm:ss");
}

public static class DigitalClockWidget
{
    public const string Type = "dash.widgets.digital-clock";

    public static WidgetDefinition Definition { get; } = new(
        Type,
        WidgetExecutionMode.ClientOnly,
        new WidgetDisplayMetadata(
            "Digital Clock",
            "Client-rendered clock widget with server-stored configuration."),
        typeof(DigitalClockWidgetConfiguration).FullName ?? nameof(DigitalClockWidgetConfiguration));
}
