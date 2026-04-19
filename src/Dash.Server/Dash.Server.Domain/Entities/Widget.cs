namespace Dash.Server.Domain.Entities;

public sealed class Widget
{
    public Guid WidgetId { get; set; }

    public Guid PageId { get; set; }

    public string WidgetType { get; set; } = string.Empty;

    public string ConfigurationJson { get; set; } = "{}";

    /// <summary>Last-known serialized <see cref="Dash.WidgetSdk.Abstractions.WidgetStateEnvelope"/>. Null until the widget has executed at least once.</summary>
    public string? StateJson { get; set; }

    public Page Page { get; set; } = null!;
}
