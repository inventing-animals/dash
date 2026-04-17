using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Dash.WidgetSdk.Avalonia;
using Dash.WidgetSdk.Serialization.Json;

namespace Dash.Widgets.EmailChecker.Client;

public sealed class EmailCheckerClientWidget : IAvaloniaWidget
{
    public Dash.WidgetSdk.Abstractions.WidgetDefinition Definition => EmailCheckerWidget.Definition;

    public Control Build(AvaloniaWidgetRenderContext context)
    {
        var configuration = WidgetJsonSerializer.Deserialize<EmailCheckerWidgetConfiguration>(context.Instance.Configuration) ??
                            EmailCheckerWidgetConfiguration.Default;
        var state = context.State is null
            ? null
            : WidgetJsonSerializer.Deserialize<EmailCheckerWidgetState>(context.State.State);

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
                        FontSize = 22,
                        Text = $"{(state?.UnreadCount.ToString() ?? "-")} unread",
                    },
                    new TextBlock
                    {
                        FontSize = 12,
                        Text = $"{configuration.AccountLabel} / {configuration.Folder}",
                    },
                    new TextBlock
                    {
                        FontSize = 12,
                        Text = state?.Status ?? "Waiting for server state",
                    },
                },
            },
        };
    }
}
