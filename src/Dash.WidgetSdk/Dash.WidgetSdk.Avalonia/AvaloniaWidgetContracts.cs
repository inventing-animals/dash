using Avalonia.Controls;
using Dash.WidgetSdk.Abstractions;

namespace Dash.WidgetSdk.Avalonia;

public sealed record AvaloniaWidgetRenderContext(
    WidgetInstanceConfiguration Instance,
    WidgetStateEnvelope? State);

public interface IAvaloniaWidget
{
    WidgetDefinition Definition { get; }

    Control Build(AvaloniaWidgetRenderContext context);
}
