namespace ReportEngine.Shared.Config.JsonHelpers;

public class AppSettings
{
    public ConnectionStrings ConnectionStrings { get; set; }
    public AtomicConnectionString AtomicConnectionString { get; set; }
    public PathSettings PathSettings { get; set; }
    public DatabaseSettings DatabaseSettings { get; set; }
}
