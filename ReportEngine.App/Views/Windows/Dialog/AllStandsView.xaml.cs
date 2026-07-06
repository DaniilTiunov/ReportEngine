using System.Windows;
using System.Windows.Input;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Windows.Dialog;

/// <summary>
///     Логика взаимодействия для AllStandsView.xaml
/// </summary>
public partial class AllStandsView : Window
{
    private readonly AllStandsViewModel _allStandsViewModel;

    public AllStandsView(AllStandsViewModel allStandsViewModel)
    {
        InitializeComponent();
        DataContext = allStandsViewModel;
        _allStandsViewModel = allStandsViewModel;
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
            MaxRestoreButton_Click(sender, e);
        else
            DragMove();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaxRestoreButton_Click(object sender, RoutedEventArgs e)
    {
        var area = SystemParameters.WorkArea;
        if (Width != area.Width || Height != area.Height || Left != area.Left || Top != area.Top)
        {
            Left = area.Left;
            Top = area.Top;
            Width = area.Width;
            Height = area.Height;
        }
        else
        {
            Width = 1280;
            Height = 800;
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await _allStandsViewModel.GetAllProjectsAsync();
    }

    private void StandsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is AllStandsViewModel vm && vm.SelectedStand != null)
        {
            vm.ConfirmSelection();

            DialogResult = true;
        }
    }
}
