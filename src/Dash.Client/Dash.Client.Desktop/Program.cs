using System;
using System.IO;
using Avalonia;
using Dash.Client.Desktop.Server;
using Dash.Client.Server;
using Ink.Platform.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Dash.Client.Desktop;

internal sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var settings = new FileSettingsService(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Dash",
            "settings.json"));

        BuildAvaloniaApp(settings)
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp(ISettingsService settings)
    {
        return AppBuilder.Configure(() => new App(
                settings,
                services => services.AddSingleton<ILocalServerLauncher, DesktopChildProcessServerLauncher>()))
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}
