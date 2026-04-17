using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using Dash.Client.Core;
using Dash.Client.Server;

namespace Dash.Client.ViewModels;

public sealed class SettingsViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly Store _store;
    private readonly IServerConnectionRuntime _serverConnectionRuntime;
    private readonly IServerConnectionSettingsService _serverConnectionSettingsService;
    private string _localExecutablePath;
    private string _remoteHostUrl;
    private ServerConnectionMode _selectedServerMode;
    private string _statusMessage = string.Empty;

    public SettingsViewModel(
        IContextService contextService,
        IServerConnectionSettingsService serverConnectionSettingsService,
        IServerConnectionRuntime serverConnectionRuntime)
    {
        _store = contextService?.getStore() ?? throw new ArgumentNullException(nameof(contextService));
        _serverConnectionSettingsService = serverConnectionSettingsService ?? throw new ArgumentNullException(nameof(serverConnectionSettingsService));
        _serverConnectionRuntime = serverConnectionRuntime ?? throw new ArgumentNullException(nameof(serverConnectionRuntime));
        _store.StateChanged += OnStateChanged;

        var settings = _serverConnectionSettingsService.Get();
        _selectedServerMode = settings.Mode;
        _localExecutablePath = settings.LocalExecutablePath;
        _remoteHostUrl = settings.RemoteHostUrl;

        SaveCommand = new RelayCommand(Save);
        GoDashboardCommand = new RelayCommand(GoDashboard);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public RelayCommand SaveCommand { get; }

    public RelayCommand GoDashboardCommand { get; }

    public State State => _store.State;

    public Mode Mode => State.Mode;

    public bool IsDashboard => Mode == Mode.Dashboard;

    public bool IsSettings => Mode == Mode.Settings;

    public Array ServerModes => Enum.GetValues<ServerConnectionMode>();

    public ServerConnectionMode SelectedServerMode
    {
        get => _selectedServerMode;
        set
        {
            if (_selectedServerMode == value)
            {
                return;
            }

            _selectedServerMode = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsLocalMode));
            OnPropertyChanged(nameof(IsRemoteMode));
        }
    }

    public bool IsLocalMode => SelectedServerMode == ServerConnectionMode.Local;

    public bool IsRemoteMode => SelectedServerMode == ServerConnectionMode.Remote;

    public string LocalExecutablePath
    {
        get => _localExecutablePath;
        set
        {
            if (string.Equals(_localExecutablePath, value, StringComparison.Ordinal))
            {
                return;
            }

            _localExecutablePath = value;
            OnPropertyChanged();
        }
    }

    public string RemoteHostUrl
    {
        get => _remoteHostUrl;
        set
        {
            if (string.Equals(_remoteHostUrl, value, StringComparison.Ordinal))
            {
                return;
            }

            _remoteHostUrl = value;
            OnPropertyChanged();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set
        {
            if (string.Equals(_statusMessage, value, StringComparison.Ordinal))
            {
                return;
            }

            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    public void GoDashboard()
    {
        _store.Dispatch(new Navigate(Mode.Dashboard));
    }

    public void GoSettings()
    {
        _store.Dispatch(new Navigate(Mode.Settings));
    }

    public void Save()
    {
        var settings = new ServerConnectionSettings(
            SelectedServerMode,
            LocalExecutablePath.Trim(),
            RemoteHostUrl.Trim());

        _serverConnectionSettingsService.Save(settings);
        _serverConnectionRuntime.Apply(settings);

        StatusMessage = settings.Mode switch
        {
            ServerConnectionMode.Local => "Saved local server settings.",
            ServerConnectionMode.Remote => "Saved remote server settings.",
            _ => "Saved settings.",
        };
    }

    private void OnStateChanged(object? sender, State newState)
    {
        OnPropertyChanged(nameof(State));
        OnPropertyChanged(nameof(Mode));
        OnPropertyChanged(nameof(IsDashboard));
        OnPropertyChanged(nameof(IsSettings));
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public void Dispose()
    {
        _store.StateChanged -= OnStateChanged;
    }
}
