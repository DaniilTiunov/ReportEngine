using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.ViewModels;
using ReportEngine.App.ViewModels.CalculationSettings;
using ReportEngine.Shared.Config.Directory;
using AboutProgram = ReportEngine.App.Views.Windows.AboutProgram;

namespace ReportEngine.App;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window //Это так называемый "Code Behind" файл для MainWindow.xaml
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly IServiceProvider _serviceProvider;

    public MainWindow(
        MainWindowViewModel mainViewModel,
        IServiceProvider serviceProvider)
    {
        InitializeComponent();
        DataContext = mainViewModel;
        _mainViewModel = mainViewModel;
        _serviceProvider = serviceProvider;

        Loaded += MainWindow_Loaded;
        StateChanged += MindowWindow_StateChanges;
    }

    // Событие загрузки окна
    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        StandartTheme(null, null);

        MainWindow_StartUpState();

        var canConnect = await _mainViewModel.CanAppConnect();

        if (canConnect) await _mainViewModel.ShowAllProjectsAsync();

        await _mainViewModel.CheckDbConnectionAsync();
        await LoadCalculationSettingsDataAsync();
    }

    private async Task LoadCalculationSettingsDataAsync()
    {
        var calcSettings = _serviceProvider.GetRequiredService<CalculationSettingsViewModel>();
        await calcSettings.LoadSettingsAsync();
    }

    // Событие изменения состояния окна
    private void MindowWindow_StateChanges(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Maximized)
            WindowState = WindowState.Normal;
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

    private void AutoUpdate(object sender, RoutedEventArgs e)
    {
        try
        {
            var localPath = AppDomain.CurrentDomain.BaseDirectory;
            var updaterPath = Path.Combine(localPath, "ReportUpdater.exe");

            if (!File.Exists(updaterPath))
            {
                MessageBox.Show("ReportUpdater.exe не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Process.Start(updaterPath);

            // Завершаем текущий WPF
            Application.Current.Shutdown();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка запуска обновления: {ex.Message}");
        }
    }

    private void OpenHelp(object sender, RoutedEventArgs e)
    {
        var helpPath = Path.Combine(DirectoryHelper.GetDirectory(), "Help", "HelpDesk.chm");

        ExceptionHelper.SafeExecute(() =>
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = helpPath,
                UseShellExecute = true
            });
        });
    }
}