using ReportEngine.App.Model;
using ReportEngine.App.Model.CalculationModels;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.Services.Interfaces;

namespace ReportEngine.App.Services.Core;

public class CalculationService : ICalculationService
{
    private readonly IProjectService _projectService;

    public StandSettingsModel DefaultStandSettings { get; set; } = new();
    public HumanCostSettingsModel HumanCostSettingsModel { get; set; } = new();

    public CalculationService(IProjectService projectService)
    {
        _projectService = projectService;
    }

    private async Task LoadSettingsCost()
    {
        await DefaultStandSettings.LoadStandsSettingsDataAsync();
        await HumanCostSettingsModel.LoadHumanCostDataFromIniAsync();
    }

    public async Task CalculateProjectAsync(ProjectModel project)
    {
        await LoadSettingsCost();

        CalculateStandsCountAsync(project);

        foreach (var stand in project.Stands)
        {
            CalculateStandsWidthAsync(stand);
            CalculateStandsWeightAsync(stand);

            stand.StandSummCost = CalculateStandEquipCost(stand);
        }

        var standsCost = project.Stands.Sum(stand => stand.StandSummCost);
        var galvanizedCost = CalculateGalvanizedCost(project);

        project.Cost = standsCost + galvanizedCost;

        project.HumanCost = project.Stands.Sum(ObvHumanCostCalculation);

        await _projectService.UpdateProjectAsync(project);
        await _projectService.UpdateStandEntity(project);
    }

    private void CalculateStandsCountAsync(ProjectModel project)
    {
        project.StandCount = project.Stands.Count;
    }

    private void CalculateStandsWeightAsync(StandModel standModel)
    {
        standModel.Weight = 0;

        standModel.Weight += standModel.FramesInStand.Sum(fr => fr.Weight);

        standModel.Weight += standModel.AllElectricalPurposesInStand.Sum(ec => ec.Weight) ?? 0.0f;

        standModel.Weight += standModel.AllAdditionalEquipPurposesInStand.Sum(ec => ec.Weight) ?? 0.0f;

        standModel.Weight += standModel.AllDrainagePurposesInStand.Sum(dp => dp.Weight) ?? 0.0f;

        standModel.Weight += standModel.ObvyazkaAdditionalComponents.Sum(ac => ac.Weight) ?? 0.0f;

        standModel.Weight += standModel.ObvyazkiInStand.Sum(ec => ec.Weight) ?? 0.0f;
    }

    private void CalculateStandsWidthAsync(StandModel standModel)
    {
        standModel.Width = standModel.FramesInStand.Sum(fr  => fr.Width);
    }

    private decimal CalculateStandEquipCost(StandModel standModel)
    {
        decimal cost = 0;

        cost += standModel.AllElectricalPurposesInStand
            .Sum(p => (decimal)(p.CostPerUnit ?? 0) * (decimal)(p.Quantity ?? 0));

        cost += standModel.FramesInStand
            .SelectMany(f => f.Components)
            .Sum(c => c.Length == null
                ? (c.Count ?? 0) * (decimal)(c.CostComponent ?? 0)
                : (decimal)(c.Length ?? 0) * (decimal)(c.CostComponent ?? 0));

        cost += standModel.AllAdditionalEquipPurposesInStand
            .Sum(p => (decimal)(p.CostPerUnit ?? 0) * (decimal)(p.Quantity ?? 0));

        cost += standModel.AllDrainagePurposesInStand
            .Sum(p => (decimal)(p.CostPerUnit ?? 0) * (decimal)(p.Quantity ?? 0));

        return cost;
    }

    private decimal CalculateGalvanizedCost(ProjectModel projectModel)
    {
        if (!projectModel.IsGalvanized)
            return 0;

        return projectModel.StandCount * (decimal)HumanCostSettingsModel.GalvanizedStands;
    }

    private float ObvHumanCostCalculation(StandModel stand)
    {
        return stand.ObvyazkiInStand.Sum(obv => obv.HumanCost ?? 0f);
    }
}
