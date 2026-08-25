using System.Windows.Controls.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;
using ReportEngine.Updater.Config;
using ReportEngine.Updater.Helpers;
using ReportEngine.Updater.Services;
using ReportEngine.Updater.ViewModels.Base;

namespace ReportEngine.Updater.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly UpdateSettingsService _updateSettingsService;
    
    [ObservableProperty]
    private string _localPath;
    
    [ObservableProperty]
    private string _remotePath;

    public SettingsViewModel(UpdateSettingsService updateSettingsService)
    {
        _updateSettingsService = updateSettingsService;

        _ = LoadSettingsAsync();
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
}