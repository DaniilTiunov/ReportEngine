using System.IO;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using ReportEngine.Updater.Config;
using ReportEngine.Updater.Helpers;
using ReportEngine.Updater.Services;
using ReportEngine.Updater.ViewModels.Base;

namespace ReportEngine.Updater.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly UpdateSettingsService _updateSettingsService;
    private readonly NotificationService _notificationService;
    
    [ObservableProperty]
    private string _localPath;
    
    [ObservableProperty]
    private string _remotePath;

    public SettingsViewModel(
        UpdateSettingsService updateSettingsService,
        NotificationService notificationService)
    {
        _updateSettingsService = updateSettingsService;
        _notificationService = notificationService;

        _ = LoadSettingsAsync();

        SaveSettingsCommand = new AsyncRelayCommand(SaveSettingsAsync);
        BrowseCommand = new RelayCommand<string>(BrowseFolder);
    }

    public ICommand SaveSettingsCommand { get; set; }

    public ICommand BrowseCommand { get; set; }
    
    private void BrowseFolder(string type)
    {
        if (string.IsNullOrEmpty(type))
            return;
        
        var folderDialog = new OpenFolderDialog();

        if (folderDialog.ShowDialog() != true)
            return;

        switch (type)
        {
            case "Local":
                LocalPath = folderDialog.FolderName;
                break;

            case "Remote":
                RemotePath = folderDialog.FolderName;
                break;
        }
    }
    
    private async Task LoadSettingsAsync()
    {
        var settingsPath = UpdateSettingsHelper.GetUpdateSettingsPath();
        LocalPath = await _updateSettingsService.GetPath(
            jsonConfigPath: settingsPath,
            selector: json => json.LocalPath);

        RemotePath = await _updateSettingsService.GetPath(
            jsonConfigPath: settingsPath,
            selector: json => json.RemotePath);
    }

    private async Task SaveSettingsAsync()
    {
        var newLocalPath = LocalPath;
        var newRemotePath = RemotePath;
        
        await _updateSettingsService.SetLocalPath(
            UpdateSettingsHelper.GetUpdateSettingsPath(),
            newLocalPath);
        
        await _updateSettingsService.SetRemotePath(
            UpdateSettingsHelper.GetUpdateSettingsPath(),
            newRemotePath);
        
        _notificationService.ShowInfo("Настройки успешно сохранены");
    }
}