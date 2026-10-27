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
    private FrameParser _frameParser;

    private ObvParser _obvParser;

    [TestInitialize]
    public void Initialize()
    {
        _fileService = new FileService();
        _excelAddressingService = new ExcelAddressingService();
        _parser = new ComponentParser(_excelAddressingService);
        _frameParser = new FrameParser(_excelAddressingService);

        _obvParser = new ObvParser(_excelAddressingService, _parser);
    }

    [TestMethod]
    public void ParseTest()
    {

        var expected = 2;

        var workbook = _fileService.OpenFileAsXlw(filename);

        var actual = _obvParser.Parse(workbook.Worksheets.Last());

        Assert.HasCount(expected, actual);

        foreach (var act in actual)
        {
            Console.WriteLine($"{act.Number}:{act.ObvType}");
        }



    }
}