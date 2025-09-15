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
                await FillWorksheetTable(ws, stand);


                ws.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }


            var savePath = SettingsManager.GetReportDirectory();
            var fileName = "Ведомость комплектующих___" + DateTime.Now.ToString("dd-MM-yy___HH-mm-ss") + ".xlsx";


            var fullSavePath = Path.Combine(savePath, fileName);

            Debug.WriteLine("Отчёт сохранён: " + fullSavePath);
            wb.SaveAs(fullSavePath);
        }
    }

    //Создает общий заголовок таблицы на листе
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

    //Заполняет таблицу на листе
    private async Task FillWorksheetTable(IXLWorksheet ws, Stand stand)
    {
        var activeRow = 4;


        //Формирование списка труб

        var pipesList = stand.ObvyazkiInStand
            .Select(obv => new
            {
                name = obv.MaterialLine,
                units = obv.MaterialLineMeasure,
                length = obv.MaterialLineCount
            })
            .GroupBy(pipe => pipe.name)
            .Select(group => (
                name: group.Key ?? "",
                unit: group.First().units ?? "",
                quantity: group.Sum(pipe => pipe.length) ?? 0f
            ))
            .ToList();

        if (pipesList.Count > 0)
        {
            activeRow = CreateSubheaderOnWorksheet(activeRow, "Сортамент труб", ws);
            activeRow = FillSubtableData(activeRow, pipesList, ws);
        }


        //Формирование списка арматуры

        var armaturesList = stand.ObvyazkiInStand
            .Select(obv => new
            {
                name = obv.Armature,
                quantity = obv.ArmatureCount
            })
            .GroupBy(arm => arm.name)
            .Select(group => (
                name: group.Key ?? "",
                unit: "м",
                quantity: group.Sum(arm => arm.quantity) ?? 0f
            ))
            .ToList();

        if (armaturesList.Count > 0)
        {
            activeRow = CreateSubheaderOnWorksheet(activeRow, "Арматура", ws);
            activeRow = FillSubtableData(activeRow, armaturesList, ws);
        }


        //Формирование списка тройников и КМЧ

        var treeList = stand.ObvyazkiInStand
            .Select(obv => new
            {
                name = obv.TreeSocket,
                quantity = obv.TreeSocketCount
            })
            .GroupBy(item => item.name)
            .Select(group => (
                name: group.Key ?? "",
                unit: "шт",
                quantity: group.Sum(item => item.quantity)
            ))
            .ToList();

        var kmchList = stand.ObvyazkiInStand
            .Select(obv => new
            {
                name = obv.KMCH,
                quantity = obv.KMCHCount
            })
            .GroupBy(item => item.name)
            .Select(group => (
                name: group.Key ?? "",
                unit: "шт",
                quantity: group.Sum(item => item.quantity) ?? 0f
            ))
            .ToList();


        if (treeList.Count > 0 || kmchList.Count > 0)
            activeRow = CreateSubheaderOnWorksheet(activeRow, "Тройники и КМЧ", ws);

        if (treeList.Count > 0) activeRow = FillSubtableData(activeRow, treeList, ws);

        if (kmchList.Count > 0) activeRow = FillSubtableData(activeRow, kmchList, ws);


        //Формирование списка рамных комплектующих

        var framesCollection = await _projectInfoRepository.GetAllFramesInStandAsync(stand.Id);

        var framesList = framesCollection
            .SelectMany(fr => fr.Frame.Components.Select(comp => new
            {
                name = comp.ComponentType,
                unit = comp.Measure,
                quantity = comp.Count
            }))
            .GroupBy(frameComp => frameComp.name)
            .Select(group => (
                name: group.Key ?? "",
                unit: group.First().unit ?? "",
                quantity: (float)group.Sum(frameComp => frameComp.quantity)
            ))
            .ToList();

        if (framesList.Count > 0)
        {
            activeRow = CreateSubheaderOnWorksheet(activeRow, "Рамные комплектующие", ws);
            activeRow = FillSubtableData(activeRow, framesList, ws);
        }

        //формирование списка кронштейнов

        var drainageHolders = stand.StandDrainages.SelectMany(drainage => drainage.Drainage.Purposes)
            .Where(purpose => purpose.Purpose.Contains("Кронштейн"));

        var boxesHolders = stand.StandAdditionalEquips.SelectMany(equip => equip.AdditionalEquip.Purposes)
            .Where(purpose => purpose.Purpose.Contains("Кронштейн"));

        var sensorsHolders = stand.StandElectricalComponent.SelectMany(comp => comp.ElectricalComponent.Purposes)
            .Where(purpose => purpose.Purpose.Contains("Кронштейн"));


        if (drainageHolders.Count() > 0 || boxesHolders.Count() > 0 || sensorsHolders.Any())
            activeRow = CreateSubheaderOnWorksheet(activeRow, "Кронштейны", ws);

        if (drainageHolders.Count() > 0)
        {
            var drainageHoldersList = drainageHolders
                .GroupBy(purpose => purpose.Purpose)
                .Select(group => (
                    name: group.Key ?? "",
                    unit: "шт",
                    quantity: (float)group.Count()
                ))
                .ToList();

            activeRow = FillSubtableData(activeRow, drainageHoldersList, ws);
        }
    }

    //создает заголовок для подтаблицы и возвращает следующую строку
    private int CreateSubheaderOnWorksheet(int row, string title, IXLWorksheet ws)
    {
        var subHeaderRange = ws.Range($"B{row}:D{row}");
        subHeaderRange.Merge();
        subHeaderRange.Value = title;
        subHeaderRange.Style.Font.SetFontSize(10);
        subHeaderRange.Style.Font.SetBold();

        row++;
        return row;
    }

    //Заполняет подтаблицу и возвращает следующую строку
    private int FillSubtableData(int startRow, List<(string name, string unit, float quantity)> items, IXLWorksheet ws)
    {
        var currentRow = startRow;
        foreach (var item in items)
        {
            ws.Cell($"B{currentRow}").Value = item.name;
            ws.Cell($"C{currentRow}").Value = item.unit;
            ws.Cell($"D{currentRow}").Value = item.quantity;
            currentRow++;
        }

        return currentRow;
    }
}