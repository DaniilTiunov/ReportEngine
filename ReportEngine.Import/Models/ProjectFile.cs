using System.Text.Json.Serialization;

namespace ReportEngine.Import.Models;

public class ProjectFile
{
    [JsonPropertyName("meta")]
    public ProjectMeta Meta { get; set; } = new();

    [JsonPropertyName("Проект")]
    public ProjectInfoJsonModel Project { get; set; } = new();

    [JsonPropertyName("Стенды")]
    public List<StandJsonModel> Stands { get; set; } = new();
}