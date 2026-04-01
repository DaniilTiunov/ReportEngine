using ReportEngine.App.Extensions;
using ReportEngine.App.Model;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities.CalculationParameters.Enums;
using ReportEngine.Domain.Store;
using ReportEngine.Shared.Helpers;

namespace ReportEngine.App.Services.Calculation;

public class CalculationService : ICalculationService
{
    private readonly ParametersStore _parametersStore;
    private readonly IProjectService _projectService;

    public CalculationService(
        IProjectService projectService,
        ParametersStore parametersStore)
    {
        _projectService = projectService;
        _parametersStore = parametersStore;
    }


    public async Task CalculateProjectAsync(ProjectModel project)
    {
        CalculateStandsCount(project);

        foreach (var stand in project.Stands)
        {
            CalculateStandsWidth(stand);
            CalculateStandsWeight(stand);

            stand.StandSummCost = CalculateStandEquipCost(stand);
        }

        var standsCost = project.Stands.Sum(stand => stand.StandSummCost);
        var galvanizedCost = CalculateGalvanizedCost(project);

        project.Cost = standsCost + galvanizedCost;

        project.HumanCost = (project.Stands.Sum(ObvHumanCostCalculation) + CalculateOtherHumanCost(project)).Round(2);

        await _projectService.UpdateProjectAsync(project);
        await _projectService.UpdateStandEntity(project);
    }

    private void CalculateStandsCount(ProjectModel project)
    {
        project.StandCount = project.Stands.Count;
    }

    private void CalculateStandsWeight(StandModel standModel)
    {
        standModel.Weight = 0;

        standModel.Weight += standModel.FramesInStand.Sum(fr => fr.Weight);

        standModel.Weight += standModel.AllElectricalPurposesInStand.Sum(ec => ec.Weight) ?? 0.0f;

        standModel.Weight += standModel.AllAdditionalEquipPurposesInStand.Sum(ec => ec.Weight) ?? 0.0f;

        standModel.Weight += standModel.AllDrainagePurposesInStand.Sum(dp => dp.Weight) ?? 0.0f;

        standModel.Weight += standModel.ObvyazkaAdditionalComponents.Sum(ac => ac.Weight) ?? 0.0f;

        standModel.Weight += standModel.ObvyazkiInStand.Sum(ec => ec.Weight) ?? 0.0f;
    }

    private void CalculateStandsWidth(StandModel standModel)
    {
        standModel.Width = standModel.FramesInStand.Sum(fr => fr.Width);
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

        cost += standModel.ObvyazkaAdditionalComponents
            .Sum(p => (decimal)(p.CostPerUnit ?? 0) * (decimal)(p.Quantity ?? 0));

        cost += standModel.ObvyazkiInStand
            .Sum(p =>
                Convert.ToDecimal(p.ArmatureCostPerUnit) * Convert.ToDecimal(p.ArmatureCount) +
                Convert.ToDecimal(p.KMCHCostPerUnit) * Convert.ToDecimal(p.KMCHCount) +
                Convert.ToDecimal(p.TreeSocketMaterialCostPerUnit) * Convert.ToDecimal(p.TreeSocketCount) +
                Convert.ToDecimal(p.MaterialLineCostPerUnit) * Convert.ToDecimal(p.MaterialLineCount));

        return cost;
    }

    private decimal CalculateGalvanizedCost(ProjectModel projectModel)
    {
        if (!projectModel.IsGalvanized)
            return 0;

        return projectModel.StandCount *
               _parametersStore[CalculationParameterType.HumanCost, "TestBenchGalvCost"].Value.ToDecimal();
    }

    private float ObvHumanCostCalculation(StandModel stand)
    {
        return (float)Math.Round(stand.ObvyazkiInStand.Sum(obv => obv.HumanCost ?? 0f), 2);
    }

    private float CalculateOtherHumanCost(ProjectModel projectModel)
    {
        float totalHumanCost = 0;

        var standsCount = projectModel.Stands.Count;
        var framesCount = projectModel.Stands.Sum(s => s.FramesInStand.Count);
        var obvyazkiCount = projectModel.Stands.Sum(s => s.ObvyazkiInStand.Count);

        var humanStandsCost = (_parametersStore[CalculationParameterType.HumanCost, "StandInspectTime"].Value.ToDouble()
                              + _parametersStore[CalculationParameterType.HumanCost, "HoleDrillTime"].Value.ToDouble()
                              + _parametersStore[CalculationParameterType.HumanCost, "InputInstallTime"].Value
                                  .ToDouble()
                              + _parametersStore[CalculationParameterType.HumanCost, "BusbarHoleDrillTime"].Value
                                  .ToDouble()
                              + _parametersStore[CalculationParameterType.HumanCost, "ManifoldWeldTime"].Value
                                  .ToDouble()
                              + _parametersStore[CalculationParameterType.HumanCost, "EquipmentPrepTime"].Value
                                  .ToDouble() * standsCount
                              + _parametersStore[CalculationParameterType.HumanCost, "AllTestsTime"].Value.ToDouble()
                              + _parametersStore[CalculationParameterType.HumanCost, "FinalWorkTime"].Value.ToDouble()
                              + _parametersStore[CalculationParameterType.HumanCost, "OtherOpsTime"].Value.ToDouble());

        var humanFrameCost = (_parametersStore[CalculationParameterType.FrameCost, "FramePaintTime"].Value.ToDouble()
                              + _parametersStore[CalculationParameterType.FrameCost, "FramePrepTime"].Value.ToDouble()
                              + _parametersStore[CalculationParameterType.FrameCost, "FrameProdTime"].Value.ToDouble()
                              +_parametersStore[CalculationParameterType.FrameCost, "FrameFabCost"].Value.ToDouble()) * framesCount +
                              (_parametersStore[CalculationParameterType.FrameCost, "PipeworkPaintTime"].Value.ToDouble() * obvyazkiCount);

        totalHumanCost = humanStandsCost.ToFloat() + humanFrameCost.ToFloat();

        return totalHumanCost;
    }
}
