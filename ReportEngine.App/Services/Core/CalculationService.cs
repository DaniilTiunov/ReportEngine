using ReportEngine.App.Model;
using ReportEngine.App.Model.CalculationModels;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.Services.Interfaces;

namespace ReportEngine.App.Services.Core;

public class CalculationService : ICalculationService
{
    private readonly INotificationService _notificationService;
    private readonly IProjectService _projectService;

    public StandSettingsModel DefaultStandSettings { get; set; } = new();
    public HumanCostSettingsModel HumanCostSettingsModel { get; set; } = new();

    public CalculationService(
        IProjectService projectService,
        INotificationService notificationService)
    {
        _projectService = projectService;
        _notificationService = notificationService;
    }

    private async Task LoadSettingsCost()
    {
        await DefaultStandSettings.LoadStandsSettingsDataAsync();
        await HumanCostSettingsModel.LoadHumanCostDataFromIniAsync();
    }

    public async Task CalculateProjectAsync(ProjectModel project)
    {
        await LoadSettingsCost();

        await CalculateStandsCountAsync(project);

        foreach (var stand in project.Stands)
            stand.StandSummCost = CalculateStandEquipCost(stand);

        var standsCost = project.Stands.Sum(stand => stand.StandSummCost);
        var galvanizedCost = CalculateGalvanizedCost(project);

        project.Cost = standsCost + galvanizedCost;

        project.HumanCost = project.Stands.Sum(ObvHumanCostCalculation);

        await _projectService.UpdateProjectAsync(project);
        await _projectService.UpdateStandEntity(project);
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
                ? (c.Count ?? 0) * (decimal)(c.CostComponent ?? 0)
                : (decimal)(c.Length ?? 0) * (decimal)(c.CostComponent ?? 0));

        cost += standModel.ElectricalComponentsInStand
            .SelectMany(e => e.Purposes)
            .Sum(p => (decimal)(p.CostPerUnit ?? 0) * (decimal)(p.Quantity ?? 0));

        cost += standModel.DrainagesInStand
            .SelectMany(d => d.Purposes)
            .Sum(p => (decimal)(p.CostPerUnit ?? 0) * (decimal)(p.Quantity ?? 0));

        return cost;
    }

    private decimal CalculateGalvanizedCost(ProjectModel projectModel)
    {
        decimal cost = 0;

        if (projectModel.IsGalvanized)
        {
            cost = projectModel.Stands.Count * (decimal)HumanCostSettingsModel.GalvanizedStands;
        }

        return cost;
    }

    private float ObvHumanCostCalculation(StandModel stand)
    {
        return stand.ObvyazkiInStand.Sum(obv => obv.HumanCost ?? 0f);
    }
}
