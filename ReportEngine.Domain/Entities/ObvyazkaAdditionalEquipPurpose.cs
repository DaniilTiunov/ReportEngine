using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Entities.BaseEntities.Interface;

namespace ReportEngine.Domain.Entities;

public class ObvyazkaAdditionalEquipPurpose : IPurposeEntity
{
    public int? ObvyazkaInStandId { get; set; }

    [ForeignKey("ObvyazkaInStandId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual ObvyazkaInStand? ObvyazkaInStand { get; set; }

    public string? Purpose { get; set; }
    public string? Material { get; set; }
    public float? Quantity { get; set; }
    public float? CostPerUnit { get; set; }
    public string? Measure { get; set; }
    public float? Weight { get; set; }
    public int? ExportDays { get; set; }

    [Key] public int Id { get; set; }
}
