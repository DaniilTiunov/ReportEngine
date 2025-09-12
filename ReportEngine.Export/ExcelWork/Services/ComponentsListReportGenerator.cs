using System.Diagnostics;
using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.IniHeleprs;

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

        using (var wb = new XLWorkbook())
        {
            wb.Worksheets.ToList().ForEach(ws => ws.Delete());

            foreach (var stand in project.Stands)
            {
                var ws = wb.Worksheets.Add($"Стенд_{stand.KKSCode}");
                CreateWorksheetTableHeader(ws, stand);
                FillWorksheetTable(ws, stand);


                ws.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }


            var savePath = SettingsManager.GetReportDirectory();
            var fileName = "Ведомость комплектующих___" + DateTime.Now.ToString("yy-MM-dd___HH-mm-ss") + ".xlsx";


            var fullSavePath = Path.Combine(savePath, fileName);

            Debug.WriteLine("Отчёт сохранён: " + fullSavePath);
            wb.SaveAs(fullSavePath);
        }
    }

    private void CreateWorksheetTableHeader(IXLWorksheet ws, Stand stand)
    {
        ws.Cell("B1").Value = $"Код-KKS: {stand.KKSCode}";
        ws.Cell("C1").Value = $"Наименование: {stand.Design}";

        ws.Cell("B2").Value = "Сводная ведомость комплектующих";

        ws.Columns().AdjustToContents();


        ws.Cell("B3").Value = "Наименование";
        ws.Cell("C3").Value = "Ед. изм";
        ws.Cell("D3").Value = "Кол.";


        ws.Columns().AdjustToContents();

        var headerRange = ws.Range("B1:D3");

        headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);

        headerRange.Style.Font.SetBold();


        ws.Range("B2:D2").Merge();
        ws.Range("C1:D1").Merge();
    }

    private void FillWorksheetTable(IXLWorksheet ws, Stand stand)
    {
        var activeRow = 4;


        // Подзаголовок для сортамента труб
        var subHeaderRange = ws.Range($"A{activeRow}:D{activeRow}");
        subHeaderRange.Merge();
        subHeaderRange.Value = "Сортамент труб";
        subHeaderRange.Style.Font.SetFontSize(10);
        subHeaderRange.Style.Font.SetBold();

        activeRow++;


        //формирование списка труб

        //худо-бедно работает, но надо поправить на нормальный LINQ

        var standPipes = stand.ObvyazkiInStand
            .GroupBy(obv => obv.MaterialLine)
            .Select(group => new
            {
                Name = group.Key,
                Unit = "м", //поправить на реальные ед. изм
                LengthSum = group.Sum(pipe => pipe.MaterialLineCount)
            });


        foreach (var pipe in standPipes)
        {
            ws.Cell($"B{activeRow}").Value = pipe.Name;
            ws.Cell($"C{activeRow}").Value = pipe.Unit;
            ;
            ws.Cell($"D{activeRow}").Value = pipe.LengthSum;
            activeRow++;
        }
    }
}