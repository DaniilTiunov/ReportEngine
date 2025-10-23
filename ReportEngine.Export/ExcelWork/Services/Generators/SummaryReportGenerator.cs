using ClosedXML.Excel;
using QuestPDF.Infrastructure;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Generators.DTO;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Export.Mapping;
using ReportEngine.Shared.Config.IniHeleprs;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;


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
            var fileName = ExcelReportHelper.CreateReportName("Сводная_ведомость", "xlsx");
            var fullSavePath = Path.Combine(savePath, fileName);

            Debug.WriteLine("Отчёт сохранён: " + fullSavePath);
            wb.SaveAs(fullSavePath);
        }

    }



    #region Заголовки

    //создает заголовок для стенда
    private void CreateStandTableHeader(IXLWorksheet ws, Stand stand, XLAlignmentHorizontalValues alignment)
    {
        var headerRange = ws.Range("A1:F3");



        var exportDaysRange = ws.Range("A2:A3").Merge();
        exportDaysRange.Value = "Срок поставки комплектующих, дней";

        var kksCodeRange = ws.Range("C1:F1").Merge();
        kksCodeRange.Value = $"Код KKS:{stand.KKSCode}";

        var summaryComponentsList = ws.Range("B2:F2").Merge();
        summaryComponentsList.Value = "Сводная ведомость комплектующих";



        ws.Cell("B3").Value = "Наименование";
        ws.Cell("C3").Value = "Ед.изм.";
        ws.Cell("D3").Value = "Кол.";
        ws.Cell("E3").Value = "Цена за шт., руб";
        ws.Cell("F3").Value = "Цена, руб";



        headerRange.Style.Font.Bold = true;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        headerRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);

    }

    //создает заголовок сводной ведомости
    private void CreateCommonListTableHeader(IXLWorksheet ws, ProjectInfo project, XLAlignmentHorizontalValues alignment)
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
    private int CreateSubheaderOnWorksheet(int row, string title, IXLWorksheet ws)
    {
        var subHeaderRange = ws.Range($"B{row}:F{row}");
        subHeaderRange.Merge();
        subHeaderRange.Value = title;
        subHeaderRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        subHeaderRange.Style.Font.SetFontSize(10);
        subHeaderRange.Style.Font.SetBold();

        row++;
        return row;
    }



    #endregion



    #region Заполнители


    //заполняет целиком таблицу для стенда
    private void FillStandTable(IXLWorksheet ws, Stand stand)
    {
        var activeRow = 4;

        var temp = new List<Stand>();
        temp.Add(stand);
        var generatedData = GenerateStandsData(temp);


        activeRow = CreateSubheaderOnWorksheet(activeRow, "Сортамент труб", ws);
        activeRow = FillSubtableData(activeRow, generatedData.PipesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Арматура", ws);
        activeRow = FillSubtableData(activeRow, generatedData.ArmaturesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Тройники и КМЧ", ws);
        activeRow = FillSubtableData(activeRow, generatedData.TreeList, ws);
        activeRow = FillSubtableData(activeRow, generatedData.KmchList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Дренаж", ws);
        activeRow = FillSubtableData(activeRow, generatedData.DrainageParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Рамные комплектующие", ws);
        activeRow = FillSubtableData(activeRow, generatedData.FramesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Кронштейны", ws);
        activeRow = FillSubtableData(activeRow, generatedData.SensorsHolders, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Электрические компоненты", ws);
        activeRow = FillSubtableData(activeRow, generatedData.ElectricalParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Прочие", ws);
        activeRow = FillSubtableData(activeRow, generatedData.OthersParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Расходные материалы", ws);
        activeRow = FillSubtableData(activeRow, generatedData.Supplies, ws);



    }

    //Заполняет подтаблицу и возвращает следующую строку
    private int FillSubtableData(
        int startRow,
        List<ReportStandData> items,
        IXLWorksheet ws)
    {
        var currentRow = startRow;

        foreach (var item in items)
        {
            ws.Cell($"B{currentRow}").Value = item.Name;
            ws.Cell($"C{currentRow}").Value = item.Unit;
            ws.Cell($"D{currentRow}").Value = item.Quantity;
            ws.Cell($"E{currentRow}").Value = item.CostPerUnit;
            ws.Cell($"F{currentRow}").Value = item.CommonCost;

            ws.Range($"B{currentRow}:F{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            currentRow++;
        }

        return currentRow;
    }



    //заполняет сводную ведомость
    private void FillCommonListTable(IXLWorksheet ws, ProjectInfo project)
    {
       var generatedData = GenerateStandsData(project.Stands);

        var activeRow = 4;

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Сортамент труб", ws);
        activeRow = FillSubtableData(activeRow, generatedData.PipesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Арматура", ws);
        activeRow = FillSubtableData(activeRow, generatedData.ArmaturesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Тройники и КМЧ", ws);
        activeRow = FillSubtableData(activeRow, generatedData.TreeList, ws);
        activeRow = FillSubtableData(activeRow, generatedData.KmchList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Дренаж", ws);
        activeRow = FillSubtableData(activeRow, generatedData.DrainageParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Рамные комплектующие", ws);
        activeRow = FillSubtableData(activeRow, generatedData.FramesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Кронштейны", ws);
        activeRow = FillSubtableData(activeRow, generatedData.SensorsHolders, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Электрические компоненты", ws);
        activeRow = FillSubtableData(activeRow, generatedData.ElectricalParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Прочие", ws);
        activeRow = FillSubtableData(activeRow, generatedData.OthersParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Расходные материалы", ws);
        activeRow = FillSubtableData(activeRow, generatedData.Supplies, ws);
    }


    #endregion



    #region Вспомогательные

    private SummaryReportStandsData GenerateStandsData(IEnumerable<Stand> stands)
    {
        const string dbErrorString = "Ошибка получения данных из БД";

        //Формирование списка труб
        var pipesList = stands
            .SelectMany(stand => stand.ObvyazkiInStand)
            .Select(obv => new
            {
                name = obv.MaterialLine,
                units = obv.MaterialLineMeasure,
                length = obv.MaterialLineCount,
                price = obv.MaterialLineCostPerUnit
            })
            .GroupBy(pipe => pipe.name)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().units,
                quantity = group.Sum(pipe => pipe.length),
                costPerUnit = group.First().price
            })
            .Select(group => new ReportStandData
            {
                Name = group.name ?? dbErrorString,
                Unit = group.unit ?? dbErrorString,
                Quantity = (group.quantity.ToString() ?? dbErrorString),
                CostPerUnit = group.costPerUnit ?? dbErrorString,
                CommonCost = (group.costPerUnit == null || group.quantity == null)
                    ? dbErrorString
                    : (Single.Parse(group.costPerUnit) * group.quantity).ToString()
            })
            .ToList();



        //Формирование списка арматуры
        var armaturesList = stands
            .SelectMany(stand => stand.ObvyazkiInStand)
            .Select(obv => new
            {
                name = obv.Armature,
                units = obv.ArmatureMeasure,
                quantity = obv.ArmatureCount,
                price = obv.ArmatureCostPerUnit
            })
            .GroupBy(arm => arm.name)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().units,
                quantity = group.Sum(arm => arm.quantity),
                costPerUnit = group.First().price
            })
            .Select(group => new ReportStandData
            {
                Name = group.name ?? dbErrorString,
                Unit = group.unit ?? dbErrorString,
                Quantity = (group.quantity.ToString() ?? dbErrorString),
                CostPerUnit = group.costPerUnit ?? dbErrorString,
                CommonCost = (group.costPerUnit == null || group.quantity == null)
                    ? dbErrorString
                    : (Single.Parse(group.costPerUnit) * group.quantity).ToString()
            })
            .ToList();



        //Формирование списка тройников и КМЧ
        var treeList = stands
            .SelectMany(stand => stand.ObvyazkiInStand)
            .Select(obv => new
            {
                name = obv.TreeSocket,
                units = obv.TreeSocketMaterialMeasure,
                quantity = obv.TreeSocketCount,
                price = obv.TreeSocketMaterialCostPerUnit
            })
            .GroupBy(item => item.name)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().units,
                quantity = group.Sum(tree => tree.quantity),
                costPerUnit = group.First().price
            })
            .Select(group => new ReportStandData
            {
                Name = group.name ?? dbErrorString,
                Unit = group.unit ?? dbErrorString,
                Quantity = (group.quantity.ToString() ?? dbErrorString),
                CostPerUnit = group.costPerUnit ?? dbErrorString,
                CommonCost = (group.costPerUnit == null || group.quantity == null)
                    ? dbErrorString
                    : (Single.Parse(group.costPerUnit) * group.quantity).ToString()
            })
            .ToList();



        var kmchList = stands
            .SelectMany(stand => stand.ObvyazkiInStand)
            .Select(obv => new
            {
                name = obv.KMCH,
                units = obv.KMCHMeasure,
                quantity = obv.KMCHCount,
                price = obv.KMCHCostPerUnit
            })
            .GroupBy(item => item.name)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().units,
                quantity = group.Sum(tree => tree.quantity),
                costPerUnit = group.First().price
            })
            .Select(group => new ReportStandData
            {
                Name = group.name ?? dbErrorString,
                Unit = group.unit ?? dbErrorString,
                Quantity = (group.quantity.ToString() ?? dbErrorString),
                CostPerUnit = group.costPerUnit ?? dbErrorString,
                CommonCost = (group.costPerUnit == null || group.quantity == null)
                    ? dbErrorString
                    : (Single.Parse(group.costPerUnit) * group.quantity).ToString()
            })
            .ToList();



        //формирование дренажа
        var drainageParts = stands
            .SelectMany(stand => stand.StandDrainages)
            .SelectMany(drainage => drainage.Drainage.Purposes)
            .GroupBy(purpose => purpose.Material)
            .Select(group => new
            {

                name = group.Key,
                unit = group.First().Measure,
                quantity = group.Sum(groupElement => groupElement.Quantity),
                costPerUnit = group.First().CostPerUnit

            })
            .Select(group => new ReportStandData
            {
                Name = group.name ?? dbErrorString,
                Unit = group.unit ?? dbErrorString,
                Quantity = (group.quantity.ToString() ?? dbErrorString),
                CostPerUnit = group.costPerUnit.ToString() ?? dbErrorString,
                CommonCost = (group.costPerUnit == null || group.quantity == null)
                    ? dbErrorString
                    : (group.costPerUnit * group.quantity).ToString()
            })
            .ToList();


        //Формирование списка рамных комплектующих
        var framesList = stands
            .SelectMany(stand => stand.StandFrames)
            .SelectMany(fr => fr.Frame.Components)
            .Select(comp => new
            {
                name = comp.ComponentName,
                unit = comp.Measure,
                quantity = comp.Count,
                costPerUnit = comp.CostComponent
            })
            .GroupBy(frameComp => frameComp.name)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().unit,
                quantity = group.Sum(frameComp => frameComp.quantity),
                costPerUnit = group.First().costPerUnit,
            })
            .Select(record => new ReportStandData
            {
                Name = record.name ?? dbErrorString,
                Unit = record.unit ?? dbErrorString,
                Quantity = record.quantity.ToString() ?? dbErrorString,
                CostPerUnit = record.costPerUnit.ToString() ?? dbErrorString,
                CommonCost = (record.costPerUnit == null || record.quantity == null)
                    ? dbErrorString
                    : (record.costPerUnit * record.quantity).ToString()
            })
            .ToList();




        //формирование списка кронштейнов
        var sensorsHolders = stands
            .SelectMany(stand => stand.StandAdditionalEquips)
            .SelectMany(equip => equip.AdditionalEquip.Purposes)
            .Where(purpose => purpose.Purpose.Contains("Кронштейн"))   //сомнительная хрень, хз что брать за источник информации
            .GroupBy(purpose => purpose.Material)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().Measure,
                quantity = group.Sum(groupElement => groupElement.Quantity),
                costPerUnit = group.First().CostPerUnit
            })
            .Select(record => new ReportStandData
            {
                Name = record.name ?? dbErrorString,
                Unit = record.unit ?? dbErrorString,
                Quantity = record.quantity.ToString() ?? dbErrorString,
                CostPerUnit = record.costPerUnit.ToString() ?? dbErrorString,
                CommonCost = (record.costPerUnit == null || record.quantity == null)
                    ? dbErrorString
                    : (record.costPerUnit * record.quantity).ToString()
            })
            .ToList();



        //формирование списка электрических комплектующих
        var electricalParts = stands
            .SelectMany(stand => stand.StandElectricalComponent)
            .SelectMany(equip => equip.ElectricalComponent.Purposes)
            .GroupBy(purpose => purpose.Material)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().Measure,
                quantity = group.Sum(item => item.Quantity),
                costPerUnit = group.First().CostPerUnit
            })
            .Select(record => new ReportStandData
            {
                Name = record.name ?? dbErrorString,
                Unit = record.unit ?? dbErrorString,
                Quantity = record.quantity.ToString() ?? dbErrorString,
                CostPerUnit = record.costPerUnit.ToString() ?? dbErrorString,
                CommonCost = (record.costPerUnit == null || record.quantity == null)
                    ? dbErrorString
                    : (record.costPerUnit * record.quantity).ToString()
            })
            .ToList();



        //формирование списка дополнительного оборудования
        var additionalParts = stands
            .SelectMany(stand => stand.StandAdditionalEquips)
            .SelectMany(equip => equip.AdditionalEquip.Purposes)
            .GroupBy(purpose => purpose.Material)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().Measure,
                quantity = group.Sum(item => item.Quantity),
                costPerUnit = group.First().CostPerUnit
            })
            .Select(record => new ReportStandData
            {
                Name = record.name ?? dbErrorString,
                Unit = record.unit ?? dbErrorString,
                Quantity = record.quantity.ToString() ?? dbErrorString,
                CostPerUnit = record.costPerUnit.ToString() ?? dbErrorString,
                CommonCost = (record.costPerUnit == null || record.quantity == null)
                    ? dbErrorString
                    : (record.costPerUnit * record.quantity).ToString()
            })
            .Except(sensorsHolders);

        var othersParts = additionalParts
            .Where(part => part.Name.Contains("Шильдик") || part.Name.Contains("Табличка")) //тоже сомнительно
            .ToList();

        var supplies = additionalParts
            .Except(othersParts)
            .ToList();


        return new SummaryReportStandsData(
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
}