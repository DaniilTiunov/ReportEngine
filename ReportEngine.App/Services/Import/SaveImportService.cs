using ReportEngine.App.Model;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.Services.Interfaces;

namespace ReportEngine.App.Services.Import;

public class SaveImportService
{
    private readonly IProjectService _projectService;
    private readonly IDialogService _dialogService;
    private readonly IStandService _standService;

    public SaveImportService(
        IProjectService projectService,
        IDialogService dialogService,
        IStandService standService)
    {
        _projectService = projectService;
        _dialogService = dialogService;
        _standService = standService;
    }

    public async Task SaveImportedProjectAsync(ProjectModel projectModel)
    {
        await _dialogService.RunWithProgressDialogAsync(async () =>
        {
            await SaveProjectInfoAsync(projectModel);
            await SaveStandsAsync(projectModel);
        });
    }

    private async Task SaveProjectInfoAsync(ProjectModel projectModel)
    {
        await _projectService.CreateProjectAsync(projectModel);
    }

    private async Task SaveStandsAsync(ProjectModel projectModel)
    {
        foreach (var stand in projectModel.Stands)
        {
            await _projectService.AddStandToProjectAsync(projectModel.CurrentProjectId, stand);
            await SaveStandEquips(stand);
        }
    }

    private async Task SaveStandEquips(StandModel standModel)
    {
        await _standService.AddCustomAdditionalEquipAsync(
            standModel.Id, standModel.AllAdditionalEquipPurposesInStand);
        await _standService.AddCustomElectricalComponentAsync(
            standModel.Id, standModel.AllElectricalPurposesInStand);
        await _standService.AddCustomDrainageAsync(
            standModel.Id, standModel.AllDrainagePurposesInStand);
    }
}
