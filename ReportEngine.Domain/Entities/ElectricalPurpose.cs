using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ReportEngine.Domain.Entities.BaseEntities.Interface;

namespace ReportEngine.Domain.Entities;

public class ElectricalPurpose : IPurposeEntity
{
    public string Purpose { get; set; }
    public string? Material { get; set; }
    public float? Quantity { get; set; }
    public float? CostPerUnit { get; set; }
    public string? Measure { get; set; }

    public int FormedElectricalComponentId { get; set; }

    [ForeignKey(nameof(FormedElectricalComponentId))]
    public virtual FormedElectricalComponent FormedElectricalComponent { get; set; }

    [Key] public int Id { get; set; }
}