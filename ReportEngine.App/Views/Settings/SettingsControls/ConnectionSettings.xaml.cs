using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Settings.SettingsControls
{
    /// <summary>
    /// Логика взаимодействия для ConnectionSettings.xaml
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
}
