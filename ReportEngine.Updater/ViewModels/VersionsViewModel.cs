using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportEngine.Shared.Config.JsonHelpers;
using ReportEngine.Updater.Helpers;
using ReportEngine.Updater.Models;
using ReportEngine.Updater.Services;
using ReportEngine.Updater.ViewModels.Base;

namespace ReportEngine.Updater.ViewModels;

public partial class VersionsViewModel : BaseViewModel
{
    private readonly DirectoryService _directoryService;
    private readonly NotificationService _notificationService;
    private readonly UpdateSettingsService _updateSettingsService;

    [ObservableProperty] 
    private ObservableCollection<Release> _releases = new();

    [ObservableProperty] 
    private Release? _selectedRelease = new();

    public VersionsViewModel(
        UpdateSettingsService updateSettingsService,
        DirectoryService directoryService,
        NotificationService notificationService)
    {
        _updateSettingsService = updateSettingsService;
        _directoryService = directoryService;
        _notificationService = notificationService;

        _ = LoadReleasesAsync();

        RefreshCommand = new AsyncRelayCommand(Refresh);
        InstallCommand = new AsyncRelayCommand(InstallSelectedReleaseAsync);
    }

    public ICommand RefreshCommand { get; set; }
    public ICommand InstallCommand { get; set; }

    private async Task Refresh()
    {
        Releases.Clear();

        await LoadReleasesAsync();
        
        _notificationService.ShowInfo("Версии загружены");
    }

    private async Task InstallSelectedReleaseAsync()
    {
        var distPath = await _updateSettingsService.GetPath(
            UpdateSettingsHelper.GetUpdateSettingsPath(),
            path => path.LocalPath);

        _directoryService.Copy(SelectedRelease.Path, distPath);

        var result = _notificationService.ShowConfirmation("""
                                                           Установка прошла успешно
                                                           Открыть директорию с версиями?
                                                           """);

        if (result)
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = distPath,
                UseShellExecute = true
            });
    }

    private async Task LoadReleasesAsync()
    {
        var releasesDirectories = await GetDirectoriesAsync();

        foreach (var releasesDirectory in releasesDirectories)
        {
            var updateInfo = await GetUpdaterInfoAsync(releasesDirectory);

            if (updateInfo == null)
                continue;

            var release = new Release
            {
                Info = updateInfo,
                Path = releasesDirectory
            };

            Releases.Add(release);
        }
    }

    private async Task<IEnumerable<string>> GetDirectoriesAsync()
    {
        var serverVersionsPath = await _updateSettingsService.GetPath(
            UpdateSettingsHelper.GetUpdateSettingsPath(),
            path => path.RemotePath);

        var ReleasesDirectories = Directory
            .EnumerateDirectories(
                serverVersionsPath,
                "*",
                SearchOption.TopDirectoryOnly);

        return ReleasesDirectories;
    }

    private async Task<UpdateInfo?> GetUpdaterInfoAsync(string releaseDirectory)
    {
        var path = Path.Combine(
            releaseDirectory,
            "Config",
            "updateInfo.json");

        if (!File.Exists(path))
            return null;

        var json = await File.ReadAllTextAsync(path);

        var updates = JsonSerializer.Deserialize<List<UpdateInfo>>(json);

        return updates?.FirstOrDefault();
    }
}