using ReportEngine.App.Model;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.Services.Interfaces;

public interface IProjectService
{
    Task CreateProjectAsync(ProjectModel projectModel);
    Task UpdateProjectAsync(ProjectModel projectModel);
    Task AddStandToProjectAsync(int projectId, StandModel standModel);
    Task UpdateStandEntity(ProjectModel standModel);
    Task<ProjectModel> LoadProjectInfoAsync(int projectId);
    Task DeleteStandAsync(int projectId, int standId);
    Task DeleteObvFromStandAsync(int standId, int obvyazkaInStandId);

    Task<ContainerBatch> CreateBatchAsync(ContainerBatch batchModel);
    Task DeleteBatchAsync(int batchId);
    Task<IEnumerable<ContainerBatch>> GetBatchesByProjectAsync(int projectId);
    Task<ContainerBatch> GetBatchWithContainersAsync(int batchId);
    Task AddContainerToBatchAsync(int batchId, ContainerStand containerModel);
    Task RemoveContainerFromBatchAsync(int batchId, int containerId);
    Task DeleteContainerAsync(int containerId);
    Task AddStandToContainerAsync(int containerId, int standId);
    Task RemoveStandFromContainerAsync(int containerId, int standId);
}