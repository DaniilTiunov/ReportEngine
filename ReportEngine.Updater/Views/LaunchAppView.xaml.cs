using System.Windows.Controls;
using ReportEngine.Updater.ViewModels;

namespace ReportEngine.Updater.Views;

public partial class LaunchAppView : UserControl
{
    public LaunchAppView(LaunchAppViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}