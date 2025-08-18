using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities;

public class StandElectricalComponent
{
    [Key]
    public int Id { get; set; }

    public int StandId { get; set; }
    public int ElectricalComponentId { get; set; }

    [ForeignKey(nameof(StandId))]
    public virtual Stand Stand { get; set; }

    [ForeignKey(nameof(ElectricalComponentId))]
    public virtual FormedElectricalComponent ElectricalComponent { get; set; }
}