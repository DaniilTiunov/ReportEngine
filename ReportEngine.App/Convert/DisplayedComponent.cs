using ReportEngine.Domain.Entities.BaseEntities.Interface;

namespace ReportEngine.App.Convert
{
    public class DisplayedComponent
    {
        public IBaseEquip Component { get; set; }
        public int Count { get; set; }
    }
}
