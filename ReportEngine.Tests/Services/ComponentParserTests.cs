using ReportEngine.Import.Abstractions;
using ReportEngine.Import.Services.Excel;
using ReportEngine.Import.Services.Stand;

namespace ReportEngine.Tests.Services;

[TestClass]
public class ComponentParserTests
{
    private const string filename = "C:\\Work\\Тестовая папка для отчётов\\Test.xlsx";

    private const string containerFileName = "C:\\Work\\Тестовая папка для отчётов\\ContainerTest.xlsx";
    private IFileService _fileService;
    private ExcelAddressingService _excelAddressingService;
    private ComponentParser _parser;
    private FrameParser _frameParser;
    private ContainerParser _containerParser;
    private ObvParser _obvParser;

    [TestInitialize]
    public void Initialize()
    {
        _fileService = new FileService();
        _excelAddressingService = new ExcelAddressingService();
        _parser = new ComponentParser(_excelAddressingService);
        _frameParser = new FrameParser(_excelAddressingService);

        _obvParser = new ObvParser(_excelAddressingService, _parser);

        _containerParser = new ContainerParser(_excelAddressingService);
    }

    [TestMethod]
    public async Task ParseTest()
    {
        var workbook = await _fileService.OpenFileAsXlwAsync(containerFileName);

        var firstSheet = workbook.Worksheets.First();

        var results = _containerParser.Parse(firstSheet);



        var sraka = 5;

    }
}