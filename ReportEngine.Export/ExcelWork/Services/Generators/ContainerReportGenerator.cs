using System.Diagnostics;
using ClosedXML.Excel;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.IniHeleprs;

namespace ReportEngine.Export.ExcelWork.Services.Generators;

public class ContainerReportGenerator : IReportGenerator
{
    private readonly IProjectInfoRepository _projectInfoRepository;

    public ContainerReportGenerator(
        IProjectInfoRepository projectInfoRepository)
    {
        _projectInfoRepository = projectInfoRepository;
    }

    public ReportType Type => ReportType.ContainerReport;

    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);

        using (var wb = new XLWorkbook())
        {
            var ws = wb.Worksheets.Add("MainSheet");

            CreateWorksheetTableHeader(ws);

            ws.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cells().Style.Alignment.WrapText = true;
            ws.Columns().AdjustToContents();

            var savePath = SettingsManager.GetReportDirectory();

            var fileName = ExcelReportHelper.CreateReportName("Тара", "xlsx");
            var fullSavePath = Path.Combine(savePath, fileName);

            Debug.WriteLine("Отчёт сохранён: " + fullSavePath);
            wb.SaveAs(fullSavePath);
        }
    }

    private void CreateWorksheetTableHeader(IXLWorksheet ws)
    {
        var headerRange = ws.Range("A1:I1");

        headerRange.Cell(1, 1).Value = "№ места";
        headerRange.Cell(1, 2).Value = "№ места в ящике";
        headerRange.Cell(1, 3).Value = "Наименование оборудования и комплектующих";
        headerRange.Cell(1, 4).Value = "Серийный №";
        headerRange.Cell(1, 5).Value = "Код KKS";
        headerRange.Cell(1, 6).Value = "Количество";
        headerRange.Cell(1, 7).Value = "Ширина рамы, мм";
        headerRange.Cell(1, 8).Value = "Масса, кг";
        headerRange.Cell(1, 9).Value = "Упаковка";

        headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);

        headerRange.Style.Font.SetBold();
    }
}
