using ReportEngine.App.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace ReportEngine.App.Views.Windows;

/// <summary>
///     Логика взаимодействия для ObvyazkiView.xaml
/// </summary>
public partial class ObvyazkiView : Window
{
    public ObvyazkiView(ObvyazkaViewModel obvyazkiViewModel)
    {
        InitializeComponent();
        DataContext = obvyazkiViewModel;

        InitializeData(obvyazkiViewModel);
    }

    private void ObvyazkaListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is ObvyazkaViewModel vm && vm.CurrentObvyazka.SelectedObvyazka != null)
        {
            vm.SelectionHandler?.Invoke(vm.CurrentObvyazka.SelectedObvyazka);
            Close();
        }
    }

    private void InitializeData(ObvyazkaViewModel obvyazkiViewModel)
    {
        obvyazkiViewModel.ShowAllObvyazkiAsync();
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
            Width = 1400;
            Height = 700;
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}