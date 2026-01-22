using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.ViewModels;
using ReportEngine.App.ViewModels.CalculationSettings;
using ReportEngine.App.Views.Windows;
using ReportEngine.Domain.Entities;
using ReportEngine.Shared.Config.Directory;
using AboutProgram = ReportEngine.App.Views.Windows.AboutProgram;

namespace ReportEngine.App;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window //Это так называемый "Code Behind" файл для MainWindow.xaml
{
    private ICollectionView _projectsView;

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
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            StandartTheme(null, null);

            MainWindow_StartUpState();

            var canConnect = await _mainViewModel.CanAppConnect();

            if (canConnect) await _mainViewModel.ShowAllProjectsAsync();

            await _mainViewModel.CheckDbConnectionAsync();
            await LoadCalculationSettingsDataAsync();

            _projectsView = CollectionViewSource.GetDefaultView(_mainViewModel.MainWindowModel.AllProjects);
            MainDataGrid.ItemsSource = _projectsView;
        });
    }
    private void MainDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        _mainViewModel.OnEditProjectCommandExecuted(e);
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
    private void ShowUpdateIndo(object sender, RoutedEventArgs e)
    {
        var updateInfo = new UpdateInfoView();
        updateInfo.Show();
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
        ChangesTheme("/Resources/Dictionaries/ColorThemes/DarkTheme.xaml");
    }
    private void StandartTheme(object sender, RoutedEventArgs e)
    {
        ChangesTheme("/Resources/Dictionaries/ColorThemes/LightTheme.xaml");
    }
    private void MangoParadiseTheme(object sender, RoutedEventArgs e)
    {
        ChangesTheme("/Resources/Dictionaries/ColorThemes/MangoParadiseTheme.xaml");
    }
    private void BubbleGumTheme(object sender, RoutedEventArgs e)
    {
        ChangesTheme("/Resources/Dictionaries/ColorThemes/BubbleGumTheme.xaml");
    }
    private void ChangesTheme(string dictPath)
    {
        var uri = new Uri(dictPath, UriKind.Relative);
        var themeDict = Application.LoadComponent(uri) as ResourceDictionary;

        var mergedDicts = Application.Current.Resources.MergedDictionaries;
        for (int i = 0; i < mergedDicts.Count; i++)
        {
            if (mergedDicts[i].Source != null && mergedDicts[i].Source.OriginalString.Contains("ColorThemes"))
            {
                mergedDicts[i] = themeDict;
                return;
            }
        }

        // Если цветовая тема ещё не подключена
        mergedDicts.Add(themeDict);
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
    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_projectsView == null)
            return;

        var query = SearchTextBox.Text.Trim().ToLower();

        if (string.IsNullOrEmpty(query))
            _projectsView.Filter = null;
        else
            _projectsView.Filter = obj =>
            {
                if (obj is ProjectInfo prj)
                {
                    bool companyMatch = !string.IsNullOrEmpty(prj.Company) && prj.Company.ToLower().Contains(query);
                    bool objectMatch = !string.IsNullOrEmpty(prj.Object) && prj.Object.ToLower().Contains(query);
                    return companyMatch || objectMatch;
                }
                return false;
            };

        _projectsView.Refresh();
    }
}
