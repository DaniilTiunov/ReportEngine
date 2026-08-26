using System.IO;
using System.Text.Json;
using System.Windows;
using ReportEngine.Shared.Config.JsonHelpers;
using ReportEngine.Updater.Config;

namespace ReportEngine.Updater.Services;

public class JsonSettingsService
{
    private readonly JsonSerializerOptions _jsonOptions;
    
    public JsonSettingsService(JsonSerializerOptions jsonOptions)
    {
        _jsonOptions = jsonOptions;
    }
    
    private async Task<UpdateSettings> GetUpdateSettingsJsonAsync(string jsonConfigPath)
    {
        var json = await File.ReadAllTextAsync(jsonConfigPath);

        return JsonSerializer.Deserialize<UpdateSettings>(json)
               ?? new UpdateSettings();
    }

    private async Task SetUpdateSettingsJsonAsync(
        string jsonConfigPath,
        UpdateSettings updateSettings)
    {
        var json = JsonSerializer.Serialize(
            updateSettings,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        await File.WriteAllTextAsync(jsonConfigPath, json);
    }

    public async Task<string> GetPathAsync(
        string jsonConfigPath,
        Func<UpdatePaths, string> selector)
    {
        var settings = await GetUpdateSettingsJsonAsync(jsonConfigPath);

        return selector(settings.UpdatePaths) ?? string.Empty;
    }

    public async Task<UpdateInfo> GetLatestReleaseInfoAsync(string releaseDirectory, string jsonName)
    {
        var path = Path.Combine(releaseDirectory, jsonName);
        
        var json = await File.ReadAllTextAsync(path);
        
        if (string.IsNullOrEmpty(json))
        {
            MessageBox.Show("Файл пуст");
            return new UpdateInfo();
        }
        
        return JsonSerializer.Deserialize<UpdateInfo>(json, _jsonOptions) ?? 
               new UpdateInfo();
    }

    public async Task SetRemotePathAsync(string jsonConfigPath, string newRemotePath)
    {
        var settings = await GetUpdateSettingsJsonAsync(jsonConfigPath);
        settings.UpdatePaths.RemotePath = newRemotePath;
        await SetUpdateSettingsJsonAsync(jsonConfigPath, settings);
    }

    public async Task SetLocalPathAsync(string jsonConfigPath, string newLocalPath)
    {
        var settings = await GetUpdateSettingsJsonAsync(jsonConfigPath);
        settings.UpdatePaths.LocalPath = newLocalPath;
        await SetUpdateSettingsJsonAsync(jsonConfigPath, settings);
    }
}