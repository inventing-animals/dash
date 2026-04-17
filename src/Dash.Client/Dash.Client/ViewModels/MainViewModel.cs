using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Dash.Client.Core;

namespace Dash.Client.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly Store _store;
    public ToolbarViewModel Toolbar { get; }
    public DebugViewModel Debug { get; }
    public DashboardViewModel Dashboard { get; }
    public SettingsViewModel Settings { get; }
    public AboutViewModel About { get; }

    public MainViewModel(IContextService contextService, ToolbarViewModel toolbar, DebugViewModel debug, DashboardViewModel dashboard, SettingsViewModel settings, AboutViewModel about)
    {
        _store = contextService?.getStore() ?? throw new ArgumentNullException(nameof(contextService));
        _store.StateChanged += OnStateChanged;
        Toolbar = toolbar ?? throw new ArgumentNullException(nameof(toolbar));
        Debug = debug ?? throw new ArgumentNullException(nameof(debug));
        Dashboard = dashboard ?? throw new ArgumentNullException(nameof(dashboard));
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        About = about ?? throw new ArgumentNullException(nameof(about));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public State State => _store.State;

    public Mode Mode => State.Mode;

    public bool IsDashboard => Mode == Mode.Dashboard;
    public bool IsSettings => Mode == Mode.Settings;

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