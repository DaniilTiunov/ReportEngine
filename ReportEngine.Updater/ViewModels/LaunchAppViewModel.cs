using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportEngine.Updater.Models;
using ReportEngine.Updater.Services;
using ReportEngine.Updater.ViewModels.Base;

namespace ReportEngine.Updater.ViewModels;

public partial class LaunchAppViewModel : BaseViewModel
{
    private readonly DirectoryService _directoryService;
    private readonly UpdateService _updateService;
    private readonly NotificationService _notificationService;
    
    [ObservableProperty]
    private ObservableCollection<Release> _localReleases = new();
    
    [ObservableProperty]
    private Release _selectedRelease;
    
    public LaunchAppViewModel(
        DirectoryService directoryService,
        UpdateService updateService,
        NotificationService notificationService
        )
    {
        _directoryService = directoryService;
        _updateService = updateService;
        _notificationService = notificationService;
        
        _ = LoadReleasesAsync();

        RefreshCommand = new AsyncRelayCommand(LoadReleasesAsync);
        LaunchCommand = new RelayCommand(LaunchApplicationAsync);
        CreateShortcutCommand = new RelayCommand(CreateShortcut);
    }
    
    public ICommand RefreshCommand { get; set; }
    public ICommand LaunchCommand { get; set; }
    public ICommand CreateShortcutCommand { get; set; }
    
    private async Task LoadReleasesAsync()
    {
        LocalReleases.Clear();
        
        var localDirectories = await _directoryService.GetDirectoriesAsync(
            paths => paths.LocalPath);

        foreach (var directory in localDirectories)
        {
            var updateInfo = await _updateService.GetUpdaterInfoAsync(directory);

            if (updateInfo == null)
                continue;

            var release = new Release
            {
                Info = updateInfo,
                Path = directory
            };
            
            LocalReleases.Add(release);
        }
    }

    private void LaunchApplicationAsync()
    {
        var exePath = Path.Combine(SelectedRelease.Path, "ReportEngine.App.exe");

        Process.Start(exePath);
    }
    
    private void CreateShortcut()
    {
        if (SelectedRelease == null)
        {
            _notificationService.ShowInfo("Сначала выберите приложение из списка.");
            return;
        }
        
        string desktopPath  = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string shortcutName = $"Стенды КИПиА v{SelectedRelease.Info.Version} ({SelectedRelease.Info.Channel}).lnk";
        string shortcutPath = Path.Combine(desktopPath, shortcutName);
        string targetPath = Path.Combine(SelectedRelease.Path, "ReportEngine.App.exe");
            
        Type t = Type.GetTypeFromProgID("WScript.Shell");
        dynamic shell = Activator.CreateInstance(t);
        
        dynamic shortcut = shell.CreateShortcut(shortcutPath);
        shortcut.TargetPath = targetPath;
        shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);

        shortcut.Save();
    }
}