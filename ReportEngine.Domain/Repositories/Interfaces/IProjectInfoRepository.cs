using ReportEngine.Domain.Entities;

namespace ReportEngine.Domain.Repositories.Interfaces
{
    public interface IProjectInfoRepository : IBaseRepository<ProjectInfo>
    {
        Task<Stand> AddStandAsync(int projectId, Stand stand);
        Task UpdateStandAsync(Stand stand);
        Task<ProjectInfo> GetStandsByIdAsync(int projectId);
    }
}
