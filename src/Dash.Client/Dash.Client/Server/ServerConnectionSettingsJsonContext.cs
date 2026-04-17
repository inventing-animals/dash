using System.Text.Json.Serialization;

namespace Dash.Client.Server;

[JsonSerializable(typeof(ServerConnectionSettings))]
internal partial class ServerConnectionSettingsJsonContext : JsonSerializerContext
{
}
