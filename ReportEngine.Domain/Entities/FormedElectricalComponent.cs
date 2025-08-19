using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities;

public class FormedElectricalComponent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<ElectricalPurpose> Purposes { get; set; } = new List<ElectricalPurpose>();

    public virtual ICollection<StandElectricalComponent> StandElectricalComponents { get; set; } =
        new List<StandElectricalComponent>();
}