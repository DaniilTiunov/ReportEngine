using ClosedXML.Excel;

namespace ReportEngine.Import.Abstractions;

public interface IFileService
{
    Task<XLWorkbook> OpenFileAsXlwAsync(string filename);
}