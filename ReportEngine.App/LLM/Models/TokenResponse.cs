using System.Text.Json.Serialization;

namespace ReportEngine.App.LLM.Models;

public class TokenResponse
{
    [JsonPropertyName("access_token")] public string AccessToken { get; set; }

    [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
}
