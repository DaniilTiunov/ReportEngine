using ReportEngine.App.ViewModels;
using ReportEngine.Import.Dto;

namespace ReportEngine.App.Services.Import;

public class ImportService : IImportService
{
    public void Import(ReportDto report, ProjectViewModel projectViewModel)
    {
        projectViewModel.ResetProject();
        
        ImportProjectInfo(report.ProjectInfo,  projectViewModel);
    }
    
    private void ImportProjectInfo(
        ProjectInfoDto info,
        ProjectViewModel viewModel)
    {
        viewModel.CurrentProjectModel.Company = info.Company;
        viewModel.CurrentProjectModel.Object = info.Object;
    }
}