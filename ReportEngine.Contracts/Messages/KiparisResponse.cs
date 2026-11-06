using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReportEngine.Contracts.Messages;

public class KiparisResponse
{
    [JsonPropertyName("protocolVersion")]
    public required int ProtocolVersion { get; init; }

    [JsonPropertyName("id")]
    public required Guid Id { get; init; }

    [JsonPropertyName("success")]
    public required bool Success { get; init; }

    [JsonPropertyName("data")]
    public JsonElement? Data { get; init; }

    [JsonPropertyName("error")]
    public KiparisError? Error { get; init; }
}