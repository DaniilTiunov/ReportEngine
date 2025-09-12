using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities;

public class FrameComponent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int FormedFrameId { get; set; }
    public FormedFrame FormedFrame { get; set; }

    public int ComponentId { get; set; }
    public string ComponentType { get; set; }
    public int Count { get; set; }
    public float? CostComponent { get; set; }
    public float? Length { get; set; } // Метраж

    public string? Measure { get; set; }
}