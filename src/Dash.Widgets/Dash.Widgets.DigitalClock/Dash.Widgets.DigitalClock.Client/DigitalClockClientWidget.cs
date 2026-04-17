using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Dash.WidgetSdk.Avalonia;
using Dash.WidgetSdk.Serialization.Json;

namespace Dash.Widgets.DigitalClock.Client;

public sealed class DigitalClockClientWidget : IAvaloniaWidget
{
    public Dash.WidgetSdk.Abstractions.WidgetDefinition Definition => DigitalClockWidget.Definition;

    public Control Build(AvaloniaWidgetRenderContext context)
    {
        var configuration = WidgetJsonSerializer.Deserialize<DigitalClockWidgetConfiguration>(context.Instance.Configuration) ??
                            DigitalClockWidgetConfiguration.Default;
        var timeZone = ResolveTimeZone(configuration.TimeZoneId);
        var now = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, timeZone);
        var format = string.IsNullOrWhiteSpace(configuration.Format)
            ? DigitalClockWidgetConfiguration.Default.Format
            : configuration.Format;

        return new Border
        {
            Padding = new Thickness(16),
            Child = new StackPanel
            {
                Spacing = 4,
                Children =
                {
                    new TextBlock
                    {
                        FontSize = 14,
                        Text = Definition.Display.Name,
                    },
                    new TextBlock
                    {
                        FontSize = 28,
                        Text = now.ToString(format),
                    },
                    new TextBlock
                    {
                        FontSize = 12,
                        Text = timeZone.Id,
                    },
                },
            },
        };
    }

    private static TimeZoneInfo ResolveTimeZone(string timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId))
        {
            return TimeZoneInfo.Utc;
        }

        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.Utc;
        }
        catch (InvalidTimeZoneException)
        {
            return TimeZoneInfo.Utc;
        }
    }
}
