using ClosedXML.Excel;
using ReportEngine.Import.Abstractions;

namespace ReportEngine.Import.Services.Excel;

public class FileService : IFileService
{
    public XLWorkbook OpenFileAsXlw(string filename)
    {
        return new XLWorkbook(filename);
    }
}