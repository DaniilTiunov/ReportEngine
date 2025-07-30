using ReportEngine.App.ViewModels;
using ReportEngine.App.Views;
using ReportEngine.App.Views.UpdateInformation;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;
using System.Diagnostics;
using System.Windows;
using AboutProgram = ReportEngine.App.Views.Windows.AboutProgram;


namespace ReportEngine.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window //Это так называемый "Code Behind" файл для MainWindow.xaml
    {
        public MainWindow(MainWindowViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = mainViewModel;

            mainViewModel.OnShowAllProjectsCommandExecuted(null);
            mainViewModel.OnChekDbConnectionCommandExecuted(null);
        }       
        //Здесь реализованый методы, которые не требуют много времени на выполнение
        
        private void CheckForUpdates(object sender, RoutedEventArgs e)
        {
            try
            {
                string appSettingsConfigPath = DirectoryHelper.GetConfigPath(); //Тянется жысон

                Updater.CheckForUpdate(JsonHandler.GetVersionOnServer(appSettingsConfigPath), JsonHandler.GetLocalVersion(appSettingsConfigPath));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ShowAboutProgram(object sender, RoutedEventArgs e) //Просто простые синхронные операции
        {
            var aboutWindow = new AboutProgram();

            aboutWindow.Show();
        }
        private void OpenSettingsWindow(object sender, RoutedEventArgs e) //Просто простые синхронные операции
        {
            var settingsWindow = new SettingsWindow();

            settingsWindow.Show();
        }
        private void ShowCalculator(object sender, RoutedEventArgs e)
        {
            Process.Start("calc.exe");
        }
        private void ShowNotepad(object sender, RoutedEventArgs e)
        {
            Process.Start("notepad.exe");
        }
    }
}