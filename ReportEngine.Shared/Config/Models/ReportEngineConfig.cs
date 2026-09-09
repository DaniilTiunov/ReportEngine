namespace ReportEngine.Shared.Config.Models;

public class ReportEngineConfig
{
    public PathSettings PathSettings { get; set; } = new();
    public ConnectionStrings ConnectionStrings { get; set; } = new();
    public DatabaseSettings DatabaseSettings { get; set; } = new();
}