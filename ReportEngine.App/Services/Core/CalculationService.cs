using ReportEngine.App.Model;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities.BaseEntities.Interface;

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

        project.Cost = project.Stands.Sum(s => s.StandSummCost);

        await _projectService.UpdateProjectAsync(project);

        _notificationService.ShowInfo("Расчёт завершён");
    }

    private async Task CalculateStandsCountAsync(ProjectModel project)
    {
        project.StandCount = project.Stands.Count;
    }

    private decimal CalculateStandEquipCost(StandModel stand)
    {
        decimal cost = 0;

        // Дополнительное оборудование
        foreach (var equip in stand.AdditionalEquipsInStand)
            if (equip is IBaseEquip addEquip)
                cost += (decimal)addEquip.Cost;

        // Дренажи
        foreach (var drainage in stand.DrainagesInStand)
            if (drainage is IBaseEquip drainEquip)
                cost += (decimal)drainEquip.Cost;

        // Электрические компоненты
        foreach (var electrical in stand.ElectricalComponentsInStand)
            if (electrical is IBaseEquip elecEquip)
                cost += (decimal)elecEquip.Cost;

        return cost;
    }
}