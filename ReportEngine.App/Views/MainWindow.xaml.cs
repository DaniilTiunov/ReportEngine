using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
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

        MainWindow_StartUpState();

        //await _mainViewModel.ShowAllProjectsAsync();
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

    private void MainWindow_StartUpState()
    {
        var area = SystemParameters.WorkArea;
        Left = area.Left;
        Top = area.Top;
        Width = area.Width;
        Height = area.Height;
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
            MaxRestoreButton_Click(sender, e);
        else
            DragMove();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaxRestoreButton_Click(object sender, RoutedEventArgs e)
    {
        var area = SystemParameters.WorkArea;
        if (Width != area.Width || Height != area.Height || Left != area.Left || Top != area.Top)
        {
            Left = area.Left;
            Top = area.Top;
            Width = area.Width;
            Height = area.Height;
        }
        else
        {
            Width = 1280;
            Height = 800;
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}