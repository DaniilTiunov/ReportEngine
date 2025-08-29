using ReportEngine.App.Model;
using ReportEngine.App.Model.StandsModel;
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

        foreach (var stand in project.Stands) stand.StandSummCost = CalculateStandEquipCost(stand);


        project.Cost = project.Stands.Sum(stand => stand.StandSummCost);

        await _projectService.UpdateProjectAsync(project);
        await _projectService.UpdateStandEntity(project);

        _notificationService.ShowInfo("Расчёт завершён");
    }

    private async Task CalculateStandsCountAsync(ProjectModel project)
    {
        project.StandCount = project.Stands.Count;
    }

    private decimal CalculateStandEquipCost(StandModel standModel)
    {
        decimal cost = 0;

        foreach (var equipInStand in standModel.AdditionalEquipsInStand)
        foreach (var purpose in equipInStand.Purposes)
            cost += (decimal)(purpose.CostPerUnit ?? 0) * (decimal)(purpose.Quantity ?? 0);

        foreach (var frames in standModel.FramesInStand)
        foreach (var component in frames.Components)
            cost += component.Count * (decimal)(component.CostComponent ?? 0);

        return cost;
    }
}