namespace ReportEngine.Updater.Config;

public class UpdateSettings
{
    public UpdatePaths UpdatePaths { get; set; } = new();
}

public class UpdatePaths
{
    public string RemotePath { get; set; } = string.Empty;
    public string LocalPath { get; set; } = string.Empty;
}