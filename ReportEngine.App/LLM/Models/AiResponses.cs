using System.Text.Json.Serialization;

namespace ReportEngine.App.LLM.Models;

public class AiResponse
{
    [JsonPropertyName("choices")] public List<ChoiceDto> Choices { get; set; }
}

public class ChoiceDto
{
    [JsonPropertyName("message")] public MessageDto Message { get; set; }
}

public class MessageDto
{
    [JsonPropertyName("content")] public string Content { get; set; }
}
