using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities;

public class FormedFrame
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; }
    public string FrameType { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float Depth { get; set; }
    public float Weight { get; set; }
    public string Designe { get; set; }

    public virtual ICollection<StandFrame> StandFrames { get; set; } = new List<StandFrame>();
    public virtual ICollection<FrameComponent> Components { get; set; } = new List<FrameComponent>();
}