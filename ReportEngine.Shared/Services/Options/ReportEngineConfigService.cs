using System.Text.Json;
using Microsoft.Extensions.Options;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.Models;

namespace ReportEngine.Shared.Services.Options;

public class ReportEngineConfigService
{
    private readonly string _configFilePath;
    private readonly IOptions<ReportEngineConfig> _options;
    private readonly IOptionsMonitor<ReportEngineConfig> _optionsMonitor;

    public ReportEngineConfigService(
        IOptions<ReportEngineConfig> options,
        IOptionsMonitor<ReportEngineConfig> optionsMonitor)
    {
        _options = options;
        _optionsMonitor = optionsMonitor;
        _configFilePath = DirectoryHelper.GetConfigPath();
    }

    public string GetConnectionString()
    {
        return _options.Value.ConnectionStrings.DefaultConnection;
    }

    public string GetSqlLiteConnectionString()
    {
        return _options.Value.ConnectionStrings.SqliteConnectionString;
    }

    public string GetDatabaseMode()
    {
        return _options.Value.DatabaseSettings.DatabaseMode;
    }

    public string GetSaveReportDirectory()
    {
        return _options.Value.PathSettings.SaveReportDirectory;
    }

    public ReportEngineConfig GetSettings()
    {
        return _options.Value;
    }

    public void SetDatabaseMode(string newDatabaseMode)
    {
        UpdateConfig(settings => settings.DatabaseSettings.DatabaseMode = newDatabaseMode);
    }

    public void SetConnectionString(string newConnectionString)
    {
        UpdateConfig(settings => settings.ConnectionStrings.DefaultConnection = newConnectionString);
    }

    public void SetSaveReportDirectory(string newSaveReportDirectory)
    {
        UpdateConfig(settings => settings.PathSettings.SaveReportDirectory = newSaveReportDirectory);
    }

    private void UpdateConfig(Action<ReportEngineConfig> updateAction)
    {
        var json = File.ReadAllText(_configFilePath);
        var settings = JsonSerializer.Deserialize<ReportEngineConfig>(json) ?? new ReportEngineConfig();

        updateAction(settings);

        var newJson = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_configFilePath, newJson);
    }
}