using System.Text.Json.Serialization;

namespace ReportEngine.Import.Models;

public class MaterialJsonModel
{
    /// <summary>
    /// Сквозной номер материала в спецификации стенда.
    /// </summary>
    [JsonPropertyName("Позиция")]
    public int Position { get; set; }

    /// <summary>
    /// Часть стенда: рама, дренаж, обвязка, электрика,
    /// дополнительные комплектующие, тара.
    /// </summary>
    [JsonPropertyName("Узел")]
    public string Node { get; set; } = string.Empty;

    /// <summary>
    /// Номер строки внутри узла из исходной таблицы.
    /// Может быть null.
    /// </summary>
    [JsonPropertyName("ПозицияУзла")]
    public int? NodePosition { get; set; }

    /// <summary>
    /// Уточнение внутри обвязки (линия, арматура, тройник, КМЧ, хомут, доп. N).
    /// Может быть пустой строкой.
    /// </summary>
    [JsonPropertyName("Элемент")]
    public string Element { get; set; } = string.Empty;

    [JsonPropertyName("Материал_id")]
    public int MaterialId { get; set; }

    [JsonPropertyName("Материал")]
    public string MaterialName { get; set; } = string.Empty;

    /// <summary>
    /// Количество. Может быть null (например, у арматуры и тары
    /// в старой базе количества нет).
    /// </summary>
    [JsonPropertyName("Кол-во")]
    public double? Quantity { get; set; }

    [JsonPropertyName("Ед")]
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// Цена. Может быть null, если не задана.
    /// </summary>
    [JsonPropertyName("Цена")]
    public decimal? Price { get; set; }

    /// <summary>
    /// Сумма. Может быть null (для позиций без количества/цены).
    /// </summary>
    [JsonPropertyName("Сумма")]
    public decimal? Sum { get; set; }
}

/// <summary>
/// Константы для значений "Узел" (для удобства).
/// </summary>
public static class NodeTypes
{
    public const string Frame = "рама";
    public const string Drainage = "дренаж";
    public const string Piping = "обвязка";
    public const string Electric = "электрика";
    public const string Additional = "дополнительные комплектующие";
    public const string Packaging = "тара";
}

/// <summary>
/// Константы для значений "Элемент".
/// </summary>
public static class ElementTypes
{
    public const string Line = "линия";
    public const string Valve = "арматура";
    public const string Tee = "тройник";
    public const string Fasteners = "КМЧ";
    public const string Clamp = "хомут";
}