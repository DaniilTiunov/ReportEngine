using ReportEngine.App.Model;
using ReportEngine.App.Services.Interfaces;

namespace ReportEngine.App.Services.Core;

public class CalculationService : ICalculationService
{
    private readonly INotificationService _notificationService;
    private readonly IProjectService _projectService;

    public CalculationService(IProjectService projectService, INotificationService notificationService)
    {
        _projectService = projectService;
        _notificationService = notificationService;
    }

    public async Task CalculateProjectAsync(ProjectModel project)
    {
        await CalculateStandsCountAsync(project);

        await _projectService.UpdateProjectAsync(project);
        _notificationService.ShowInfo("Расчёт завершён");
    }

    private async Task CalculateStandsCountAsync(ProjectModel project)
    {
        project.StandCount = project.Stands.Count;
    }
}