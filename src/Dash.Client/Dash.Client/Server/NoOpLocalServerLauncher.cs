namespace Dash.Client.Server;

public sealed class NoOpLocalServerLauncher : ILocalServerLauncher
{
    public void Start(string executablePath)
    {
    }

    public void Stop()
    {
    }
}
