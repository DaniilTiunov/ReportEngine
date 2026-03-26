using System.Collections.ObjectModel;
using ReportEngine.App.ModelWrappers;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.Services.Core;

public class ProjectDataLoaderSerive : IProjectDataLoaderService
{
    private readonly IStandService _standService;
    private readonly IProjectInfoRepository _projectInfoRepository;

    public ProjectDataLoaderSerive(IStandService standService, IProjectInfoRepository projectInfoRepository)
    {
        _projectInfoRepository = projectInfoRepository;
        _standService = standService;
    }

    public async Task LoadAllProjectStandsAsync(int projectId, ProjectViewModel viewModel)
    {
        var projectInfo = await _projectInfoRepository.GetStandsByIdAsync(projectId);
        var standsEntities = projectInfo.Stands;

        viewModel.CurrentProjectModel.Stands.Clear();
        foreach (var standEntity in standsEntities)
        {
            var standModel = StandDataConverter.ConvertToStandModel(standEntity);

            viewModel.CurrentProjectModel.Stands.Add(standModel);
        }
    }

    public async Task LoadFramesToViewModelAsync(ProjectViewModel viewModel)
    {
        var frames = await _standService.LoadAllAvailableFrameAsync();
        viewModel.AllAvailableFrames = new ObservableCollection<FormedFrame>(frames);
        viewModel.OnPropertyChanged(nameof(viewModel.AllAvailableFrames));
    }

    public async Task LoadDrainagesToViewModelAsync(ProjectViewModel viewModel)
    {
        var drainages = await _standService.LoadAllAvailableDrainagesAsync();
        viewModel.AllAvailableDrainages = new ObservableCollection<FormedDrainage>(drainages);
        viewModel.OnPropertyChanged(nameof(viewModel.AllAvailableDrainages));
    }

    public async Task LoadElectricalComponentsToViewModelAsync(ProjectViewModel viewModel)
    {
        var electricalComponent = await _standService.LoadAllAvailableElectricalComponentsAsync();
        viewModel.AllAvailableElectricalComponents =
            new ObservableCollection<FormedElectricalComponent>(electricalComponent);
        viewModel.OnPropertyChanged(nameof(viewModel.AllAvailableElectricalComponents));
    }

    public async Task LoadAdditionalEquipsToViewModelAsync(ProjectViewModel viewModel)
    {
        var additionalEquips = await _standService.LoadAllAvailableAdditionalEquipsAsync();
        viewModel.AllAvailableAdditionalEquips = new ObservableCollection<FormedAdditionalEquip>(additionalEquips);
        viewModel.OnPropertyChanged(nameof(viewModel.AllAvailableAdditionalEquips));
    }

    public async Task LoadAllAvailDataToViewModelAsync(ProjectViewModel viewModel)
    {
        await LoadFramesToViewModelAsync(viewModel);
        await LoadDrainagesToViewModelAsync(viewModel);
        await LoadElectricalComponentsToViewModelAsync(viewModel);
        await LoadAdditionalEquipsToViewModelAsync(viewModel);
    }
}
