using ReportEngine.Domain.Entities;

namespace ReportEngine.Domain.Repositories.Interfaces
{
    public interface IContainerRepository : IBaseRepository<ContainerBatch>
    {
        Task<ContainerBatch> GetByIdWithContainersAsync(int id);
        Task<IEnumerable<ContainerBatch>> GetAllByProjectIdAsync(int projectId);

        Task AddContainerToBatchAsync(int batchId, ContainerStand container);
        Task RemoveContainerFromBatchAsync(int batchId, int containerId);

        Task AddStandToContainerAsync(int containerId, int standId);
        Task RemoveStandFromContainerAsync(int containerId, int standId);

        Task DeleteContainerAsync(int containerId);
    }
}
