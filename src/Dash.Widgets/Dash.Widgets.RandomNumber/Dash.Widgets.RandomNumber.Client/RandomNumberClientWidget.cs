using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Dash.WidgetSdk.Avalonia;
using Dash.WidgetSdk.Serialization.Json;

namespace Dash.Widgets.RandomNumber.Client;

public sealed class RandomNumberClientWidget : IAvaloniaWidget
{
    public Dash.WidgetSdk.Abstractions.WidgetDefinition Definition => RandomNumberWidget.Definition;

    public Control Build(AvaloniaWidgetRenderContext context)
    {
        var configuration =
            WidgetJsonSerializer.Deserialize<RandomNumberWidgetConfiguration>(context.Instance.Configuration)
            ?? RandomNumberWidgetConfiguration.Default;

        var state = context.State is null
            ? null
            : WidgetJsonSerializer.Deserialize<RandomNumberWidgetState>(context.State.State);

        return new Border
        {
            Padding = new Thickness(16),
            Child = new StackPanel
            {
                Spacing = 4,
                HorizontalAlignment = HorizontalAlignment.Center,
                Children =
                {
                    new TextBlock
                    {
                        FontSize = 14,
                        Text = Definition.Display.Name,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    },
                    new TextBlock
                    {
                        FontSize = 48,
                        FontWeight = Avalonia.Media.FontWeight.Bold,
                        Text = state?.Value.ToString() ?? "-",
                        HorizontalAlignment = HorizontalAlignment.Center,
                    },
                    new TextBlock
                    {
                        FontSize = 12,
                        Text = $"Range: {configuration.Min} – {configuration.Max}",
                        HorizontalAlignment = HorizontalAlignment.Center,
                    },
                    new TextBlock
                    {
                        FontSize = 11,
                        Text = state is null
                            ? "Waiting for server state"
                            : $"Generated {state.GeneratedAtUtc:HH:mm:ss} UTC",
                        HorizontalAlignment = HorizontalAlignment.Center,
                    },
                },
            },
        };
    }
}
