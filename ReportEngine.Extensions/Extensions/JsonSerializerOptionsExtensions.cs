using System.Text.Json;

namespace ReportEngine.Extensions.Extensions;

public static class JsonSerializerOptionsExtensions
{
    public static JsonSerializerOptions SetOptions(this JsonSerializerOptions options)
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        };
    }
}