using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ReportEngine.Domain.Entities.BaseEntities.Interface;

namespace ReportEngine.Domain.Entities.BaseEntities;

public class BaseFrame : IBaseEquip
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int? ExportDays { get; set; }
    public string? Measure { get; set; }
    public float? Cost { get; set; }
    public float? Weight { get; set; }
    public string? Name { get; set; }
}
