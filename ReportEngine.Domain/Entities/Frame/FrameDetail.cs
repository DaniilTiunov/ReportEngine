using ReportEngine.Domain.Entities.BaseEntities;

namespace ReportEngine.Domain.Entities.Frame
{
    public class FrameDetail : BaseFrame //Таблица детали рамы
    {
        public int FormedFrameId { get; set; }
        public virtual FormedFrame FormedFrame { get; set; }
    }
}
