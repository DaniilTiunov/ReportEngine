using System.Diagnostics;
using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Generators.DTO;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Export.Mapping;
using ReportEngine.Shared.Config.IniHeleprs;

namespace ReportEngine.Export.ExcelWork.Services.Generators;

public class SummaryReportGenerator : IReportGenerator
{
    private readonly IProjectInfoRepository _projectInfoRepository;

    public SummaryReportGenerator(IProjectInfoRepository projectInfoRepository)
    {
        _projectInfoRepository = projectInfoRepository;
    }

    public ReportType Type => ReportType.SummaryReport;

    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);

        using (var wb = new XLWorkbook())
        {
            var standNumber = 1;

            ;

            var zhozh = project.Stands;

            ;
            //заполняем листы по стендам
            foreach (var stand in project.Stands)
            {
                var ws = wb.Worksheets.Add($"{standNumber}");

                CreateStandTableHeader(ws, stand, XLAlignmentHorizontalValues.Center);
                FillStandTable(ws, stand);

                ws.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();

                standNumber++;
            }


            //заполняем сводную ведомость
            var lastSheet = wb.Worksheets.Add("Сводная заявка");

            CreateCommonListTableHeader(lastSheet, project, XLAlignmentHorizontalValues.Center);
            FillCommonListTable(lastSheet, project);

            //применяем оформление ко всему документу
            foreach (var ws in wb.Worksheets)
            {
                ws.Cells().Style.Font.FontName = "Times New Roman";
                ws.Cells().Style.Alignment.WrapText = true;
                ws.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();
            }

            var savePath = SettingsManager.GetReportDirectory();
            var fileName = ExcelReportHelper.CreateReportName("Ведомость комплектующих", "xlsx");
            var fullSavePath = Path.Combine(savePath, fileName);

            Debug.WriteLine("Отчёт сохранён: " + fullSavePath);
            wb.SaveAs(fullSavePath);
        }
    }


    #region Вспомогательные

    private StandsReportData GetStandReportData(Stand stand)
    {
        const string dbErrorString = "Ошибка получения данных из БД";

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
                name: group.Key ?? dbErrorString,
                unit: group.First().units ?? dbErrorString,
                quantity: group.Sum(pipe => pipe.length).ToString() ?? dbErrorString
            ))
            .ToList();
        //Формирование списка арматуры
        var armaturesList = stand.ObvyazkiInStand
            .Select(obv => new
            {
                name = obv.Armature,
                units = obv.ArmatureMeasure,
                quantity = obv.ArmatureCount
            })
            .GroupBy(arm => arm.name)
            .Select(group => (
                name: group.Key ?? dbErrorString,
                unit: group.First().units ?? dbErrorString,
                quantity: group.Sum(arm => arm.quantity).ToString() ?? dbErrorString
            ))
            .ToList();
        //Формирование списка тройников и КМЧ
        var treeList = stand.ObvyazkiInStand
            .Select(obv => new
            {
                name = obv.TreeSocket,
                units = obv.TreeSocketMaterialMeasure,
                quantity = obv.TreeSocketCount
            })
            .GroupBy(item => item.name)
            .Select(group => (
                name: group.Key ?? dbErrorString,
                unit: group.First().units ?? dbErrorString,
                quantity: group.Sum(item => item.quantity).ToString() ?? dbErrorString
            ))
            .ToList();

        var kmchList = stand.ObvyazkiInStand
            .Select(obv => new
            {
                name = obv.KMCH,
                units = obv.KMCHMeasure,
                quantity = obv.KMCHCount
            })
            .GroupBy(item => item.name)
            .Select(group => (
                name: group.Key ?? dbErrorString,
                unit: group.First().units ?? dbErrorString,
                quantity: group.Sum(item => item.quantity).ToString() ?? dbErrorString
            ))
            .ToList();
        //формирование дренажа
        var drainageParts = stand.StandDrainages
            .SelectMany(drainage => drainage.Drainage.Purposes)
            .GroupBy(purpose => purpose.Material)
            .Select(group => (
                name: group.Key ?? dbErrorString,
                unit: group.First().Measure ?? dbErrorString,
                quantity: group.Sum(groupElement => groupElement.Quantity).ToString() ?? dbErrorString
            ))
            .ToList();
        //Формирование списка рамных комплектующих
        var framesList = stand.StandFrames
            .SelectMany(fr => fr.Frame.Components)
            .Select(comp => new
            {
                name = comp.ComponentName,
                unit = comp.Measure,
                quantity = comp.Count
            })
            .GroupBy(frameComp => frameComp.name)
            .Select(group => (
                name: group.Key ?? dbErrorString,
                unit: group.First().unit ?? dbErrorString,
                quantity: group.Sum(frameComp => frameComp.quantity).ToString() ?? dbErrorString
            ))
            .ToList();
        //сомнительная хрень, хз что брать за источник информации
        //формирование списка кронштейнов
        var sensorsHolders = stand.StandAdditionalEquips
            .SelectMany(equip => equip.AdditionalEquip.Purposes)
            .Where(purpose => purpose.Purpose.Contains("Кронштейн"))
            .GroupBy(purpose => purpose.Material)
            .Select(group => (
                name: group.Key ?? dbErrorString,
                unit: group.First().Measure ?? dbErrorString,
                quantity: group.Sum(groupElement => groupElement.Quantity).ToString() ?? dbErrorString
            ))
            .ToList();
        //формирование списка электрических комплектующих
        var electricalParts = stand.StandElectricalComponent
            .SelectMany(equip => equip.ElectricalComponent.Purposes)
            .GroupBy(purpose => purpose.Material)
            .Select(group => (
                name: group.Key ?? dbErrorString,
                unit: group.First().Measure ?? dbErrorString,
                quantity: group.Sum(item => item.Quantity).ToString() ?? dbErrorString
            ))
            .ToList();
        //формирование списка дополнительного оборудования
        var additionalParts = stand.StandAdditionalEquips
            .SelectMany(equip => equip.AdditionalEquip.Purposes)
            .GroupBy(purpose => purpose.Material)
            .Select(group => (
                name: group.Key ?? dbErrorString,
                unit: group.First().Measure ?? dbErrorString,
                quantity: group.Sum(item => item.Quantity).ToString() ?? dbErrorString
            ))
            .Except(sensorsHolders);
        var othersParts = additionalParts
            .Where(part => part.name.Contains("Шильдик") || part.name.Contains("Табличка"))
            .ToList();

        var supplies = additionalParts
            .Except(othersParts)
            .ToList();

        return new StandsReportData(
            pipesList,
            armaturesList,
            treeList,
            kmchList,
            drainageParts,
            framesList,
            sensorsHolders,
            electricalParts,
            othersParts,
            supplies
        );
    }

    #endregion


    #region Заголовки

    //создает заголовок для стенда
    private void CreateStandTableHeader(IXLWorksheet ws, Stand stand, XLAlignmentHorizontalValues alignment)
    {
        ws.Cell("A1").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        var exportDays = ws.Cell("A2");
        exportDays.Value = "Срок\nпоставки\nкомплектующих";
        exportDays.Style.Font.Bold = true;
        exportDays.Style.Alignment.Horizontal = alignment;
        exportDays.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        exportDays.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);

        CreateStandTableHeader(ws, stand, alignment);

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

    //создает заголовок сводной ведомости
    private void CreateCommonListTableHeader(IXLWorksheet ws, ProjectInfo project,
        XLAlignmentHorizontalValues alignment)
    {
        var headerRange = ws.Range("B1:F3");

        var customerCompanyRange = ws.Range("B1:F1").Merge();
        customerCompanyRange.Value = $"{project.Company}";
        customerCompanyRange.Style.Alignment.Horizontal = alignment;

        var commonListStringRange = ws.Range("B2:F2").Merge();
        commonListStringRange.Value = "Сводная ведомость комплектующих";
        commonListStringRange.Style.Alignment.Horizontal = alignment;

        ws.Cell("B3").Value = "Наименование";
        ws.Cell("C3").Value = "Ед.изм.";
        ws.Cell("D3").Value = "Кол.";
        ws.Cell("E3").Value = "Цена за шт., руб";
        ws.Cell("F3").Value = "Цена, руб";

        ws.Columns().AdjustToContents();

        headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Alignment.Horizontal = alignment;
        headerRange.Style.Font.SetBold();
    }

    //создает подзаголовок для подтаблицы и возвращает следующую строку
    private int CreateSubheaderOnWorksheet(int row, string title, IXLWorksheet ws,
        XLAlignmentHorizontalValues alignment)
    {
        var subHeaderRange = ws.Range($"B{row}:F{row}");
        subHeaderRange.Merge();
        subHeaderRange.Value = title;
        subHeaderRange.Style.Alignment.Horizontal = alignment;
        subHeaderRange.Style.Font.SetFontSize(10);
        subHeaderRange.Style.Font.SetBold();

        row++;
        return row;
    }

    #endregion


    #region Заполнители

    //заполняет сводную ведомость
    private void FillCommonListTable(IXLWorksheet ws, ProjectInfo project)
    {
        const string dbErrorString = "Ошибка получения данных из БД";

        //Формирование списка труб
        var pipesList = project.Stands
            .SelectMany(stand => stand.ObvyazkiInStand)
            .Select(obv => new
            {
                name = obv.MaterialLine,
                units = obv.MaterialLineMeasure,
                length = obv.MaterialLineCount
            })
            .GroupBy(pipe => pipe.name)
            .Select(group => (
                name: group.Key ?? dbErrorString,
                unit: group.First().units ?? dbErrorString,
                quantity: group.Sum(pipe => pipe.length).ToString() ?? dbErrorString
            ))
            .ToList();

        //Формирование списка арматуры
        var armaturesList = project.Stands
            .SelectMany(stand => stand.ObvyazkiInStand)
            .Select(obv => new
            {
                name = obv.Armature,
                units = obv.ArmatureMeasure,
                quantity = obv.ArmatureCount
            })
            .GroupBy(arm => arm.name)
            .Select(group => (
                name: group.Key ?? dbErrorString,
                unit: group.First().units ?? dbErrorString,
                quantity: group.Sum(arm => arm.quantity).ToString() ?? dbErrorString
            ))
            .ToList();

        //Формирование списка тройников и КМЧ
        var treeList = project.Stands
            .SelectMany(stand => stand.ObvyazkiInStand)
            .Select(obv => new
            {
                name = obv.TreeSocket,
                units = obv.TreeSocketMaterialMeasure,
                quantity = obv.TreeSocketCount
            })
            .GroupBy(item => item.name)
            .Select(group => (
                name: group.Key ?? dbErrorString,
                unit: group.First().units ?? dbErrorString,
                quantity: group.Sum(item => item.quantity).ToString() ?? dbErrorString
            ))
            .ToList();
        var kmchList = project.Stands
            .SelectMany(stand => stand.ObvyazkiInStand)
            .Select(obv => new
            {
                name = obv.KMCH,
                units = obv.KMCHMeasure,
                quantity = obv.KMCHCount
            })
            .GroupBy(item => item.name)
            .Select(group => (
                name: group.Key ?? dbErrorString,
                unit: group.First().units ?? dbErrorString,
                quantity: group.Sum(item => item.quantity).ToString() ?? dbErrorString
            ))
            .ToList();

        //формирование дренажа
        var drainageParts = project.Stands
            .SelectMany(stand => stand.StandDrainages)
            .SelectMany(drainage => drainage.Drainage.Purposes)
            .GroupBy(purpose => purpose.Material)
            .Select(group => (
                name: group.Key ?? dbErrorString,
                unit: group.First().Measure ?? dbErrorString,
                quantity: group.Sum(groupElement => groupElement.Quantity).ToString() ?? dbErrorString
            ))
            .ToList();

        //Формирование списка рамных комплектующих
        var framesList = project.Stands
            .SelectMany(stand => stand.StandFrames)
            .SelectMany(fr => fr.Frame.Components)
            .Select(comp => new
            {
                name = comp.ComponentName,
                unit = comp.Measure,
                quantity = comp.Count
            })
            .GroupBy(frameComp => frameComp.name)
            .Select(group => (
                name: group.Key ?? dbErrorString,
                unit: group.First().unit ?? dbErrorString,
                quantity: group.Sum(frameComp => frameComp.quantity).ToString() ?? dbErrorString
            ))
            .ToList();

        //сомнительная хрень, хз что брать за источник информации
        //формирование списка кронштейнов
        var sensorsHolders = project.Stands
            .SelectMany(stand => stand.StandAdditionalEquips)
            .SelectMany(equip => equip.AdditionalEquip.Purposes)
            .Where(purpose => purpose.Purpose.Contains("Кронштейн"))
            .GroupBy(purpose => purpose.Material)
            .Select(group => (
                name: group.Key ?? dbErrorString,
                unit: group.First().Measure ?? dbErrorString,
                quantity: group.Sum(groupElement => groupElement.Quantity).ToString() ?? dbErrorString
            ))
            .ToList();

        //формирование списка электрических комплектующих
        var electricalParts = project.Stands
            .SelectMany(stand => stand.StandElectricalComponent)
            .SelectMany(equip => equip.ElectricalComponent.Purposes)
            .GroupBy(purpose => purpose.Material)
            .Select(group => (
                name: group.Key ?? dbErrorString,
                unit: group.First().Measure ?? dbErrorString,
                quantity: group.Sum(item => item.Quantity).ToString() ?? dbErrorString
            ))
            .ToList();

        //формирование списка дополнительного оборудования
        var additionalParts = project.Stands
            .SelectMany(stand => stand.StandAdditionalEquips)
            .SelectMany(equip => equip.AdditionalEquip.Purposes)
            .GroupBy(purpose => purpose.Material)
            .Select(group => (
                name: group.Key ?? dbErrorString,
                unit: group.First().Measure ?? dbErrorString,
                quantity: group.Sum(item => item.Quantity).ToString() ?? dbErrorString
            ))
            .Except(sensorsHolders);

        var othersParts = additionalParts
            .Where(part => part.name.Contains("Шильдик") || part.name.Contains("Табличка"))
            .ToList();

        var supplies = additionalParts
            .Except(othersParts)
            .ToList();

        var activeRow = 4;

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Сортамент труб", ws, XLAlignmentHorizontalValues.Center);
        activeRow = FillSubtableData(activeRow, pipesList, ws, XLAlignmentHorizontalValues.Left);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Арматура", ws, XLAlignmentHorizontalValues.Center);
        activeRow = FillSubtableData(activeRow, armaturesList, ws, XLAlignmentHorizontalValues.Left);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Тройники и КМЧ", ws, XLAlignmentHorizontalValues.Center);
        activeRow = FillSubtableData(activeRow, treeList, ws, XLAlignmentHorizontalValues.Left);
        activeRow = FillSubtableData(activeRow, kmchList, ws, XLAlignmentHorizontalValues.Left);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Дренаж", ws, XLAlignmentHorizontalValues.Center);
        activeRow = FillSubtableData(activeRow, drainageParts, ws, XLAlignmentHorizontalValues.Left);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Рамные комплектующие", ws,
            XLAlignmentHorizontalValues.Center);
        activeRow = FillSubtableData(activeRow, framesList, ws, XLAlignmentHorizontalValues.Left);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Кронштейны", ws, XLAlignmentHorizontalValues.Center);
        activeRow = FillSubtableData(activeRow, sensorsHolders, ws, XLAlignmentHorizontalValues.Left);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Электрические компоненты", ws,
            XLAlignmentHorizontalValues.Center);
        activeRow = FillSubtableData(activeRow, electricalParts, ws, XLAlignmentHorizontalValues.Left);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Прочие", ws, XLAlignmentHorizontalValues.Center);
        activeRow = FillSubtableData(activeRow, othersParts, ws, XLAlignmentHorizontalValues.Left);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Расходные материалы", ws,
            XLAlignmentHorizontalValues.Center);
        activeRow = FillSubtableData(activeRow, supplies, ws, XLAlignmentHorizontalValues.Left);
    }

    //заполняет целиком таблицу для стенда
    private void FillStandTable(IXLWorksheet ws, Stand stand)
    {
        var data = GetStandReportData(stand);
        var activeRow = 4;

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Сортамент труб", ws, XLAlignmentHorizontalValues.Center);
        activeRow = FillSubtableData(activeRow, data.PipesList, ws, XLAlignmentHorizontalValues.Left);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Арматура", ws, XLAlignmentHorizontalValues.Center);
        activeRow = FillSubtableData(activeRow, data.ArmaturesList, ws, XLAlignmentHorizontalValues.Left);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Тройники и КМЧ", ws, XLAlignmentHorizontalValues.Center);
        activeRow = FillSubtableData(activeRow, data.TreeList, ws, XLAlignmentHorizontalValues.Left);
        activeRow = FillSubtableData(activeRow, data.KmchList, ws, XLAlignmentHorizontalValues.Left);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Дренаж", ws, XLAlignmentHorizontalValues.Center);
        activeRow = FillSubtableData(activeRow, data.DrainageParts, ws, XLAlignmentHorizontalValues.Left);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Рамные комплектующие", ws,
            XLAlignmentHorizontalValues.Center);
        activeRow = FillSubtableData(activeRow, data.FramesList, ws, XLAlignmentHorizontalValues.Left);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Кронштейны", ws, XLAlignmentHorizontalValues.Center);
        activeRow = FillSubtableData(activeRow, data.SensorsHolders, ws, XLAlignmentHorizontalValues.Left);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Электрические компоненты", ws,
            XLAlignmentHorizontalValues.Center);
        activeRow = FillSubtableData(activeRow, data.ElectricalParts, ws, XLAlignmentHorizontalValues.Left);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Прочие", ws, XLAlignmentHorizontalValues.Center);
        activeRow = FillSubtableData(activeRow, data.OthersParts, ws, XLAlignmentHorizontalValues.Left);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Расходные материалы", ws,
            XLAlignmentHorizontalValues.Center);
        activeRow = FillSubtableData(activeRow, data.Supplies, ws, XLAlignmentHorizontalValues.Left);
    }

    //Заполняет подтаблицу и возвращает следующую строку
    private int FillSubtableData(int startRow, List<(string name, string unit, string quantity)> items, IXLWorksheet ws,
        XLAlignmentHorizontalValues alignment)
    {
        var currentRow = startRow;
        foreach (var item in items)
        {
            ws.Cell($"B{currentRow}").Style.Alignment.Horizontal = alignment;
            ws.Cell($"C{currentRow}").Style.Alignment.Horizontal = alignment;
            ws.Cell($"D{currentRow}").Style.Alignment.Horizontal = alignment;

            ws.Cell($"B{currentRow}").Value = item.name;
            ws.Cell($"C{currentRow}").Value = item.unit;
            ws.Cell($"D{currentRow}").Value = item.quantity;
            ws.Cell($"D{currentRow}").Value = item.quantity;
            ws.Cell($"D{currentRow}").Value = item.quantity;

            currentRow++;
        }

        return currentRow;
    }

    #endregion
}