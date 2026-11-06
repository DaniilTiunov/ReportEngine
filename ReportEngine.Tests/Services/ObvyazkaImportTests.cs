using ClosedXML.Excel;
using Mapster;
using ReportEngine.App.Mapping.Import;
using ReportEngine.Domain.Entities;
using ReportEngine.Import.Services.Excel;
using ReportEngine.Import.Services.Stand;

namespace ReportEngine.Tests.Services;

[TestClass]
public class ObvyazkaImportTests
{
    [TestMethod]
    public void ParseAndMap_PreservesCatalogIdSeparatelyFromNumberAndInstanceId()
    {
        using var workbook = new XLWorkbook();
        var worksheet = CreateWorksheet(workbook);
        worksheet.Cell("K46").Value = 42;
        worksheet.Cell("K47").Value = "Не ID";

        var dto = CreateParser().Parse(worksheet).Single();
        var config = new TypeAdapterConfig();
        new ObvyazkiMapper().Register(config);
        var entity = dto.Adapt<ObvyazkaInStand>(config);

        Assert.AreEqual(42, dto.ObvyazkaId);
        Assert.AreEqual(42, entity.ObvyazkaId);
        Assert.AreEqual(7, entity.NN);
        Assert.AreEqual(0, entity.Id);
        Assert.AreEqual("Тип обвязки", entity.ObvyazkaName);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("0")]
    [DataRow("-1")]
    [DataRow("abc")]
    [DataRow("1.5")]
    [DataRow("2147483648")]
    public void Parse_InvalidCatalogId_ReportsWorksheetAndCell(string value)
    {
        using var workbook = new XLWorkbook();
        var worksheet = CreateWorksheet(workbook);
        worksheet.Cell("K46").Value = value;

        var exception = Assert.ThrowsExactly<FormatException>(() => CreateParser().Parse(worksheet));

        StringAssert.Contains(exception.Message, "Стенд 1");
        StringAssert.Contains(exception.Message, "K46");
    }

    [TestMethod]
    public void Parse_MultipleObvyazki_ReadsIdFromEachHeaderRow()
    {
        using var workbook = new XLWorkbook();
        var worksheet = CreateWorksheet(workbook);
        worksheet.Cell("K46").Value = 42;
        worksheet.Cell("K58").Value = 84;
        worksheet.Cell("C59").Value = 8;
        worksheet.Cell("G59").Value = "Вторая обвязка";
        worksheet.Cell("B68").Value = "Датчики";

        var obvyazki = CreateParser().Parse(worksheet);

        CollectionAssert.AreEqual(new[] { 42, 84 }, obvyazki.Select(obv => obv.ObvyazkaId).ToArray());
    }

    private static IXLWorksheet CreateWorksheet(XLWorkbook workbook)
    {
        var worksheet = workbook.AddWorksheet("Стенд 1");
        worksheet.Cell("B45").Value = "ОБВЯЗКИ СТЕНДА";
        worksheet.Cell("C47").Value = 7;
        worksheet.Cell("G47").Value = "Тип обвязки";
        worksheet.Cell("B56").Value = "Датчики";
        return worksheet;
    }

    private static ObvParser CreateParser()
    {
        var addressing = new ExcelAddressingService();
        return new ObvParser(addressing, new ComponentParser(addressing));
    }
}
