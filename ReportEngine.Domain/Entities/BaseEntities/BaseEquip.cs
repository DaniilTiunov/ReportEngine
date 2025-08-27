using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ReportEngine.Domain.Entities.BaseEntities.Interface;

namespace ReportEngine.Domain.Entities.BaseEntities;

public class BaseEquip : IBaseEquip
{
    public float Height { get; set; }
    public float Width { get; set; }
    public float Depth { get; set; }
    public float Weight { get; set; }
    public string Type { get; set; }
    public int ExportDays { get; set; }
    public string Measure { get; set; }
    public float Cost { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; }
}