using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;

namespace ReportEngine.Export.ExcelWork.Services.Generators;

public class SummaryReportGenerator : ComponentListReportGenerator, IReportGenerator
{
    private readonly IProjectInfoRepository _projectInfoRepository;

    public SummaryReportGenerator(IProjectInfoRepository projectInfoRepository)
        : base(projectInfoRepository)
    {
        _projectInfoRepository = projectInfoRepository;
    }

    public ReportType Type => ReportType.SummaryReport;

    public new async Task GenerateAsync(int projectId)
    {
        await base.GenerateAsync(projectId);
    }

    protected override void CreateStandTableHeader(IXLWorksheet ws, Stand stand, XLAlignmentHorizontalValues alignment)
    {
        ws.Cell("A1").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        var exportDays = ws.Cell("A2");
        exportDays.Value = "Срок\nпоставки\nкомплектующих";
        exportDays.Style.Font.Bold = true;
        exportDays.Style.Alignment.Horizontal = alignment;
        exportDays.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        exportDays.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);

        base.CreateStandTableHeader(ws, stand, alignment);

        ws.Cell("B1").Value = "";
        ws.Range("B2:F2").Merge();
        ws.Range("B2:F2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        ws.Range("B2:F2").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);

        ws.Range("C1:F1").Merge();
        ws.Range("C1:F1").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        ws.Range("C1:F1").Value = $"Код-KKS: {stand.KKSCode}";

        ws.Cells("E3").Value = "Цена за\nшт.";
        ws.Cells("E3").Style.Font.Bold = true;
        ws.Cells("E3").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        ws.Cells("F3").Value = "Цена руб.";
        ws.Cells("F3").Style.Font.Bold = true;
        ws.Cells("F3").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
    }

    protected override string GetReportFileName()
    {
        return "Сводная ведомость___" +
               DateTime.Now.ToString("dd-MM-yy___HH-mm-ss") +
               ".xlsx";
    }
}