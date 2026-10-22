using ReportEngine.Import.Abstractions;
using ReportEngine.Import.Services.Excel;
using ReportEngine.Import.Services.Stand;

namespace ReportEngine.Tests.Services;

[TestClass]
public class ComponentParserTests
{
    private const string filename = "C:\\Work\\Тестовая папка для отчётов\\Test.xlsx";
    private IFileService _fileService;
    private ExcelAddressingService _excelAddressingService;
    private ComponentParser _parser;
    
    [TestInitialize]
    public void Initialize()
    {
        _fileService = new FileService();
        _excelAddressingService = new ExcelAddressingService();
        _parser = new ComponentParser(_excelAddressingService);
    }

    [TestMethod]
    public void ParseTest()
    {
        var workbook = _fileService.OpenFileAsXlw(filename);
        
        var actual = _parser.Parse(workbook.Worksheets);

        foreach (var act in actual)
        {
            Console.WriteLine(act.Material);
        }

    }
}