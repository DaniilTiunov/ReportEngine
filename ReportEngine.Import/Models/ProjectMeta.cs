using System.Text.Json.Serialization;

namespace ReportEngine.Import.Models;

public class ProjectMeta
{
    [JsonPropertyName("formatVersion")]
    public int FormatVersion { get; set; }

    [JsonPropertyName("sourceDb")]
    public string SourceDb { get; set; } = string.Empty;

    [JsonPropertyName("exportedAt")]
    public DateTime ExportedAt { get; set; }

    [JsonPropertyName("projectBusinessId")]
    public int ProjectBusinessId { get; set; }

    [JsonPropertyName("stands")]
    public int Stands { get; set; }

    [JsonPropertyName("note")]
    public string Note { get; set; } = string.Empty;
}