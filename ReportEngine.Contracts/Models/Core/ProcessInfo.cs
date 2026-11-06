using System.Text.Json.Serialization;

namespace ReportEngine.Contracts.Models.Core;

public class ProcessInfo
{
    [JsonPropertyName("processName")]
    public string? ProcessName { get; init; }

    [JsonPropertyName("currentProcessId")]
    public int? CurrentProcessId { get; init; }
}