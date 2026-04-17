using Dash.WidgetSdk.Server;

namespace Dash.Server.WidgetHost;

public interface IServerWidgetCatalog
{
    IReadOnlyCollection<IServerWidget> GetAll();

    bool TryGet(string widgetType, out IServerWidget? widget);
}

public sealed class ServerWidgetCatalog : IServerWidgetCatalog
{
    private readonly IReadOnlyDictionary<string, IServerWidget> _widgets;

    public ServerWidgetCatalog(IEnumerable<IServerWidget> widgets)
    {
        _widgets = widgets.ToDictionary(
            widget => widget.Definition.Type,
            StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyCollection<IServerWidget> GetAll()
    {
        return _widgets.Values.ToArray();
    }

    public bool TryGet(string widgetType, out IServerWidget? widget)
    {
        return _widgets.TryGetValue(widgetType, out widget);
    }
}
