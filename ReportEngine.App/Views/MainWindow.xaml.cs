using System.Diagnostics;
using System.Windows;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.ViewModels;
using ReportEngine.App.Views.UpdateInformation;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;
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
        StandartTheme(null, null);
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
        ChangesTheme("/Resources/Dictionaries/DarkTheme.xaml");
    }

    private void StandartTheme(object sender, RoutedEventArgs e)
    {
        ChangesTheme("/Resources/Dictionaries/GigaChadUI.xaml");
    }

    private void TestTheme(object sender, RoutedEventArgs e)
    {
        ChangesTheme("/Resources/Dictionaries/TestTheme.xaml");
    }

    private void ChangesTheme(string dictPath)
    {
        var uri = new Uri(dictPath, UriKind.RelativeOrAbsolute);
        var resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
        Application.Current.Resources.Clear();
        Application.Current.Resources.MergedDictionaries.Add(resourceDict);
    }
}