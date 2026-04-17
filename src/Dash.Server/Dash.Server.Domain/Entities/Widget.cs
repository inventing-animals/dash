namespace Dash.Server.Domain.Entities;

public sealed class Widget
{
    public Guid WidgetId { get; set; }

    public Guid PageId { get; set; }

    public string WidgetType { get; set; } = string.Empty;

    public string ConfigurationJson { get; set; } = "{}";

    public Page Page { get; set; } = null!;
}
