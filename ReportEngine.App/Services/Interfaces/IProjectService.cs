using ReportEngine.App.Model;
using ReportEngine.App.Model.StandsModel;

namespace ReportEngine.App.Services.Interfaces;

public interface IProjectService
{
    Task CreateProjectAsync(ProjectModel projectModel);
    Task UpdateProjectAsync(ProjectModel projectModel);
    Task AddStandToProjectAsync(int projectId, StandModel standModel);
    Task UpdateStandEntity(ProjectModel standModel);
    Task<ProjectModel> LoadProjectInfoAsync(int projectId);
}