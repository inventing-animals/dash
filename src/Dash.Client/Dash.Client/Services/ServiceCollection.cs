using Dash.Client.Server;
using Dash.Client.ViewModels;
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
        collection.AddTransient<MainViewModel>();
        collection.AddTransient<ToolbarViewModel>();
        collection.AddTransient<DebugViewModel>();
        collection.AddTransient<DashboardViewModel>();
        collection.AddTransient<AboutViewModel>();
        collection.AddTransient<SettingsViewModel>();
    }
}
