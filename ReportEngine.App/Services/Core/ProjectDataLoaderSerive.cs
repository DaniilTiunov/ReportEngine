using System.Collections.ObjectModel;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.Services.Core;

public class ProjectDataLoaderSerive : IProjectDataLoaderService
{
    private readonly IStandService _standService;

    public ProjectDataLoaderSerive(IStandService standService)
    {
        _standService = standService;
    }

    public async Task LoadFramesToViewModel(ProjectViewModel viewModel)
    {
        var frames = await _standService.LoadAllAvailableFrameAsync();
        viewModel.AllAvailableFrames = new ObservableCollection<FormedFrame>(frames);
        viewModel.OnPropertyChanged(nameof(viewModel.AllAvailableFrames));
    }

    public async Task LoadDrainagesToViewModel(ProjectViewModel viewModel)
    {
        var drainages = await _standService.LoadAllAvailableDrainagesAsync();
        viewModel.AllAvailableDrainages = new ObservableCollection<FormedDrainage>(drainages);
        viewModel.OnPropertyChanged(nameof(viewModel.AllAvailableDrainages));
    }

    public async Task LoadElectricalComponentsToViewModel(ProjectViewModel viewModel)
    {
        var electricalComponent = await _standService.LoadAllAvailableElectricalComponentsAsync();
        viewModel.AllAvailableElectricalComponents =
            new ObservableCollection<FormedElectricalComponent>(electricalComponent);
        viewModel.OnPropertyChanged(nameof(viewModel.AllAvailableElectricalComponents));
    }

    public async Task LoadAdditionalEquipsToViewModel(ProjectViewModel viewModel)
    {
        var additionalEquips = await _standService.LoadAllAvailableAdditionalEquipsAsync();
        viewModel.AllAvailableAdditionalEquips = new ObservableCollection<FormedAdditionalEquip>(additionalEquips);
        viewModel.OnPropertyChanged(nameof(viewModel.AllAvailableAdditionalEquips));
    }

    public async Task LoadAllAvailDataToViewModel(ProjectViewModel viewModel)
    {
        await LoadFramesToViewModel(viewModel);
        await LoadDrainagesToViewModel(viewModel);
        await LoadElectricalComponentsToViewModel(viewModel);
        await LoadAdditionalEquipsToViewModel(viewModel);
    }
}