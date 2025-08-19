using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Services.Interfaces;

public interface IProjectDataLoaderService
{
    Task LoadFramesToViewModel(ProjectViewModel viewModel);
    Task LoadDrainagesToViewModel(ProjectViewModel viewModel);
    Task LoadElectricalComponentsToViewModel(ProjectViewModel viewModel);
    Task LoadAdditionalEquipsToViewModel(ProjectViewModel viewModel);
    Task LoadAllAvailDataToViewModel(ProjectViewModel viewModel);
}