using System.Windows;
using System.Windows.Input;
using ReportEngine.Updater.ViewModels;

namespace ReportEngine.Updater.Views;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }

    private void OnClickClose(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OnClickDrag(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}