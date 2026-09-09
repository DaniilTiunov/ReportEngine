using System.Windows.Input;
using MahApps.Metro.Controls;
using ReportEngine.App.ViewModels.FormedEquips;

namespace ReportEngine.App.Views.Windows;

public partial class FrameDialogView : MetroWindow
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
}