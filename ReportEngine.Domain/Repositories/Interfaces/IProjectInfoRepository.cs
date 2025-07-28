using ReportEngine.Domain.Entities;

namespace ReportEngine.Domain.Repositories.Interfaces
{
    public interface IProjectInfoRepository : IBaseRepository<ProjectInfo>
    {
        Task AddStandAsync(int projectId, Stand stand);
    }
}
