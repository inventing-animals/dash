using Dash.WidgetSdk.Avalonia;

namespace Dash.Client.WidgetHost;

public interface IAvaloniaWidgetCatalog
{
    IReadOnlyCollection<IAvaloniaWidget> GetAll();

    bool TryGet(string widgetType, out IAvaloniaWidget? widget);
}

public sealed class AvaloniaWidgetCatalog : IAvaloniaWidgetCatalog
{
    private readonly IReadOnlyDictionary<string, IAvaloniaWidget> _widgets;

    public AvaloniaWidgetCatalog(IEnumerable<IAvaloniaWidget> widgets)
    {
        _widgets = widgets.ToDictionary(
            widget => widget.Definition.Type,
            StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyCollection<IAvaloniaWidget> GetAll()
    {
        return _widgets.Values.ToArray();
    }

    public bool TryGet(string widgetType, out IAvaloniaWidget? widget)
    {
        return _widgets.TryGetValue(widgetType, out widget);
    }
}
