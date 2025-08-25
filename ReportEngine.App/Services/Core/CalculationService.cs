using ReportEngine.App.Model;
using ReportEngine.App.Services.Interfaces;

namespace ReportEngine.App.Services.Core;

public class CalculationService : ICalculationService
{
    private readonly IProjectService _projectService;
    private readonly INotificationService _notificationService;
    
    public CalculationService(IProjectService projectService, INotificationService notificationService)
    {
        _projectService = projectService;
        _notificationService = notificationService;
    }

    public async Task CalculateProjectAsync(ProjectModel project)
    {
        project.StandCount = project.Stands.Count;
        _notificationService.ShowInfo("Расчёт завершён");
    }
}