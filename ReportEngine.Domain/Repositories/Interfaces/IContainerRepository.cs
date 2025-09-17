using ReportEngine.Domain.Entities;

namespace ReportEngine.Domain.Repositories.Interfaces
{
    public interface IContainerRepository : IBaseRepository<ContainerBatch>
    {
        Task<ContainerBatch> GetByIdWithContainersAsync(int id);
        Task<IEnumerable<ContainerBatch>> GetAllByProjectIdAsync(int projectId);
        Task AddContainerToBatchAsync(int batchId, ContainerStand container);
    }
}
