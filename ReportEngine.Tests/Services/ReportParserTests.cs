using ReportEngine.Import.Abstractions;
using ReportEngine.Import.Services.Excel;
using ReportEngine.Import.Services.Project;
using ReportEngine.Import.Services.Report;

namespace ReportEngine.Tests.Services;

[TestClass]
public class ReportParserTests
{
    private const string filename = "C:\\Work\\Тестовая папка для отчётов\\Test.xlsx";
    private IFileService _fileService;
    private ExcelAddressingService _excelAddressingService;
    
    [TestInitialize]
    public void Initialize()
    {
        _fileService = new FileService();
        _excelAddressingService = new ExcelAddressingService();
    }
    
    [TestMethod]
    [DataRow(8)]
    public void FindStartRow_Test(int expected)
    {
       
    }
}