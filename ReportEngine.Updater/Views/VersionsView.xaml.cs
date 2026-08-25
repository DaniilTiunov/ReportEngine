using System.Windows.Controls;
using ReportEngine.Updater.ViewModels;

namespace ReportEngine.Updater.Views;

public partial class VersionsView : UserControl
{
    public VersionsView(VersionsViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}