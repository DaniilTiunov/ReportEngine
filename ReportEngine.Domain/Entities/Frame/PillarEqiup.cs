using ReportEngine.Domain.Entities.BaseEntities;

namespace ReportEngine.Domain.Entities.Frame
{
    public class PillarEqiup : BaseFrame //Комплектующие для стойки
    {
        public int FormedFrameId { get; set; }
        public virtual FormedFrame FormedFrame { get; set; }
    }
}
