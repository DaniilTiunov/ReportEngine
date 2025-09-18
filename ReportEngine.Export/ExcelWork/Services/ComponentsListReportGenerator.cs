using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
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

        using (var wb = new XLWorkbook())
        {

            int standNumber = 1;

            //заполняем листы по стендам
            foreach (var stand in project.Stands)
            {
                var ws = wb.Worksheets.Add($"{standNumber}");

                CreateStandTableHeader(ws, stand);
                FillStandTable(ws, stand);

                standNumber++;
            }


            //заполняем сводную ведомость
            var lastSheet = wb.Worksheets.Add("Сводная заявка");



            CreateCommonListTableHeader(lastSheet, project);
            FillCommonListTable(lastSheet, project);




            //применяем оформление ко всему документу

            foreach (var ws in wb.Worksheets)
            {
                ws.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                ws.Cells().Style.Alignment.WrapText = true;
                ws.Columns().AdjustToContents();

            }


            var savePath = SettingsManager.GetReportDirectory();
            var fileName = "Ведомость комплектующих___" + DateTime.Now.ToString("dd-MM-yy___HH-mm-ss") + ".xlsx";


            var fullSavePath = Path.Combine(savePath, fileName);

            Debug.WriteLine("Отчёт сохранён: " + fullSavePath);
            wb.SaveAs(fullSavePath);

        }
    }


    #region Заголовки 

    //Создает заголовок на листе (для стенда)
    private void CreateStandTableHeader(IXLWorksheet ws, Stand stand)
    {

        var headerRange = ws.Range("B1:D3");

        ws.Cell("B1").Value = $"Код-KKS: {stand.KKSCode}";

        var standNameRange = ws.Range("C1:D1").Merge();
        standNameRange.Value = $"Наименование: {stand.Design}";

        var commonListStringRange = ws.Range("B2:D2").Merge();
        commonListStringRange.Value = "Сводная ведомость комплектующих";

        ws.Cell("B3").Value = "Наименование";
        ws.Cell("C3").Value = "Ед. изм";
        ws.Cell("D3").Value = "Кол.";

        ws.Columns().AdjustToContents();



        headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);

        headerRange.Style.Font.SetBold();

    }


    //создание заголовка для сводной ведомости
    private void CreateCommonListTableHeader(IXLWorksheet ws, ProjectInfo project)
    {

        var headerRange = ws.Range("B1:D3");

        var customerCompanyRange = ws.Range("B1:D1").Merge();
        customerCompanyRange.Value = $"{project.Company}";

        var commonListStringRange = ws.Range("B2:D2").Merge();
        commonListStringRange.Value = "Сводная ведомость комплектующих";

        ws.Cell("B3").Value = "Наименование";
        ws.Cell("C3").Value = "Ед.изм.";
        ws.Cell("D3").Value = "Кол.";



        ws.Columns().AdjustToContents();



        headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);

        headerRange.Style.Font.SetBold();

    }

    #endregion


    #region Заполнители 


    //Заполняет таблицу на листе (для стенда)
    private void FillStandTable(IXLWorksheet ws, Stand stand)
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
                name = comp.ComponentType,
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



        activeRow = CreateSubheaderOnWorksheet(activeRow, "Сортамент труб", ws);
        activeRow = FillSubtableData(activeRow, pipesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Арматура", ws);
        activeRow = FillSubtableData(activeRow, armaturesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Тройники и КМЧ", ws);
        activeRow = FillSubtableData(activeRow, treeList, ws);
        activeRow = FillSubtableData(activeRow, kmchList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Дренаж", ws);
        activeRow = FillSubtableData(activeRow, drainageParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Рамные комплектующие", ws);
        activeRow = FillSubtableData(activeRow, framesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Кронштейны", ws);
        activeRow = FillSubtableData(activeRow, sensorsHolders, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Электрические компоненты", ws);
        activeRow = FillSubtableData(activeRow, electricalParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Прочие", ws);
        activeRow = FillSubtableData(activeRow, othersParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Расходные материалы", ws);
        activeRow = FillSubtableData(activeRow, supplies, ws);

    }

    //создание сводной ведомости
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
                name = comp.ComponentType,
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



        activeRow = CreateSubheaderOnWorksheet(activeRow, "Сортамент труб", ws);
        activeRow = FillSubtableData(activeRow, pipesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Арматура", ws);
        activeRow = FillSubtableData(activeRow, armaturesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Тройники и КМЧ", ws);
        activeRow = FillSubtableData(activeRow, treeList, ws);
        activeRow = FillSubtableData(activeRow, kmchList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Дренаж", ws);
        activeRow = FillSubtableData(activeRow, drainageParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Рамные комплектующие", ws);
        activeRow = FillSubtableData(activeRow, framesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Кронштейны", ws);
        activeRow = FillSubtableData(activeRow, sensorsHolders, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Электрические компоненты", ws);
        activeRow = FillSubtableData(activeRow, electricalParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Прочие", ws);
        activeRow = FillSubtableData(activeRow, othersParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Расходные материалы", ws);
        activeRow = FillSubtableData(activeRow, supplies, ws);


    }

    #endregion


    #region Вспомогательные 

    //создает подзаголовок для подтаблицы и возвращает следующую строку
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
    private int FillSubtableData(int startRow, List<(string name, string unit, string quantity)> items, IXLWorksheet ws)
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

    #endregion
}