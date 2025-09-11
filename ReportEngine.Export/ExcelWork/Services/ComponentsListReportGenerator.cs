using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs;
using System.Diagnostics;

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

        var project = await _projectInfoRepository.GetByIdAsync(projectId);

        var templatePath = DirectoryHelper.GetReportsTemplatePath("Ведомость комплектующих");
        var fileName = "Ведомость комплектующих___" + DateTime.Now.ToString("yy-MM-dd___HH-mm-ss") + ".xlsx";

        var savePath = SettingsManager.GetReportDirectory();
        var fullSavePath = Path.Combine(savePath, fileName);

        using (var wb = new XLWorkbook(templatePath))
        {
            wb.Worksheets.ToList().ForEach(ws => ws.Delete());

            foreach (var stand in project.Stands)
            {
                var ws = wb.Worksheets.Add($"Стенд_{stand.KKSCode}");
                CreateTableHeader(ws, stand);


                ws.Columns().AdjustToContents();
                ws.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            Debug.WriteLine("Отчёт сохранён: " + fullSavePath);
            wb.SaveAs(fullSavePath);
        }


    }

    private void CreateTableHeader(IXLWorksheet ws, Stand stand)
    {

        ws.Cell("B1").Value = $"Код-KKS: {stand.KKSCode}";

        var standNameArea = ws.Range("C1:D1").Merge();
        standNameArea.Value = $"Наименование: {stand.Design}";

        var reportNameArea = ws.Range("B2:D2").Merge();
        reportNameArea.Value = "Сводная ведомость комплектующих";

        ws.Cell("B3").Value = "Наименование";
        ws.Cell("C3").Value = "Ед. изм";
        ws.Cell("D3").Value = "Кол.";

        var headerRange = ws.Range("B1:D3");

        headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);

        headerRange.Style.Font.SetBold();
    }

    private void FillStandsData(IXLWorksheet ws, Stand stand)
    {
        var row = 4;

        foreach (var obvyazka in stand.ObvyazkiInStand) ws.Cell("B5").Value = "Сортамент труб";
    }
}