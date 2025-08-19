using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities;

public class StandAdditionalEquip
{
    [Key] public int Id { get; set; }

    public int StandId { get; set; }
    public int AdditionalEquipId { get; set; }

    [ForeignKey(nameof(StandId))] public virtual Stand Stand { get; set; }

    [ForeignKey(nameof(AdditionalEquipId))]
    public virtual FormedAdditionalEquip AdditionalEquip { get; set; }
}