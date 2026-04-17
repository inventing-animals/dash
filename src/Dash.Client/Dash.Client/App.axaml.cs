using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Dash.Client.Core;
using Dash.Client.Server;
using Dash.Client.ViewModels;
using Dash.Client.Views;
using Ink.Platform.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Dash.Client;

public partial class App : Application
{
    private readonly Action<IServiceCollection>? _configurePlatformServices;
    private readonly ISettingsService _settingsService;
    private ServiceProvider? _services;

    public App()
        : this(new InMemorySettingsService())
    {
    }

    public App(
        ISettingsService settingsService,
        Action<IServiceCollection>? configurePlatformServices = null)
    {
        _settingsService = settingsService;
        _configurePlatformServices = configurePlatformServices;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.AddSingleton(_settingsService);
        collection.AddCommonServices();
        _configurePlatformServices?.Invoke(collection);

        _services = collection.BuildServiceProvider();

        var serverConnectionSettings = _services.GetRequiredService<IServerConnectionSettingsService>();
        var serverConnectionRuntime = _services.GetRequiredService<IServerConnectionRuntime>();
        serverConnectionRuntime.Apply(serverConnectionSettings.Get());

        var vm = _services.GetRequiredService<MainViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = vm,
            };
            desktop.Exit += OnDesktopExit;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = vm,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnDesktopExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        _services?.Dispose();
        _services = null;
    }
}
