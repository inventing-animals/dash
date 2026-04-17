using System;

namespace Dash.Client.Server;

public sealed class ServerConnectionRuntime : IServerConnectionRuntime
{
    private readonly ILocalServerLauncher _localServerLauncher;

    public ServerConnectionRuntime(ILocalServerLauncher localServerLauncher)
    {
        _localServerLauncher = localServerLauncher ?? throw new ArgumentNullException(nameof(localServerLauncher));
    }

    public void Apply(ServerConnectionSettings settings)
    {
        if (settings.Mode == ServerConnectionMode.Local)
        {
            _localServerLauncher.Start(settings.LocalExecutablePath);
            return;
        }

        _localServerLauncher.Stop();
    }
}
