using ReportEngine.App.ViewModels;
using ReportEngine.Import.Dto;

namespace ReportEngine.App.Services.Import;

public interface IImportService
{
    void Import(ReportDto report, ProjectViewModel projectViewModel);
}