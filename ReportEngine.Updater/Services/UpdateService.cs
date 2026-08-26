using System.IO;
using System.Text.Json;
using ReportEngine.Shared.Config.JsonHelpers;

namespace ReportEngine.Updater.Services;

public class UpdateService
{
    private readonly JsonSettingsService _jsonSettingsService;
    
    public UpdateService(JsonSettingsService jsonSettingsService)
    {
        _jsonSettingsService = jsonSettingsService;
    }
    
    public async Task<UpdateInfo?> GetUpdaterInfoAsync(string releaseDirectory)
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