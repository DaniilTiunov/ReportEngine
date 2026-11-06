using System.Text.Json.Serialization;

namespace ReportEngine.Contracts.Models.Project;

public class AppStateResult
{
    [JsonPropertyName("isReady")]
    public required bool IsReady { get; init; }

    [JsonPropertyName("currentProjectId")]
    public int? CurrentProjectId { get; init; }
    
    [JsonPropertyName("standsCount")]
    public int? StandsCount { get; init; }
}