using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Dash.Client.Core;

namespace Dash.Client.ViewModels;

public sealed class DashboardViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly Store _store;

    public DashboardViewModel(IContextService contextService)
    {
        _store = contextService?.getStore() ?? throw new ArgumentNullException(nameof(contextService));
        _store.StateChanged += OnStateChanged;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public State State => _store.State;

    public Mode Mode => State.Mode;

    public bool IsDashboard => Mode == Mode.Dashboard;
    public bool IsSettings => Mode == Mode.Settings;

    public void GoDashboard()
    {
        _store.Dispatch(new Navigate(Mode.Dashboard));
    }

    public void GoSettings()
    {
        _store.Dispatch(new Navigate(Mode.Settings));
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