using System.Diagnostics;
using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.Directory;

namespace ReportEngine.Export.ExcelWork.Services;

public class ComponentsListReportGenerator : IReportGenerator
{
    private readonly IProjectInfoRepository _projectInfoRepository;

    public ComponentsListReportGenerator(IProjectInfoRepository projectInfoRepository)
    {
        _projectInfoRepository = projectInfoRepository;
    }

    public ReportType Type => ReportType.ComponentsListReport;

    public async Task GenerateAsync(int projectId)
    {
        try
        {
            var project = await _projectInfoRepository.GetByIdAsync(projectId);
            var templatePath = DirectoryHelper.GetReportsTemplatePath("Ведомость комплектующих");
            var fileName = "Ведомость комплектующих.xlsx";
            var savePath = DirectoryHelper.GetReportSavePath(fileName);

            using (var wb = new XLWorkbook(templatePath))
            {
                wb.Worksheets.ToList().ForEach(ws => ws.Delete());

                foreach (var stand in project.Stands)
                {
                    var ws = wb.Worksheets.Add($"Стенд_{stand.KKSCode}");
                    DrawReportStandsHeader(ws, stand);
                    FillStandsData(ws, stand);
                }

                wb.SaveAs(savePath);
            }
        }
        catch (Exception e)
        {
            Debug.Write(e.Message);
        }
    }

    private void DrawReportStandsHeader(IXLWorksheet ws, Stand stand)
    {
        ws.Cell("B1").Value = $"Код-KKS: {stand.KKSCode}";
        ws.Cell("B1").Style
            .Border.SetTopBorder(XLBorderStyleValues.Medium)
            .Border.SetRightBorder(XLBorderStyleValues.Medium)
            .Border.SetBottomBorder(XLBorderStyleValues.Medium)
            .Border.SetLeftBorder(XLBorderStyleValues.Medium);
        ws.Cell("C1").Value = $"Наименование: {stand.Design}";

        ws.Cell("B2").Value = "Сводная ведомость комплектующих";
        ws.Cell("B2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell("B2").Style
            .Border.SetTopBorder(XLBorderStyleValues.Medium)
            .Border.SetRightBorder(XLBorderStyleValues.Medium)
            .Border.SetBottomBorder(XLBorderStyleValues.Medium)
            .Border.SetLeftBorder(XLBorderStyleValues.Medium);

        ws.Cell("B4").Value = "Наименование";
        ws.Cell("C4").Value = "Ед. изм.";
        ws.Cell("D4").Value = "Кол.";

        var headerStyle = ws.Range("B4:D4").Style;
        headerStyle.Font.Bold = true;
        headerStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        ws.Column("B").Width = 60;
        ws.Column("C").Width = 15;
        ws.Column("D").Width = 10;
    }

    private void FillStandsData(IXLWorksheet ws, Stand stand)
    {
        var row = 4;

        foreach (var obvyazka in stand.ObvyazkiInStand) ws.Cell("B5").Value = "Сортамент труб";
    }
}