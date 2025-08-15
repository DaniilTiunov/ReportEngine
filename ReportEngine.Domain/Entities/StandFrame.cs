using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities
{
    public class StandFrame
    {
        [Key]
        public int Id { get; set; }

        public int StandId { get; set; }
        public int FrameId { get; set; }

        [ForeignKey(nameof(StandId))]
        public virtual Stand Stand { get; set; }

        [ForeignKey(nameof(FrameId))]
        public virtual FormedFrame Frame { get; set; }
    }
}