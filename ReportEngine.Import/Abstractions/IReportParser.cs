using ReportEngine.Import.Dto;

namespace ReportEngine.Import.Abstractions;

public interface IReportParser
{
    ReportDto Parse(string fileName);
}