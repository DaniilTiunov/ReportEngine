using System.ComponentModel.DataAnnotations;
using ReportEngine.Domain.Entities.BaseEntities.Interface;

namespace ReportEngine.Domain.Entities;

public class DrainagePurpose : IPurposeEntity
{
    public string? Purpose { get; set; } // Например, "Клапан", "Труба"
    public string? Material { get; set; } // Просто текстовое поле для материала
    public float? Quantity { get; set; } // Количество
    public float? CostPerUnit { get; set; }
    public string? Measure { get; set; }

    // Внешний ключ на дренаж
    public int FormedDrainageId { get; set; }
    public virtual FormedDrainage FormedDrainage { get; set; }
    [Key] public int Id { get; set; }
}