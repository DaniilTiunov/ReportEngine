using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Services.Notification;
using ReportEngine.App.ViewModels;
using ReportEngine.App.Views.Controls;
using ReportEngine.App.Views.Windows;
using ReportEngine.Domain.Entities;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;
using AboutProgram = ReportEngine.App.Views.Windows.AboutProgram;

namespace ReportEngine.App;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window //Это так называемый "Code Behind" файл для MainWindow.xaml
{
    private readonly ExceptionService _exceptionService;
    private readonly MainWindowViewModel _mainViewModel;
    private readonly IServiceProvider _serviceProvider;
    private ICollectionView _projectsView;

    public MainWindow(
        MainWindowViewModel mainViewModel,
        IServiceProvider serviceProvider,
        ExceptionService exceptionService)
    {
        InitializeComponent();
        DataContext = mainViewModel;
        _mainViewModel = mainViewModel;
        _serviceProvider = serviceProvider;
        _exceptionService = exceptionService;

        SetWindowTitle();
        
        Loaded += MainWindow_Loaded;
        StateChanged += MainWindow_StateChanges;
    }

    private void SetWindowTitle()
    {
        try
        {
            var filePath = DirectoryHelper.GetUpdateInfoPath();
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter() }
                };
                var updates = JsonSerializer.Deserialize<List<UpdateInfo>>(json, options);
                var update = updates?.FirstOrDefault();
                
                if (update != null)
                {
                    Title = $"Стенды КИПиА v{update.Version} ({update.Channel})";
                    return;
                }
            }
        }
        catch { }
        
        Title = "Стенды КИПиА";
    }
    
    // Событие загрузки окна
    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            StandardTheme(null, null);

            MainWindow_StartUpState();

            if (StartUp.CanConnect) await _mainViewModel.ShowAllProjectsAsync();

            _projectsView = CollectionViewSource.GetDefaultView(
                _mainViewModel.MainWindowModel.AllProjects);

            MainDataGrid.ItemsSource = _projectsView;
        });
    }

    private async void MainDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        await _mainViewModel.OnEditProjectCommandExecuted();
    }

    // Событие изменения состояния окна
    private void MainWindow_StateChanges(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Maximized)
            WindowState = WindowState.Normal;
    }

    private void ShowAboutProgram(object sender, RoutedEventArgs e) //Просто простые синхронные операции
    {
        var aboutWindow = new AboutProgram(_exceptionService);
        aboutWindow.Show();
    }

    private void ShowUpdateIndo(object sender, RoutedEventArgs e)
    {
        var updateInfo = new UpdateInfoView(_exceptionService);
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

    private void StandardTheme(object sender, RoutedEventArgs e)
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
        for (var i = 0; i < mergedDicts.Count; i++)
            if (mergedDicts[i].Source != null && mergedDicts[i].Source.OriginalString.Contains("ColorThemes"))
            {
                mergedDicts[i] = themeDict;
                return;
            }

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
                    var companyMatch = !string.IsNullOrEmpty(prj.Company) && prj.Company.ToLower().Contains(query);
                    var objectMatch = !string.IsNullOrEmpty(prj.Object) && prj.Object.ToLower().Contains(query);
                    return companyMatch || objectMatch;
                }

                return false;
            };

        _projectsView.Refresh();
    }
    
    private void OpenLogger_Click(object sender, RoutedEventArgs e)
    {
        if (LogContainer.Visibility == Visibility.Collapsed)
        {
            LogContainer.Visibility = Visibility.Visible;

            if (LogHost.Content == null)
            {
                var logView = _serviceProvider.GetRequiredService<AppLogsView>();
                LogHost.Content = logView;
            }
        }
        else
        {
            LogContainer.Visibility = Visibility.Collapsed;
        }
    }
}
