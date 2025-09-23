using ReportEngine.App.Model;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.Services.Interfaces;

public interface IProjectService
{
    Task CreateProjectAsync(ProjectModel projectModel);
    Task UpdateProjectAsync(ProjectModel projectModel);
    Task AddStandToProjectAsync(int projectId, StandModel standModel);
    Task CopyStandsAsync(ProjectModel projectModel);
    Task UpdateStandEntity(ProjectModel standModel);
    Task<ProjectModel> LoadProjectInfoAsync(int projectId);
    Task DeleteStandAsync(int projectId, int standId);
    Task DeleteObvFromStandAsync(int standId, int obvyazkaInStandId);
    Task UpdateObvInStandAsync(ProjectModel projectModel, Obvyazka selectedObvyazka);
    Task DeleteFrameFromStandAsync(ProjectModel projectModel);
    Task LoadAllObvyazkiInProject(ProjectModel projectModel);
}