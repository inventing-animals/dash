using System;
using System.Collections.Specialized;
using Avalonia.Controls;
using Dash.Client.ViewModels;

namespace Dash.Client.Views;

public partial class DashboardView : UserControl
{
    private DashboardViewModel? _viewModel;

    public DashboardView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (_viewModel is not null)
        {
            _viewModel.WidgetControls.CollectionChanged -= OnWidgetControlsChanged;
        }

        _viewModel = DataContext as DashboardViewModel;

        if (_viewModel is not null)
        {
            _viewModel.WidgetControls.CollectionChanged += OnWidgetControlsChanged;
        }

        SyncWidgetPanel();
    }

    private void OnWidgetControlsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SyncWidgetPanel();
    }

    private void SyncWidgetPanel()
    {
        WidgetPanel.Children.Clear();

        if (_viewModel is null)
        {
            return;
        }

        foreach (var control in _viewModel.WidgetControls)
        {
            WidgetPanel.Children.Add(control);
        }
    }
}
