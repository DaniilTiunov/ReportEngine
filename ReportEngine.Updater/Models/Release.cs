using ReportEngine.Shared.Config.JsonHelpers;

namespace ReportEngine.Updater.Models;

public class Release
{
    public UpdateInfo Info { get; set; } = new();

    public string Path { get; set; } = string.Empty;
}