using ClosedXML.Excel;
using ReportEngine.Import.Dto;

namespace ReportEngine.Import.Abstractions;

public interface IStandParser
{
    List<StandDto> Parse(IXLWorksheets worksheets);
}