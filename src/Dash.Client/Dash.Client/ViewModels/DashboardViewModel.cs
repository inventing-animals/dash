using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using Dash.Client.Api;
using Dash.Client.Core;
using Dash.Client.Server;
using Dash.Client.WidgetHost;
using Dash.WidgetSdk.Abstractions;
using Dash.WidgetSdk.Avalonia;

namespace Dash.Client.ViewModels;

public sealed class DashboardViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly Store _store;
    private readonly IDashApiClient _apiClient;
    private readonly IDashHubConnection _hubConnection;
    private readonly IAvaloniaWidgetCatalog _widgetCatalog;
    private readonly SemaphoreSlim _subscriptionGate = new(1, 1);
    private Guid? _subscribedPageId;
    private bool _hubStarted;

    public ObservableCollection<Control> WidgetControls { get; } = [];

    private string? _statusMessage = "Loading…";
    public string? StatusMessage
    {
        get => _statusMessage;
        private set
        {
            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    public DashboardViewModel(
        IContextService contextService,
        IDashApiClient apiClient,
        IDashHubConnection hubConnection,
        IAvaloniaWidgetCatalog widgetCatalog)
    {
        _store = contextService?.getStore() ?? throw new ArgumentNullException(nameof(contextService));
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _hubConnection = hubConnection ?? throw new ArgumentNullException(nameof(hubConnection));
        _widgetCatalog = widgetCatalog ?? throw new ArgumentNullException(nameof(widgetCatalog));

        _store.StateChanged += OnStateChanged;
        _hubConnection.WidgetStatesReceived += OnWidgetStatesReceived;
        _ = LoadPagesAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public State State => _store.State;
    public PageData? SelectedPage => State.Pages.FirstOrDefault(page => page.PageId == State.CurrentPageId);
    public string SelectedPageName => SelectedPage?.Name ?? "Dashboard";
    public int SelectedPageWidgetCount => SelectedPage?.Widgets.Count ?? 0;
    public Mode Mode => State.Mode;
    public bool IsDashboard => Mode == Mode.Dashboard;
    public bool IsSettings => Mode == Mode.Settings;

    public void GoDashboard() => _store.Dispatch(new Navigate(Mode.Dashboard));
    public void GoSettings() => _store.Dispatch(new Navigate(Mode.Settings));

    private async Task LoadPagesAsync()
    {
        // Retry with exponential backoff so a slow-starting server is handled gracefully.
        var delays = new[] { 2, 4, 8, 15, 30 };

        for (var attempt = 0; ; attempt++)
        {
            try
            {
                var pages = await _apiClient.GetPagesAsync();
                _store.Dispatch(new PagesLoaded(pages));
                _ = EnsureLiveStateSubscriptionAsync(_store.State.CurrentPageId);
                return;
            }
            catch (Exception ex) when (attempt < delays.Length)
            {
                var wait = delays[attempt];
                Console.WriteLine($"[DashboardViewModel] Attempt {attempt + 1} failed ({ex.Message}), retrying in {wait}s…");
                await Dispatcher.UIThread.InvokeAsync(() =>
                    StatusMessage = $"Connecting… (retry {attempt + 1} of {delays.Length + 1})");
                await Task.Delay(TimeSpan.FromSeconds(wait));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DashboardViewModel] All attempts exhausted: {ex}");
                await Dispatcher.UIThread.InvokeAsync(() =>
                    StatusMessage = $"Could not reach server: {ex.Message}");
                return;
            }
        }
    }

    private void OnStateChanged(object? sender, State newState)
    {
        if (newState.CurrentPageId != _subscribedPageId)
        {
            _ = EnsureLiveStateSubscriptionAsync(newState.CurrentPageId);
        }

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            try
            {
                RebuildWidgetControls();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DashboardViewModel] RebuildWidgetControls failed: {ex}");
                StatusMessage = $"Render error: {ex.Message}";
            }
        });

        OnPropertyChanged(nameof(State));
        OnPropertyChanged(nameof(SelectedPage));
        OnPropertyChanged(nameof(SelectedPageName));
        OnPropertyChanged(nameof(SelectedPageWidgetCount));
        OnPropertyChanged(nameof(Mode));
        OnPropertyChanged(nameof(IsDashboard));
        OnPropertyChanged(nameof(IsSettings));
    }

    private void OnWidgetStatesReceived(IReadOnlyList<WidgetStateEnvelope> states)
    {
        _store.Dispatch(new WidgetStatesReceived(states));
    }

    private async Task EnsureLiveStateSubscriptionAsync(Guid? pageId, CancellationToken ct = default)
    {
        if (pageId is null)
        {
            return;
        }

        await _subscriptionGate.WaitAsync(ct);

        try
        {
            if (!_hubStarted)
            {
                await _hubConnection.StartAsync(ct);
                _hubStarted = true;
            }

            if (_subscribedPageId == pageId)
            {
                return;
            }

            if (_subscribedPageId is Guid previousPageId)
            {
                await _hubConnection.UnsubscribeFromPageAsync(previousPageId, ct);
            }

            await _hubConnection.SubscribeToPageAsync(pageId.Value, ct);
            _subscribedPageId = pageId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DashboardViewModel] Live widget subscription failed: {ex}");
        }
        finally
        {
            _subscriptionGate.Release();
        }
    }

    private void RebuildWidgetControls()
    {
        var state = _store.State;
        var page = SelectedPage;

        WidgetControls.Clear();

        if (page is null)
        {
            StatusMessage = state.Pages.Count == 0 ? "Loading…" : "No page selected.";
            return;
        }

        if (page.Widgets.Count == 0)
        {
            StatusMessage = $"\"{page.Name}\" has no widgets yet.";
            return;
        }

        StatusMessage = null;

        foreach (var widget in page.Widgets)
        {
            Control control;
            try
            {
                if (_widgetCatalog.TryGet(widget.WidgetType, out var avaloniaWidget) && avaloniaWidget is not null)
                {
                    var configJson = widget.ConfigurationJson ?? "{}";
                    var configElement = JsonSerializer.Deserialize<JsonElement>(configJson);
                    var instance = new WidgetInstanceConfiguration(
                        widget.WidgetId.ToString(),
                        widget.WidgetType,
                        configElement);

                    state.WidgetStates.TryGetValue(widget.WidgetId.ToString(), out var envelope);

                    control = avaloniaWidget.Build(new AvaloniaWidgetRenderContext(instance, envelope));
                }
                else
                {
                    control = new TextBlock { Text = $"Unknown widget: {widget.WidgetType}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DashboardViewModel] Failed to build widget {widget.WidgetType}: {ex}");
                control = new TextBlock { Text = $"Error: {widget.WidgetType}" };
            }

            WidgetControls.Add(control);
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public void Dispose()
    {
        _store.StateChanged -= OnStateChanged;
        _hubConnection.WidgetStatesReceived -= OnWidgetStatesReceived;
        _subscriptionGate.Dispose();
    }
}
