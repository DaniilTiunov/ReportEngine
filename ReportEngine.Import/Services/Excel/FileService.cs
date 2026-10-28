using ClosedXML.Excel;
using ReportEngine.Import.Abstractions;

namespace ReportEngine.Import.Services.Excel;

public class FileService : IFileService
{
    public async Task<XLWorkbook> OpenFileAsXlwAsync(string filename)
    {
        return await Task.Run(() => new XLWorkbook(filename));
    }
}