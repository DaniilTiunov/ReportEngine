using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Services.Interfaces;

public interface IProjectDataLoaderService
{
    Task LoadAllProjectStandsAsync(int projectId, ProjectViewModel viewModel);
    Task LoadFramesToViewModelAsync(ProjectViewModel viewModel);
    Task LoadDrainagesToViewModelAsync(ProjectViewModel viewModel);
    Task LoadElectricalComponentsToViewModelAsync(ProjectViewModel viewModel);
    Task LoadAdditionalEquipsToViewModelAsync(ProjectViewModel viewModel);
    Task LoadAllAvailDataToViewModelAsync(ProjectViewModel viewModel);
}
