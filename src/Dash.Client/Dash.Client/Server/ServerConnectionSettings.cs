namespace Dash.Client.Server;

public sealed record ServerConnectionSettings(
    ServerConnectionMode Mode,
    string LocalExecutablePath,
    string RemoteHostUrl)
{
    public static ServerConnectionSettings Default { get; } =
        new(
            ServerConnectionMode.Remote,
            string.Empty,
            "http://127.0.0.1:5190");
}
