using ReportEngine.Domain.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities.Frame
{
    public class FrameDetail : BaseFrame //Таблица детали рамы
    {
        
        public virtual ICollection<FormedFrame>? FormedFrames { get; set; } = new List<FormedFrame>();
    }
}
