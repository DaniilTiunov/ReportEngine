using System.Text.Json.Serialization;

namespace ReportEngine.Shared.Config.JsonHelpers;

public class UpdateInfo
{
    public string Version { get; set; }
    public string Date { get; set; }
    public ReleaseChannel Channel { get; set; }
    public UpdateSections Sections { get; set; } = new();
}

public class UpdateSections
{
    public List<string> Added { get; set; } = new();
    public List<string> Changed { get; set; } = new();
    public List<string> Fixed { get; set; } = new();
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReleaseChannel
{
    Preview = 0,
    Beta = 1,
    Rc = 2,
    Stable = 3
}
