using System;
using Ink.Platform.Settings;

namespace Dash.Client.Server;

public sealed class ServerConnectionSettingsService : IServerConnectionSettingsService
{
    private const string SettingsKey = "server.connection";
    private readonly ISettingsService _settingsService;

    public ServerConnectionSettingsService(ISettingsService settingsService)
    {
        _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
    }

    public ServerConnectionSettings Get()
    {
        return _settingsService.Get(
                   SettingsKey,
                   ServerConnectionSettingsJsonContext.Default.ServerConnectionSettings) ??
               ServerConnectionSettings.Default;
    }

    public void Save(ServerConnectionSettings settings)
    {
        _settingsService.Set(
            SettingsKey,
            settings,
            ServerConnectionSettingsJsonContext.Default.ServerConnectionSettings);
    }
}
