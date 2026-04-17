namespace Dash.Client.Server;

public interface IServerConnectionSettingsService
{
    ServerConnectionSettings Get();

    void Save(ServerConnectionSettings settings);
}
