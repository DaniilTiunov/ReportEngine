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

        foreach (var stand in project.Stands) 
            stand.StandSummCost = CalculateStandEquipCost(stand);

        project.Cost = project.Stands.Sum(stand => stand.StandSummCost);
        project.HumanCost = project.Stands.Sum(ObvHumanCostCalculation);

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

        cost += standModel.AdditionalEquipsInStand
            .SelectMany(e => e.Purposes)
            .Sum(p => (decimal)(p.CostPerUnit ?? 0) * (decimal)(p.Quantity ?? 0));

        cost += standModel.FramesInStand
            .SelectMany(f => f.Components)
            .Sum(c => c.Length == null
                ? c.Count * (decimal)(c.CostComponent ?? 0)
                : (decimal)(c.Length ?? 0) * (decimal)(c.CostComponent ?? 0));

        cost += standModel.ElectricalComponentsInStand
            .SelectMany(e => e.Purposes)
            .Sum(p => (decimal)(p.CostPerUnit ?? 0) * (decimal)(p.Quantity ?? 0));

        cost += standModel.DrainagesInStand
            .SelectMany(d => d.Purposes)
            .Sum(p => (decimal)(p.CostPerUnit ?? 0) * (decimal)(p.Quantity ?? 0));

        return cost;
    }
    
    private float ObvHumanCostCalculation(StandModel stand)
    {
        return stand.ObvyazkiInStand.Sum(obv => obv.HumanCost);
    }
}