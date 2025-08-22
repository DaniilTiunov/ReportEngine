using System.Diagnostics;
using System.Windows;
using ReportEngine.App.ViewModels;
using ReportEngine.App.Views;
using ReportEngine.App.Views.UpdateInformation;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;
using ReportEngine.Shared.Helpers;
using AboutProgram = ReportEngine.App.Views.Windows.AboutProgram;


namespace ReportEngine.App;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window //Это так называемый "Code Behind" файл для MainWindow.xaml
{
    private readonly MainWindowViewModel _mainViewModel;

    public MainWindow(MainWindowViewModel mainViewModel)
    {
        InitializeComponent();
        DataContext = mainViewModel;
        _mainViewModel = mainViewModel;

        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _mainViewModel.ShowAllProjectsAsync();
        await _mainViewModel.CheckDbConnectionAsync();
    }

    private void CheckForUpdates(object sender, RoutedEventArgs e)
    {
        ExceptionHelper.SafeExecute(() =>
        {
            var appSettingsConfigPath = DirectoryHelper.GetConfigPath();
            Updater.CheckForUpdate(JsonHandler.GetVersionOnServer(appSettingsConfigPath),
                JsonHandler.GetLocalVersion(appSettingsConfigPath));
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
    }

    private void ChangeLightTheme(object sender, RoutedEventArgs e)
    {
    }

    private void StandartTheme(object sender, RoutedEventArgs e)
    {
    }

    private void TestTheme(object sender, RoutedEventArgs e)
    {
    }
}