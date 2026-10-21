using ReportEngine.Import.Services;
using ReportEngine.Import.Services.Excel;

namespace ReportEngine.Tests.Services;

[TestClass]
public class FileServiceTests
{
    private FileService _fileService;

    [TestInitialize]
    public void Setup()
    {
        _fileService = new FileService();
    }
    
    [TestMethod]
    [DataRow("C:\\Work\\Тестовая папка для отчётов\\Сводная ведомость 16-10-26___11-39-11.xlsx")]
    public void OpenFileAsXlw_Test(string filename)
    {
        var fileContent = _fileService.OpenFileAsXlw(filename);
        Console.WriteLine(fileContent.Worksheets
            .First()
            .Row(2)
            .Cell(1)
            .Value
            .ToString());
    }
}