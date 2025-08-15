using ReportEngine.Domain.Entities;

namespace ReportEngine.Domain.Repositories.Interfaces
{
    public interface IProjectInfoRepository : IBaseRepository<ProjectInfo>
    {
        Task<Stand> AddStandAsync(int projectId, Stand stand);
        Task UpdateStandAsync(Stand stand);
        Task<ProjectInfo> GetStandsByIdAsync(int projectId);
        Task AddStandObvyazkaAsync(int standId, ObvyazkaInStand standObvyazka);

        // Теперь добавляем связь через StandFrame
        Task AddFrameToStandAsync(int standId, int frameId);
        Task AddDrainageToStandAsync(int standId, FormedDrainage drainage);

        // Получаем связи StandFrame, а не сами рамы
        Task<IEnumerable<StandFrame>> GetAllFramesInStandAsync(int standId);
        Task<IEnumerable<FormedDrainage>> GetAllDrainagesInStandAsync(int standId);
        Task<IEnumerable<ObvyazkaInStand>> GetAllObvyazkiInStandAsync(int standId);
    }
}