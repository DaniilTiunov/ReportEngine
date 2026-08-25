using System.IO;
using System.Text.Json;
using ReportEngine.Updater.Config;

namespace ReportEngine.Updater.Services;

public class UpdateSettingsService
{
    private async Task<UpdateSettings> GetUpdateSettingsJson(string jsonConfigPath)
    {
        var json = await File.ReadAllTextAsync(jsonConfigPath);

        return JsonSerializer.Deserialize<UpdateSettings>(json)
               ?? new UpdateSettings();
    }

    private async Task SetUpdateSettingsJson(
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

    public async Task<string> GetPath(
        string jsonConfigPath,
        Func<UpdatePaths, string> selector)
    {
        var settings = await GetUpdateSettingsJson(jsonConfigPath);

        return selector(settings.UpdatePaths) ?? string.Empty;
    }

    public async Task SetRemotePath(string jsonConfigPath, string newRemotePath)
    {
        var settings = await GetUpdateSettingsJson(jsonConfigPath);
        settings.UpdatePaths.RemotePath = newRemotePath;
        await SetUpdateSettingsJson(jsonConfigPath, settings);
    }

    public async Task SetLocalPath(string jsonConfigPath, string newLocalPath)
    {
        var settings = await GetUpdateSettingsJson(jsonConfigPath);
        settings.UpdatePaths.LocalPath = newLocalPath;
        await SetUpdateSettingsJson(jsonConfigPath, settings);
    }
}