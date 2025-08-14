using ReportEngine.Domain.Entities;

namespace ReportEngine.Domain.Repositories.Interfaces
{
    public interface IProjectInfoRepository : IBaseRepository<ProjectInfo>
    {
        Task<Stand> AddStandAsync(int projectId, Stand stand);
        Task UpdateStandAsync(Stand stand);
        Task<ProjectInfo> GetStandsByIdAsync(int projectId);
        Task AddStandObvyazkaAsync(int standId, ObvyazkaInStand standObvyazka);
        Task AddFrameToStandAsync(int standId, FormedFrame frame);
        Task AddDrainageToStandAsync(int standId, FormedDrainage drainage);
        Task<IEnumerable<FormedFrame>> GetAllFramesInStandAsync(int standId);
        Task<IEnumerable<FormedDrainage>> GetAllDrainagesInStandAsync(int standId);
        Task<IEnumerable<ObvyazkaInStand>> GetAllObvyazkiInStandAsync(int standId);
    }
}
