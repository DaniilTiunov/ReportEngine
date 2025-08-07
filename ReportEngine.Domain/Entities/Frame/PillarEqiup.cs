using ReportEngine.Domain.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities.Frame
{
    public class PillarEqiup : BaseFrame //Комплектующие для стойки
    {
        
        public virtual ICollection<FormedFrame> FormedFrames { get; set; } = new List<FormedFrame>();
    }
}
