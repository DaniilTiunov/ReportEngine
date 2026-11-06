using System.Text.Json.Serialization;

namespace ReportEngine.Contracts.Messages;

public sealed class KiparisError
{
    [JsonPropertyName("code")]
    public required string Code { get; init; }

    [JsonPropertyName("message")]
    public required string Message { get; init; }
}