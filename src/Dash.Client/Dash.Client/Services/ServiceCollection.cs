using Dash.Client.Api;
using Dash.Client.Server;
using Dash.Client.ViewModels;
using Dash.Client.WidgetHost;
using Dash.WidgetSdk.Avalonia;
using Dash.Widgets.DigitalClock.Client;
using Dash.Widgets.RandomNumber.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Dash.Client.Core;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<IContextService, ContextService>();
        collection.AddSingleton<ILocalServerLauncher, NoOpLocalServerLauncher>();
        collection.AddSingleton<IServerConnectionSettingsService, ServerConnectionSettingsService>();
        collection.AddSingleton<IServerConnectionRuntime, ServerConnectionRuntime>();
        collection.AddSingleton<IDashHubConnection, SignalRDashHubConnection>();

        // API client
        collection.AddSingleton<IDashApiClient, DashApiClient>();

        // Widget catalog
        collection.AddSingleton<IAvaloniaWidget, DigitalClockClientWidget>();
        collection.AddSingleton<IAvaloniaWidget, RandomNumberClientWidget>();
        collection.AddSingleton<IAvaloniaWidgetCatalog, AvaloniaWidgetCatalog>();

        collection.AddTransient<MainViewModel>();
        collection.AddTransient<ToolbarViewModel>();
        collection.AddTransient<DebugViewModel>();
        collection.AddTransient<DashboardViewModel>();
        collection.AddTransient<AboutViewModel>();
        collection.AddTransient<SettingsViewModel>();
    }
}
