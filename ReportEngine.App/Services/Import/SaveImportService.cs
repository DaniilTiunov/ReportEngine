using ReportEngine.App.Model;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.Services.Import;

public class SaveImportService
{
    private readonly IProjectService _projectService;
    private readonly IDialogService _dialogService;
    private readonly IStandService _standService;
    private readonly INotificationService _notificationService;
    private readonly IObvyazkaRepository _obvyazkaRepository;

    public SaveImportService(
        IProjectService projectService,
        IDialogService dialogService,
        IStandService standService,
        INotificationService notificationService,
        IObvyazkaRepository obvyazkaRepository)
    {
        _projectService = projectService;
        _dialogService = dialogService;
        _standService = standService;
        _notificationService = notificationService;
        _obvyazkaRepository = obvyazkaRepository;
    }

    public async Task SaveImportedProjectAsync(ProjectModel projectModel)
    {
        await _dialogService.RunWithProgressDialogAsync(async () =>
        {
            await ValidateObvyazkiAsync(projectModel);
            await SaveProjectInfoAsync(projectModel);
            await SaveStandsAsync(projectModel);
        });
    }

    private async Task ValidateObvyazkiAsync(ProjectModel projectModel)
    {
        var importedObvyazki = projectModel.Stands
            .SelectMany(stand => stand.ObvyazkiInStand)
            .ToList();

        if (importedObvyazki.Count == 0)
            return;

        var existingIds = (await _obvyazkaRepository.GetAllAsync())
            .Select(obvyazka => obvyazka.Id)
            .ToHashSet();
        var missingIds = importedObvyazki
            .Select(obvyazka => obvyazka.ObvyazkaId)
            .Where(id => id <= 0 || !existingIds.Contains(id))
            .Distinct()
            .OrderBy(id => id)
            .ToList();

        if (missingIds.Count > 0)
            throw new InvalidOperationException(
                $"В справочнике не найдены обвязки с ID: {string.Join(", ", missingIds)}. " +
                "Проверьте столбец K в импортируемом документе.");
    }

    private async Task SaveProjectInfoAsync(ProjectModel projectModel)
    {
        await _projectService.CreateProjectAsync(projectModel);
    }

    private async Task SaveStandsAsync(ProjectModel projectModel)
    {
        foreach (var stand in projectModel.Stands)
        {
            await _projectService.AddStandToProjectAsync(projectModel.CurrentProjectId, stand, includeFrames: false);
            await SaveStandEquips(stand);
            await SaveFramesAsync(stand);
        }
        
        _notificationService.ShowInfo("Стенды успешно импортированы!");
    }

    private async Task SaveStandEquips(StandModel standModel)
    {
        await _standService.AddCustomAdditionalEquipAsync(
            standModel.Id, standModel.AllAdditionalEquipPurposesInStand);
        await _standService.AddCustomElectricalComponentAsync(
            standModel.Id, standModel.AllElectricalPurposesInStand);
        await _standService.AddCustomDrainageAsync(
            standModel.Id, standModel.AllDrainagePurposesInStand);
        
        _notificationService.ShowInfo("Комплектующие стенда успешно импортированы!");
    }

    private async Task SaveFramesAsync(StandModel standModel)
    {
        foreach (var frame in standModel.FramesInStand)
        {
            await _standService.AddFrameToStandByNameAsync(standModel.Id, frame.Name);
        }
        
        _notificationService.ShowInfo("Рамы успешно импортированы");
    }
}
