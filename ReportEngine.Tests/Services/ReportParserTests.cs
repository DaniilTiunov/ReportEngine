using ReportEngine.Import.Abstractions;
using ReportEngine.Import.Services.Excel;
using ReportEngine.Import.Services.Project;
using ReportEngine.Import.Services.Report;

namespace ReportEngine.Tests.Services;

[TestClass]
public class ReportParserTests
{
    private IReportParser _reportParser = null!;

    [TestInitialize]
    public void Setup()
    {
        IFileService fileService = new FileService();
        IProjectInfoParser projectInfoParser = new ProjectInfoParser();
        
        _reportParser = new ReportParser(fileService, projectInfoParser);
    }

    [TestMethod]
    [DataRow("C:\\Work\\Тестовая папка для отчётов\\Test.xlsx", "ЗАО БЕЩЁКИ", "ПОМОЙНАЯ СКВАЖИНА 228 ОЧЕРЕДЬ")]
    public void Parse_Test(
        string fileName, 
        string expectedCustomer, 
        string expectedProject)
    {
        var report = _reportParser.Parse(fileName);
        Assert.IsNotNull(report);
        
        Assert.AreEqual(expectedCustomer, report.ProjectInfo.Company, "Имя заказчика не совпадает");
        Assert.AreEqual(expectedCustomer, report.ProjectInfo.Company, "Имя объекта не совпадает");
        
        Console.Write($"Заказчик: {report.ProjectInfo.Company}, Объект: {report.ProjectInfo.Object}");
    }
}