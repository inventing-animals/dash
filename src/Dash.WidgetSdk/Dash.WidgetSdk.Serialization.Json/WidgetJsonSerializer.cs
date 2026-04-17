using System.Text.Json;
using Dash.WidgetSdk.Abstractions;

namespace Dash.WidgetSdk.Serialization.Json;

public static class WidgetJsonSerializer
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public static T? Deserialize<T>(JsonElement element)
    {
        return element.Deserialize<T>(Options);
    }

    public static JsonElement SerializeToElement<T>(T value)
    {
        return JsonSerializer.SerializeToElement(value, Options);
    }

    public static WidgetInstanceConfiguration CreateInstance<TConfiguration>(
        string instanceId,
        string widgetType,
        TConfiguration configuration)
    {
        return new(
            instanceId,
            widgetType,
            SerializeToElement(configuration));
    }

    public static WidgetStateEnvelope CreateStateEnvelope<TState>(
        string instanceId,
        string widgetType,
        TState state)
    {
        return new(
            instanceId,
            widgetType,
            DateTimeOffset.UtcNow,
            SerializeToElement(state));
    }
}
