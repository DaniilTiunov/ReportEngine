using ClosedXML.Excel;

namespace ReportEngine.Import.Abstractions;

public interface IFileService
{
    XLWorkbook OpenFileAsXlw(string filename);
}