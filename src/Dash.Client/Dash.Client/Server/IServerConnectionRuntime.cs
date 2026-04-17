namespace Dash.Client.Server;

public interface IServerConnectionRuntime
{
    void Apply(ServerConnectionSettings settings);
}
