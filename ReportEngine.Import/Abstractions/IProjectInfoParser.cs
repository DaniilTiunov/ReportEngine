using ClosedXML.Excel;
using ReportEngine.Import.Dto;

namespace ReportEngine.Import.Abstractions;

public interface IProjectInfoParser
{
    ProjectInfoDto Parse(IXLWorksheet worksheet);
}