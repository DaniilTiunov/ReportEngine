using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities;

public class StandDrainage
{
    [Key] public int Id { get; set; }

    public int StandId { get; set; }
    public int DrainageId { get; set; }

    [ForeignKey(nameof(StandId))] public virtual Stand Stand { get; set; }

    [ForeignKey(nameof(DrainageId))] public virtual FormedDrainage Drainage { get; set; }
}