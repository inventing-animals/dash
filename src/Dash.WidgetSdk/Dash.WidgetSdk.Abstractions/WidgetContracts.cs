using System.Text.Json;

namespace Dash.WidgetSdk.Abstractions;

public enum WidgetExecutionMode
{
    ClientOnly,
    ClientAndServer,
}

public sealed record WidgetDisplayMetadata(
    string Name,
    string Description);

public sealed record WidgetDefinition(
    string Type,
    WidgetExecutionMode ExecutionMode,
    WidgetDisplayMetadata Display,
    string ConfigurationType,
    string? StateType = null);

public sealed record WidgetInstanceConfiguration(
    string InstanceId,
    string WidgetType,
    JsonElement Configuration);

public sealed record WidgetStateEnvelope(
    string InstanceId,
    string WidgetType,
    int StateVersion,
    DateTimeOffset UpdatedAtUtc,
    JsonElement State);
 