using System.Text.Json.Serialization;

namespace ReportEngine.Import.Models;

public class ProjectInfoJsonModel
{
    [JsonPropertyName("Проект_id")]
    public int ProjectId { get; set; }

    [JsonPropertyName("ОбозначениеКД")]
    public string? DesignationKD { get; set; }

    [JsonPropertyName("Клиент")]
    public string Client { get; set; } = string.Empty;

    [JsonPropertyName("Объект")]
    public string Object { get; set; } = string.Empty;

    [JsonPropertyName("Статус")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("ДатаЗапроса")]
    public DateTime? RequestDate { get; set; }

    [JsonPropertyName("ДатаОтгрузки")]
    public DateTime? ShipmentDate { get; set; }

    [JsonPropertyName("Стендов")]
    public int StandsCount { get; set; }
}