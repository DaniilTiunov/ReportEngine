using ReportEngine.Export.ExcelWork.Enums;

namespace ReportEngine.Export.ExcelWork.Services.Interfaces;

public interface IReportGenerator
{
    ReportType Type { get; }
    Task GenerateAsync(int projectId);
}