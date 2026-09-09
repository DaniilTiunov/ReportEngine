using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities;

public class FormedAdditionalEquip
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<AdditionalEquipPurpose> Purposes { get; set; } = new List<AdditionalEquipPurpose>();

    public virtual ICollection<StandAdditionalEquip> StandAdditionalEquips { get; set; } =
        new List<StandAdditionalEquip>();
}