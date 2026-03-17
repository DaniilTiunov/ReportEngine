using ReportEngine.Domain.Entities;
using ReportEngine.Export.ExcelWork.Enums;

namespace ReportEngine.Export.ExcelWork.Services.Interfaces;

public interface IReportService
{
    Task GenerateReportAsync(ReportType generatorType, int projectId);
    Task GenerateReportAsync(ReportType generatorType, int projectId, List<Stand>? selectedStands = null);
}
