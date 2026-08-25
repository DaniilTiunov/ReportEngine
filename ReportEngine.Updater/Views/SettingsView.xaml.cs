using System.Windows.Controls;
using ReportEngine.Updater.ViewModels;

namespace ReportEngine.Updater.Views;

public partial class SettingsView : UserControl
{
    public SettingsView(SettingsViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}