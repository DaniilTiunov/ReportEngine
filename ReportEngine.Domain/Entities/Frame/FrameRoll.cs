using ReportEngine.Domain.Entities.BaseEntities;

namespace ReportEngine.Domain.Entities.Frame
{
    public class FrameRoll : BaseFrame //Таблица прокат
    {
        public int FormedFrameId { get; set; }
        public virtual FormedFrame FormedFrame { get; set; }
    }
}
