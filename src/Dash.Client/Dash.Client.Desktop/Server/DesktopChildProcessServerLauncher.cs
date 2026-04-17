using System;
using System.Diagnostics;
using System.IO;
using Dash.Client.Server;

namespace Dash.Client.Desktop.Server;

public sealed class DesktopChildProcessServerLauncher : ILocalServerLauncher, IDisposable
{
    private string? _currentExecutablePath;
    private Process? _process;

    public void Start(string executablePath)
    {
        var normalizedPath = executablePath.Trim();
        if (string.IsNullOrWhiteSpace(normalizedPath))
        {
            Stop();
            return;
        }

        if (_process is { HasExited: false } &&
            string.Equals(_currentExecutablePath, normalizedPath, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Stop();

        if (!File.Exists(normalizedPath))
        {
            Console.WriteLine($"Local server executable not found: {normalizedPath}");
            return;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = normalizedPath,
            WorkingDirectory = Path.GetDirectoryName(normalizedPath) ?? AppContext.BaseDirectory,
            UseShellExecute = false,
        };

        _process = Process.Start(startInfo);
        _currentExecutablePath = normalizedPath;
    }

    public void Stop()
    {
        if (_process is { HasExited: false })
        {
            _process.Kill(true);
            _process.WaitForExit();
        }

        _process?.Dispose();
        _process = null;
        _currentExecutablePath = null;
    }

    public void Dispose()
    {
        Stop();
    }
}
