using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.Shared.Config.JsonHelpers;
using ReportEngine.Updater.Helpers;
using ReportEngine.Updater.Services;
using ReportEngine.Updater.ViewModels.Base;
using ReportEngine.Updater.Views;
using RelayCommand = ReportEngine.Updater.Commands.RelayCommand;

namespace ReportEngine.Updater.ViewModels;

public partial class MainWindowViewModel : BaseViewModel
{
    private readonly DirectoryService _directoryService;
    private readonly JsonSettingsService _jsonSettingsService;
    private readonly NotificationService _notificationService;
    private readonly IServiceProvider _serviceProvider;
    private readonly UpdateService _updateService;

    [ObservableProperty] private object _currentView;

    [ObservableProperty] private UpdateInfo _latestInfo;

    public MainWindowViewModel(
        IServiceProvider serviceProvider,
        JsonSettingsService jsonSettingsService,
        UpdateService updateService,
        DirectoryService directoryService,
        NotificationService notificationService)
    {
        _serviceProvider = serviceProvider;
        _jsonSettingsService = jsonSettingsService;
        _updateService = updateService;
        _directoryService = directoryService;
        _notificationService = notificationService;

        _currentView = _serviceProvider.GetRequiredService<HomeView>();

        NavigateCommand = new RelayCommand(Navigate);
        CheckForUpdatesCommand = new AsyncRelayCommand(CheckForUpdatesAsync);

        _ = LoadLatestReleaseAsync();
    }

    public ICommand NavigateCommand { get; set; }
    public ICommand CheckForUpdatesCommand { get; set; }

    private void Navigate(object obj)
    {
        switch (obj)
        {
            case "Home":
                CurrentView = _serviceProvider.GetRequiredService<HomeView>();
                break;
            case "Versions":
                CurrentView = _serviceProvider.GetRequiredService<VersionsView>();
                break;
            case "Settings":
                CurrentView = _serviceProvider.GetRequiredService<SettingsView>();
                break;
        }
    }

    private async Task<UpdateInfo> LoadLatestReleaseAsync()
    {
        var updateInfoPath = await _jsonSettingsService.GetPathAsync(
            UpdateSettingsHelper.GetUpdateSettingsPath(),
            path => path.RemotePath);

        var latestInfo = await _jsonSettingsService.GetLatestReleaseInfoAsync(
            updateInfoPath,
            "latest.json");

        LatestInfo = latestInfo;

        return latestInfo;
    }

    private async Task CheckForUpdatesAsync()
    {
        var localDirectories = await _directoryService.GetDirectoriesAsync(paths => paths.LocalPath);

        var latestReleaseInfo = await LoadLatestReleaseAsync();

        foreach (var localDirectory in localDirectories)
        {
            var updateInfo = await _updateService.GetUpdaterInfoAsync(localDirectory);

            if (updateInfo == null)
                continue;
            
            if (IsUpdateAvailable(latestReleaseInfo, updateInfo))
            {
                _notificationService.ShowInfo($"""
                                               Доступна новая версия для загрузки: {latestReleaseInfo.Channel} {latestReleaseInfo.Version}
                                               """);
            }
            else
            {
                _notificationService.ShowInfo("Все актуальные версии установлены!");
            }
        }
    }
    
    private bool IsUpdateAvailable(UpdateInfo serverInfo, UpdateInfo localInfo)
    {
        if (serverInfo.Channel > localInfo.Channel)
            return true;
        
        if (serverInfo.Channel == localInfo.Channel)
        {
            var serverVersion = Version.Parse(serverInfo.Version);
            var localVersion = Version.Parse(localInfo.Version);
            return serverVersion > localVersion;
        }

        return false;
    }
}