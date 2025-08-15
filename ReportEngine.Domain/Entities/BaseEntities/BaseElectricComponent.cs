using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ReportEngine.Domain.Entities.BaseEntities.Interface;

namespace ReportEngine.Domain.Entities.BaseEntities;

public class BaseElectricComponent : IBaseEquip
{
    public float Cost { get; set; }
    public string Measure { get; set; }
    public int Cabel { get; set; }
    public int ElectricProtection { get; set; }
    public int CabelInput { get; set; }
    public int ExportDays { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; }
}