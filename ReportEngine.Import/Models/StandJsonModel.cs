using System.Text.Json.Serialization;

namespace ReportEngine.Import.Models;

public class StandJsonModel
{
    [JsonPropertyName("Стенд_id")]
    public int StandId { get; set; }

    [JsonPropertyName("Номер")]
    public int Number { get; set; }

    [JsonPropertyName("Обозначение")]
    public string Designation { get; set; } = string.Empty;

    [JsonPropertyName("СерНомер")]
    public string SerialNumber { get; set; } = string.Empty;

    [JsonPropertyName("Материалов")]
    public int MaterialsCount { get; set; }

    [JsonPropertyName("Материалы")]
    public List<MaterialJsonModel> Materials { get; set; } = new();
}