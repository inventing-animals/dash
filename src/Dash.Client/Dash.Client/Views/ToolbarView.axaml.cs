using Avalonia.Controls;
using Avalonia.Interactivity;
using Dash.Client.ViewModels;

namespace Dash.Client.Views;

public partial class ToolbarView : UserControl
{
    public ToolbarView()
    {
        InitializeComponent();
    }

    private void OnDashboard(object? sender, RoutedEventArgs e)
    {
        (DataContext as ToolbarViewModel)?.GoDashboard();
    }

    private void OnSettings(object? sender, RoutedEventArgs e)
    {
        (DataContext as ToolbarViewModel)?.GoSettings();
    }
}