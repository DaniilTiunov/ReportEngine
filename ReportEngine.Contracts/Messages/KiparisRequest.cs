using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReportEngine.Contracts.Messages;

public class KiparisRequest
{
    [JsonPropertyName("protocolVersion")]
    public required int ProtocolVersion { get; init; }

    [JsonPropertyName("id")]
    public required Guid Id { get; init; }

    [JsonPropertyName("command")]
    public required string Command { get; init; }

    [JsonPropertyName("arguments")]
    public JsonElement? Arguments { get; init; }

}