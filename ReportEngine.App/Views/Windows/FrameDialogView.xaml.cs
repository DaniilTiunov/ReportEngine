using System.Windows;
using System.Windows.Input;
using ReportEngine.App.ViewModels.FormedEquips;

namespace ReportEngine.App.Views.Windows;

public partial class FrameDialogView : Window
{
    public FrameDialogView(FormedFrameViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void FormedFrameGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is FormedFrameViewModel vm && vm.FormedFrameModel.SelectedFrame != null)
        {
            vm.SelectedItem?.Invoke(vm.FormedFrameModel.SelectedFrame);
            DialogResult = true;
            Close();
        }
    }

    private void Window_MouseLeftButtonDown(object sender, RoutedEventArgs e)
    {
        DragMove();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}