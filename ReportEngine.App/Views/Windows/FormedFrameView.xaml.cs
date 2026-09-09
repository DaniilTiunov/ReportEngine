using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using ReportEngine.App.ViewModels.FormedEquips;

namespace ReportEngine.App.Views.Windows;

/// <summary>
///     Логика взаимодействия для FormedFrameView.xaml
/// </summary>
public partial class FormedFrameView : MetroWindow
{
    private readonly FormedFrameViewModel _viewModel;
    private bool _allowEdit;

    public FormedFrameView(FormedFrameViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        Loaded += FormedFrameWindow_Loaded;
        StateChanged += FormedFrameWindow_StateChanges;
    }

    private void FormedFrameWindow_Loaded(object sender, RoutedEventArgs e)
    {
        FormedFrameWindow_StartUpState();
    }

    private void FormedFrameWindow_StateChanges(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Maximized)
            WindowState = WindowState.Normal;
    }

    private void FormedFrameWindow_StartUpState()
    {
        var area = SystemParameters.WorkArea;
        Left = area.Left;
        Top = area.Top;
        Width = area.Width;
        Height = area.Height;
    }

    private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
    {
        if (!_allowEdit)
            e.Cancel = true;
    }

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is not DataGrid grid)
            return;

        _allowEdit = true;

        if (grid.CurrentCell != null) grid.BeginEdit();

        _allowEdit = false;
    }
}