using Dash.Client.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Dash.Client.Core;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<IContextService, ContextService>();
        collection.AddTransient<MainViewModel>();
        collection.AddTransient<ToolbarViewModel>();
        collection.AddTransient<DebugViewModel>();
        collection.AddTransient<DashboardViewModel>();
        collection.AddTransient<AboutViewModel>();
        collection.AddTransient<SettingsViewModel>();
    }
}