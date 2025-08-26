using ReportEngine.Export.ExcelWork.Enums;

namespace ReportEngine.Export.ExcelWork.Services.Interfaces;

public interface IReportService
{
    Task GenerateReportAsync(ReportType generatorType, int projectId);
}