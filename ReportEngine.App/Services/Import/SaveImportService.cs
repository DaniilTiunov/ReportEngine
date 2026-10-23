using ReportEngine.App.Model;
using ReportEngine.App.Services.Core;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Import.Dto;

namespace ReportEngine.App.Services.Import;

public class SaveImportService
{
    private readonly IProjectService _projectService;

    public SaveImportService(IProjectService projectService)
    {
        _projectService = projectService;
    }

    public async Task SaveImportedProjectAsync(ProjectModel projectModel)
    {
        await SaveProjectInfoAsync(projectModel);
    }

    private async Task SaveProjectInfoAsync(ProjectModel projectModel)
    {
        await _projectService.CreateProjectAsync(projectModel);
    }
}