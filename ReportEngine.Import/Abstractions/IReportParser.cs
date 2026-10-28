using ReportEngine.Import.Dto;

namespace ReportEngine.Import.Abstractions;

public interface IReportParser
{
    Task<ReportDto> Parse(string fileName);
}