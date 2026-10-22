using ReportEngine.App.ViewModels;
using ReportEngine.Import.Dto;

namespace ReportEngine.App.Services.Import;

public interface IImportService
{
    Task ImportAsync(ReportDto report, ProjectViewModel projectViewModel);
}
