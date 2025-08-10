using ReportEngine.Domain.Entities.BaseEntities.Interface;

namespace ReportEngine.App.ViewModels
{
    public class ComponentWithCount
    {
        public IBaseEquip Component { get; set; }
        public int Count { get; set; }
    }
}