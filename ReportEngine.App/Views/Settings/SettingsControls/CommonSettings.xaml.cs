using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Microsoft.WindowsAPICodePack.Dialogs;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Settings.SettingsControls
{
    /// <summary>
    /// Логика взаимодействия для CommonSettings.xaml
    /// </summary>
    public partial class CommonSettings : UserControl
    {
        private readonly SettingsViewModel _viewModel;

        public CommonSettings(SettingsViewModel settingsViewModel)
        {
            InitializeComponent();
            _viewModel = settingsViewModel;
            DataContext = _viewModel;
        }

        public void GetNewDirectory(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            var owner = new WindowInteropHelper(window).Handle;

            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                AddToMostRecentlyUsedList = true,
                InitialDirectory = _viewModel.SaveReportDirPath,
                Title = "Выберите папку для сохранения",
            };

            _viewModel.SaveReportDirPath = dialog.ShowDialog(owner) == CommonFileDialogResult.Ok
                ? dialog.FileName
                : _viewModel.SaveReportDirPath;
        }
    }
}
