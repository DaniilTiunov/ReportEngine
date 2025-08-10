using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;

namespace ReportEngine.Domain.Repositories.Interfaces
{
    public interface IFrameRepository : IBaseRepository<FormedFrame>
    {
        Task AddComponentAsync(int frameId, IBaseEquip component);
        Task RemoveComponentAsync(int frameId, IBaseEquip component);
    }
}
