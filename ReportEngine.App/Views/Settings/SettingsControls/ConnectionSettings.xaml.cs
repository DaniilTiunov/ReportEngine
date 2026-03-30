using System.Windows;
using System.Windows.Controls;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Settings.SettingsControls;

/// <summary>
///     Логика взаимодействия для ConnectionSettings.xaml
/// </summary>
public partial class ConnectionSettings : UserControl
{
    private readonly SettingsViewModel _viewModel;

    public ConnectionSettings(SettingsViewModel settingsViewModel)
    {
        InitializeComponent();
        _viewModel = settingsViewModel;
        DataContext = _viewModel;
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is SettingsViewModel vm)
            vm.DbPassword = ((PasswordBox)sender).Password;
    }
}
