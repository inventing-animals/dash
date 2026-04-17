namespace Dash.Client.Server;

public interface ILocalServerLauncher
{
    void Start(string executablePath);

    void Stop();
}
