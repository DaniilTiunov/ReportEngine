using ReportEngine.App.ViewModels;
using ReportEngine.App.Views;
using ReportEngine.App.Views.UpdateInformation;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;
using ReportEngine.Shared.Helpers;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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

            StandartTheme(null, null);
            mainViewModel.OnShowAllProjectsCommandExecuted(null);
            mainViewModel.OnChekDbConnectionCommandExecuted(null);
        }

        private void CheckForUpdates(object sender, RoutedEventArgs e)
        {
            ExceptionHelper.SafeExecute(() =>
            {
                string appSettingsConfigPath = DirectoryHelper.GetConfigPath();
                Updater.CheckForUpdate(JsonHandler.GetVersionOnServer(appSettingsConfigPath), JsonHandler.GetLocalVersion(appSettingsConfigPath));
            });
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

        private void ChangeDarkTheme(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("/Resources/Dictionaries/DarkTheme.xaml", UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }
        private void ChangeLightTheme(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("/Resources/Dictionaries/LightTheme.xaml", UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }
        private void StandartTheme(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("/Resources/Dictionaries/GigaChadUI.xaml", UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }
        private void TestTheme(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("/Resources/Dictionaries/TestTheme.xaml", UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }
    }
}