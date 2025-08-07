using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Entities.Frame;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities
{
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
        public virtual ICollection<FrameDetail> FrameDetails { get; set; } = new List<FrameDetail>(); // Компоненты рамы
        public virtual ICollection<FrameRoll> FrameRolls { get; set; } = new List<FrameRoll>(); // Возможные компоненты рамы
        public virtual ICollection<PillarEqiup> PillarEqiups { get; set; } = new List<PillarEqiup>(); // Возможные компоненты рамы


    }
}
