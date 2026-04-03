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

        project.HumanCost = (project.Stands
            .Sum(ObvHumanCostCalculation)
            + CalculatePaintAndSandBlustHumanCost(project)
            + ObvProdTime(project)
            + ObvAllTest(project)
            + CalculateFramesProductionHumanCost(project)).Round(2);

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

    private float CalculateFramesProductionHumanCost(ProjectModel projectModel)
    {
        float humanCost = 0;

        var items = projectModel.Stands
            .SelectMany(stand => stand.FramesInStand)
            .ToList();

        var groupedByType = items
            .GroupBy(x => x.FrameType);

        foreach (var group in groupedByType)
        {
            int count = group.Count();

            string prodTimeKey = "";

            switch (group.Key)
            {
                case "Рама":
                    prodTimeKey = "FrameProdTime";
                    break;

                case "Стойка":
                    prodTimeKey = "StandFabTime";
                    break;

                case "Шкаф":
                    prodTimeKey = "CabinetPrepTime";
                    break;

                default:
                    continue;
            }

            float tr = count *
                       _parametersStore[CalculationParameterType.FrameCost, prodTimeKey]
                           .Value.ToFloat();

            humanCost += tr;
        }

        return humanCost;
    }

    private float? ObvProdTime(ProjectModel projectModel)
    {
        float?  prodTime = 0;

        var obvyazki = projectModel.Stands.SelectMany(stand => stand.ObvyazkiInStand);

        prodTime = (obvyazki.Sum(obv => obv.OtherLineCount) + 1) *
                   _parametersStore[CalculationParameterType.HumanCost,"HoleDrillTime"].Value.ToFloat() +
                   _parametersStore[CalculationParameterType.HumanCost,"ManifoldWeldTime"].Value.ToFloat();

        return prodTime;
    }

    private float? ObvAllTest(ProjectModel projectModel)
    {
        float? allTest = 0;

        var obvyazki = projectModel.Stands.SelectMany(stand => stand.ObvyazkiInStand);

        allTest = obvyazki.Count() * _parametersStore[CalculationParameterType.HumanCost, "AllTestsTime"].Value.ToFloat();

        return allTest;
    }

    private float? CalculatePaintAndSandBlustHumanCost(ProjectModel projectModel)
    {
        float? humanCost = 0;

        var framesCount = projectModel.Stands.SelectMany(stand => stand.FramesInStand).Count();
        var obvCount = projectModel.Stands.SelectMany(stand => stand.ObvyazkiInStand).Count();
        var framesInStand = projectModel.Stands.SelectMany(stand => stand.FramesInStand);

        foreach (var frame in framesInStand)
        {
            humanCost += CalculatePaintTime(framesCount, obvCount, frame.FrameType, projectModel.IsGalvanized);
        }

        return humanCost;
    }

    public float CalculatePaintTime(int frameCount, int obvCount, string isCabinet, bool isGalvanized)
    {
        // Если это шкаф или изделие оцинковано,
        // покраска и пескоструй не требуются
        if (isCabinet != "Шкаф" || isGalvanized)
            return 0;

        // Время покраски всех рам
        float tcolr = frameCount * _parametersStore[CalculationParameterType.FrameCost, "FramePaintTime"].Value.ToFloat();
        // Время покраски всех обвязок
        float tcolob = obvCount * _parametersStore[CalculationParameterType.FrameCost,"PipeworkPaintTime"].Value.ToFloat();
        // Tprep = Nr * время подготовки 1 рамы
        float tprep = frameCount * _parametersStore[CalculationParameterType.FrameCost,"FramePrepTime"].Value.ToFloat();
        // Tall = время подготовки всего оборудования
        float tall = _parametersStore[CalculationParameterType.StandCost,"EquipmentPrepTime"].Value.ToFloat();
        // Tsand = фиксированное время пескоструя
        float tsand = _parametersStore[CalculationParameterType.SandBlastCost,"SandblastingTime"].Value.ToFloat();
        // Итоговое время:
        // покраска + подготовка + пескоструй
        return tcolr + tcolob + tprep + tall + tsand;
    }
}
