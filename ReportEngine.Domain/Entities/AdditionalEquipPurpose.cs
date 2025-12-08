using ReportEngine.Domain.Entities.BaseEntities.Interface;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities;

public class AdditionalEquipPurpose : IPurposeEntity
{
    public string? Purpose { get; set; }
    public string? Material { get; set; }
    public float? Quantity { get; set; }
    public float? CostPerUnit { get; set; }
    public string? Measure { get; set; }
    public int? ExportDays { get; set; }
    public int FormedAdditionalEquipId { get; set; }

    [ForeignKey(nameof(FormedAdditionalEquipId))]
    public virtual FormedAdditionalEquip FormedAdditionalEquip { get; set; }

    [Key] public int Id { get; set; }
}
