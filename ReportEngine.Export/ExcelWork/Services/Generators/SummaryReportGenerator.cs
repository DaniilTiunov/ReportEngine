using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Reflection;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.DTO;
using ReportEngine.Export.DTO.JsonObjects;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.IniHeleprs;
using ReportEngine.Shared.Helpers;

namespace ReportEngine.Export.ExcelWork.Services.Generators;

public class SummaryReportGenerator : IReportGenerator
{
    private readonly IProjectInfoRepository _projectInfoRepository;
    private readonly IContainerRepository _containerRepository;

    public SummaryReportGenerator(IProjectInfoRepository projectInfoRepository, IContainerRepository containerRepository)
    {
        _projectInfoRepository = projectInfoRepository;
        _containerRepository = containerRepository;
    }

    public ReportType Type => ReportType.SummaryReport;

    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);

        using (var wb = new XLWorkbook())
        {
            var standNumber = 1;

            foreach (var stand in project.Stands)
            {
                var ws = wb.Worksheets.Add($"{standNumber}");

                CreateStandTableHeader(ws, stand, XLAlignmentHorizontalValues.Center);
                FillStandTable(ws, stand);

                ws.Columns().Style.Alignment.WrapText = false;
                ws.Rows().Style.Alignment.WrapText = false;

                ws.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();

                // Костылёк для нормального отображения
                var exportDaysRange = ws.Range("A2:A3");
                exportDaysRange.Style.Alignment.WrapText = true;

                foreach (var row in exportDaysRange.Rows())
                {
                    row.WorksheetRow().Height = 30;
                }

                ws.Columns("A").Width = 18; // ширина столбца "Срок поставки комплектующих"

                standNumber++;
            }

            // Сводная ведомость
            var summarySheet = wb.Worksheets.Add("Сводная заявка");
            CreateCommonListTableHeader(summarySheet, project);
            await FillCommonListTable(summarySheet, project);

            summarySheet.Columns().Style.Alignment.WrapText = false;
            summarySheet.Rows().Style.Alignment.WrapText = false;
            summarySheet.Columns().AdjustToContents();
            summarySheet.Rows().AdjustToContents();

            // Калькуляция
            var calculationSheet = wb.Worksheets.Add("Калькуляция");
            CreateCalcullationTableHeader(calculationSheet, project);
            await FillCalculationTable(calculationSheet, project);

            // Применяем оформление ко всему документу
            foreach (var ws in wb.Worksheets)
            {
                ws.Cells().Style.Font.FontName = "Times New Roman";
            }

            var savePath = SettingsManager.GetReportDirectory();
            var fileName = ExcelReportHelper.CreateReportName("Сводная ведомость", "xlsx");
            var fullSavePath = Path.Combine(savePath, fileName);

            Debug.WriteLine("Отчёт сохранён: " + fullSavePath);
            wb.SaveAs(fullSavePath);
        }
    }
    public async Task GenerateAsync(int projectId, List<Stand>? selectedStands = null)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);

        using (var wb = new XLWorkbook())
        {
            var standNumber = 1;

            foreach (var stand in selectedStands)
            {
                var ws = wb.Worksheets.Add($"{standNumber}");

                CreateStandTableHeader(ws, stand, XLAlignmentHorizontalValues.Center);
                FillStandTable(ws, stand);

                ws.Columns().Style.Alignment.WrapText = false;
                ws.Rows().Style.Alignment.WrapText = false;

                ws.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();

                // Костылёк для нормального отображения
                var exportDaysRange = ws.Range("A2:A3");
                exportDaysRange.Style.Alignment.WrapText = true;

                foreach (var row in exportDaysRange.Rows())
                {
                    row.WorksheetRow().Height = 30;
                }

                ws.Columns("A").Width = 18; // ширина столбца "Срок поставки комплектующих"

                standNumber++;
            }

            // Сводная ведомость
            var summarySheet = wb.Worksheets.Add("Сводная заявка");
            CreateCommonListTableHeader(summarySheet, project);
            await FillCommonListTable(summarySheet, project);

            summarySheet.Columns().Style.Alignment.WrapText = false;
            summarySheet.Rows().Style.Alignment.WrapText = false;
            summarySheet.Columns().AdjustToContents();
            summarySheet.Rows().AdjustToContents();

            // Калькуляция
            var calculationSheet = wb.Worksheets.Add("Калькуляция");
            CreateCalcullationTableHeader(calculationSheet, project);
            await FillCalculationTable(calculationSheet, project);

            // Применяем оформление ко всему документу
            foreach (var ws in wb.Worksheets)
            {
                ws.Cells().Style.Font.FontName = "Times New Roman";
            }

            var savePath = SettingsManager.GetReportDirectory();
            var fileName = ExcelReportHelper.CreateReportName("Сводная ведомость", "xlsx");
            var fullSavePath = Path.Combine(savePath, fileName);

            Debug.WriteLine("Отчёт сохранён: " + fullSavePath);
            wb.SaveAs(fullSavePath);
        }
    }

    #region Вспомогательные

    //валидация и вывод в таблицу
    private void PasteRecord(int row, EquipmentRecord? record, IXLWorksheet ws)
    {  

        ws.Cell($"A{row}").Value = record?.ExportDays.Value?.ToString();

        ws.Cell($"B{row}").Value = record?.Name.Value?.ToString();

        ws.Cell($"C{row}").Value = record?.Unit.Value?.ToString();

        ws.Cell($"D{row}").Value = record?.Quantity.Value?.Round(2).ToString();

        ws.Cell($"E{row}").Value = record?.CostPerUnit.Value?.ToString();

        ws.Cell($"F{row}").Value = record?.CommonCost.Value.Ceiling().ToString();

        //if (!record.ExportDays.IsValid)
        //{
        //    ws.Cell($"A{row}").Value += "\n" + ExcelReportHelper.CommonErrorString;
        //}

        //if (!record.Name.IsValid)
        //{
        //    ws.Cell($"B{row}").Value += "\n" + ExcelReportHelper.CommonErrorString;
        //}

        //if (!record.Unit.IsValid)
        //{
        //    ws.Cell($"C{row}").Value += "\n" + ExcelReportHelper.CommonErrorString;
        //}

        //if (!record.Quantity.IsValid)
        //{
        //    ws.Cell($"D{row}").Value += "\n" + ExcelReportHelper.CommonErrorString;
        //}

        //if (!record.CostPerUnit.IsValid)
        //{
        //    ws.Cell($"E{row}").Value += "\n" + ExcelReportHelper.CommonErrorString;
        //}

        //if (!record.CommonCost.IsValid)
        //{
        //    ws.Cell($"F{row}").Value += "\n" + ExcelReportHelper.CommonErrorString;
        //}
    }

    #endregion Вспомогательные

    #region Заголовки

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

    //создает заголовок для стенда
    private void CreateStandTableHeader(IXLWorksheet ws, Stand stand, XLAlignmentHorizontalValues alignment)
    {
        var headerRange = ws.Range("A1:F3");

        var exportDaysRange = ws.Range("A2:A3").Merge();
        exportDaysRange.Value = "Срок\nпоставки\nкомплектующих,\nдней";


        var kksCodeRange = ws.Range("C1:F1").Merge();
        kksCodeRange.Value = $"Код KKS:{stand.KKSCode}";

        var summaryComponentsList = ws.Range("B2:F2").Merge();
        summaryComponentsList.Value = "Сводная ведомость комплектующих";

        ws.Cell("B3").Value = "Наименование";
        ws.Cell("C3").Value = "Ед.изм.";
        ws.Cell("D3").Value = "Кол.";
        ws.Cell("E3").Value = "Цена за шт., руб";
        ws.Cell("F3").Value = "Цена, руб";

        headerRange.Style.Font.SetBold();
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        headerRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);



    }

    //создает заголовок сводной ведомости
    private void CreateCommonListTableHeader(IXLWorksheet ws, ProjectInfo project)
    {
        var headerRange = ws.Range("B1:F3");

        var customerCompanyRange = ws.Range("B1:F1").Merge();
        customerCompanyRange.Value = $"{project.Company}";
        customerCompanyRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        var commonListStringRange = ws.Range("B2:F2").Merge();
        commonListStringRange.Value = "Сводная ведомость комплектующих";
        commonListStringRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        ws.Cell("B3").Value = "Наименование";
        ws.Cell("C3").Value = "Ед.изм.";
        ws.Cell("D3").Value = "Кол.";
        ws.Cell("E3").Value = "Цена за шт., руб";
        ws.Cell("F3").Value = "Цена, руб";

        ws.Columns().AdjustToContents();

        headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        headerRange.Style.Font.SetBold();
    }

    //создает заголовок листа калькуляции

    private void CreateCalcullationTableHeader(IXLWorksheet ws, ProjectInfo project)
    {
        var headerRange = ws.Range("A1:K6");

        var customerCompanyRange = ws.Range("C1:K2").Merge();
        customerCompanyRange.Value = $"{project.Company}";

        var descriptionRange = ws.Range("A3:A6").Merge();
        descriptionRange.Value = "Примечание";

        var deliveryTimeRange = ws.Range("B3:B6").Merge();
        deliveryTimeRange.Value = "Срок\nпоставки\nкомплектующих,\nдней";

        deliveryTimeRange.Style.Alignment.WrapText = true;

        foreach (var row in deliveryTimeRange.Rows())
        {
            row.WorksheetRow().Height = 20;
        }

        var devicesListRange = ws.Range("C3:K4").Merge();
        devicesListRange.Value = "Перечень оборудования, планируемого к поставке";

        var numberRange = ws.Range("C5:C6").Merge();
        numberRange.Value = "№ п/п";

        var standNameRange = ws.Range("D5:D6").Merge();
        standNameRange.Value = "Наименование стенда";

        var kksCodeRange = ws.Range("E5:E6").Merge();
        kksCodeRange.Value = "Код-KKS";

        var unitsRange = ws.Range("F5:F6").Merge();
        unitsRange.Value = "Ед.изм.";

        var quantityRange = ws.Range("G5:G6").Merge();
        quantityRange.Value = "Кол-во";

        var massRange = ws.Range("H5:H6").Merge();
        massRange.Value = "Масса, кг";

        var widthRange = ws.Range("I5:I6").Merge();
        widthRange.Value = "Ширина стенда, мм";

        var costPerUnitRange = ws.Range("J5:J6").Merge();
        costPerUnitRange.Value = "Цена за единицу, руб.";

        var priceRange = ws.Range("K5:K6").Merge();
        priceRange.Value = "Стоимость, руб";

        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        ws.Range("A3:K6").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        ws.Range("A3:K6").Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);

        customerCompanyRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        customerCompanyRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);


        headerRange.Style.Font.SetBold();


        ws.Columns("A").Width = 25; //ширина столбца "Примечание"
        ws.Columns("B").Width = 20; //ширина столбца "Срок поставки комплектующих"
        ws.Columns("C").Width = 10; //ширина столбца "№ п/п"
        ws.Columns("D").Width = 30; //ширина столбца "Наименование стенда"
        ws.Columns("E").Width = 30; //ширина столбца "Код-KKS"
        ws.Columns("F").Width = 10; //ширина столбца "Ед. изм"
        ws.Columns("G").Width = 10; //ширина столбца "Кол-во"
        ws.Columns("H").Width = 15; //ширина столбца "Масса, кг"
        ws.Columns("I").Width = 25; //ширина столбца "Ширина стенда"
        ws.Columns("J").Width = 25; //ширина столбца "Цена за единицу"
        ws.Columns("K").Width = 20; //ширина столбца "Стоимость"


    }

    #endregion Заголовки

    #region Заполнители

    //Заполняет подтаблицу и возвращает следующую строку
    private int FillSubtableData(int startRow, List<EquipmentRecord?> items, IXLWorksheet ws)
    {
        var currentRow = startRow;

        foreach (var item in items)
        {
            if (item == null)
                continue;

            PasteRecord(currentRow, item, ws);

            ws.Cell($"A{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell($"B{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell($"C{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell($"D{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell($"E{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell($"F{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            currentRow++;
        }

        return currentRow;
    }

    //заполняет таблицу для стенда
    private void FillStandTable(IXLWorksheet ws, Stand stand)
    {
        var activeRow = 4;

        var standList = new List<Stand> { stand };

        var generatedPartsData = ExcelReportHelper.GeneratePartsData(standList);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Сортамент труб", ws);
        activeRow = FillSubtableData(activeRow, generatedPartsData.PipesList, ws);
        activeRow = CreateUsualTotalRecord(activeRow, "Итого по категории:", generatedPartsData.PipesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Арматура", ws);
        activeRow = FillSubtableData(activeRow, generatedPartsData.ArmaturesList, ws);
        activeRow = CreateUsualTotalRecord(activeRow, "Итого по категории:", generatedPartsData.ArmaturesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Тройники и КМЧ", ws);
        activeRow = FillSubtableData(activeRow, generatedPartsData.TreeList, ws);
        activeRow = FillSubtableData(activeRow, generatedPartsData.KmchList, ws);

        //общий список, чтобы запихнуть в метод
        var treeAndKmchList = new List<EquipmentRecord>();
        treeAndKmchList.AddRange(generatedPartsData.TreeList);
        treeAndKmchList.AddRange(generatedPartsData.KmchList);

        activeRow = CreateUsualTotalRecord(activeRow, "Итого по категории:", treeAndKmchList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Дренаж", ws);
        activeRow = FillSubtableData(activeRow, generatedPartsData.DrainageParts, ws);
        activeRow = CreateUsualTotalRecord(activeRow, "Итого по категории:", generatedPartsData.DrainageParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Рамные комплектующие", ws);
        activeRow = FillSubtableData(activeRow, generatedPartsData.FramesList, ws);
        activeRow = CreateUsualTotalRecord(activeRow, "Итого по категории:", generatedPartsData.FramesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Кронштейны", ws);
        activeRow = FillSubtableData(activeRow, generatedPartsData.SensorsHolders, ws);
        activeRow = CreateUsualTotalRecord(activeRow, "Итого по категории:", generatedPartsData.SensorsHolders, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Электрические компоненты", ws);
        activeRow = FillSubtableData(activeRow, generatedPartsData.ElectricalParts, ws);
        activeRow = CreateUsualTotalRecord(activeRow, "Итого по категории:", generatedPartsData.ElectricalParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Прочие", ws);
        activeRow = FillSubtableData(activeRow, generatedPartsData.OthersParts, ws);
        activeRow = CreateUsualTotalRecord(activeRow, "Итого по категории:", generatedPartsData.OthersParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Расходные материалы", ws);
        activeRow = FillSubtableData(activeRow, generatedPartsData.Supplies, ws);
        activeRow = CreateUsualTotalRecord(activeRow, "Итого по категории:", generatedPartsData.Supplies, ws);

        var allPartsList = ExcelReportHelper.GenerateAllPartsCollection(generatedPartsData);
        activeRow = CreateUsualTotalRecord(activeRow, "Итого по категории:", allPartsList, ws);

        var generatedLaborData = ExcelReportHelper.GenerateLaborData(standList);
        var allLaborsList = ExcelReportHelper.GenerateAllLaborsCollection(generatedLaborData);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Трудозатраты", ws);
        activeRow = FillSubtableData(activeRow, allLaborsList, ws);
        activeRow = CreateLaborTotalRecord(activeRow, allLaborsList, ws);

        var allData = new List<EquipmentRecord>();

        allData.AddRange(allPartsList);
        allData.AddRange(allLaborsList);

        activeRow = CreateUsualTotalRecord(activeRow, "Итого по стенду:", allData, ws);
    }

    //заполняет сводную ведомость
    private async Task FillCommonListTable(IXLWorksheet ws, ProjectInfo project)
    {
        var activeRow = 4;

        var containerBatches = _containerRepository.GetAllByProjectIdAsync(project.Id);

        var generatedPartsData = ExcelReportHelper.GeneratePartsData(project.Stands);

        //принудительно обнуляем сроки поставки, они там не нужны (вроде)
        foreach (var property in generatedPartsData.GetType().GetProperties())
        {
            var propertyValue = property.GetValue(generatedPartsData);
            var recordList = propertyValue as List<EquipmentRecord>;

            if (recordList != null)
            {
                var tempList = new List<EquipmentRecord>(recordList);
                recordList.Clear();
                foreach (var part in tempList)
                {

                    var sraka = new EquipmentRecord
                    {
                        Name = part.Name,
                        Quantity = part.Quantity,
                        Unit = part.Unit,
                        CostPerUnit = part.CostPerUnit,
                        CommonCost = part.CommonCost,
                        ExportDays = new ValidatedField<int?>(null, false)
                    };

                    recordList.Add(sraka);
                }
            }
            ;
        }


        activeRow = CreateSubheaderOnWorksheet(activeRow, "Сортамент труб", ws);
        activeRow = FillSubtableData(activeRow, generatedPartsData.PipesList, ws);
        activeRow = CreateUsualTotalRecord(activeRow, "Итого по категории", generatedPartsData.PipesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Арматура", ws);
        activeRow = FillSubtableData(activeRow, generatedPartsData.ArmaturesList, ws);
        activeRow = CreateUsualTotalRecord(activeRow, "Итого по категории", generatedPartsData.ArmaturesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Тройники и КМЧ", ws);
        activeRow = FillSubtableData(activeRow, generatedPartsData.TreeList, ws);
        activeRow = FillSubtableData(activeRow, generatedPartsData.KmchList, ws);

        //общий список, чтобы запихнуть в метод
        var treeAndKmchList = new List<EquipmentRecord>();
        treeAndKmchList.AddRange(generatedPartsData.TreeList);
        treeAndKmchList.AddRange(generatedPartsData.KmchList);

        activeRow = CreateUsualTotalRecord(activeRow, "Итого по категории", treeAndKmchList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Дренаж", ws);
        activeRow = FillSubtableData(activeRow, generatedPartsData.DrainageParts, ws);
        activeRow = CreateUsualTotalRecord(activeRow, "Итого по категории", generatedPartsData.DrainageParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Рамные комплектующие", ws);
        activeRow = FillSubtableData(activeRow, generatedPartsData.FramesList, ws);
        activeRow = CreateUsualTotalRecord(activeRow, "Итого по категории", generatedPartsData.FramesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Кронштейны", ws);
        activeRow = FillSubtableData(activeRow, generatedPartsData.SensorsHolders, ws);
        activeRow = CreateUsualTotalRecord(activeRow, "Итого по категории", generatedPartsData.SensorsHolders, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Электрические компоненты", ws);
        activeRow = FillSubtableData(activeRow, generatedPartsData.ElectricalParts, ws);
        activeRow = CreateUsualTotalRecord(activeRow, "Итого по категории", generatedPartsData.ElectricalParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Прочие", ws);
        activeRow = FillSubtableData(activeRow, generatedPartsData.OthersParts, ws);
        activeRow = CreateUsualTotalRecord(activeRow, "Итого по категории", generatedPartsData.OthersParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Расходные материалы", ws);
        activeRow = FillSubtableData(activeRow, generatedPartsData.Supplies, ws);
        activeRow = CreateUsualTotalRecord(activeRow, "Итого по категории", generatedPartsData.Supplies, ws);

        var allPartsList = ExcelReportHelper.GenerateAllPartsCollection(generatedPartsData);
        activeRow = CreateUsualTotalRecord(activeRow, "Итого по комплектующим", allPartsList, ws);

        var generatedLaborData = ExcelReportHelper.GenerateLaborData(project.Stands);
        var allLaborsList = ExcelReportHelper.GenerateAllLaborsCollection(generatedLaborData);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Трудозатраты", ws);
        activeRow = FillSubtableData(activeRow, allLaborsList, ws);
        activeRow = CreateLaborTotalRecord(activeRow, allLaborsList, ws);

        var allData = new List<EquipmentRecord>();

        allData.AddRange(allPartsList);
        allData.AddRange(allLaborsList);

        activeRow = CreateUsualTotalRecord(activeRow, "Итого по комплектующим и трудозатратам:", allData, ws);
        var containersData = ExcelReportHelper.GenerateContainersData(await containerBatches);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Упаковка", ws);
        activeRow = FillSubtableData(activeRow, containersData, ws);
        activeRow = CreateUsualTotalRecord(activeRow, "Итого по упаковке:", containersData, ws);

        allData.AddRange(containersData);
        activeRow = CreateUsualTotalRecord(activeRow, "Итого по проекту:", allData, ws);
    }

    //заполняет лист калькуляции
    private async Task FillCalculationTable(IXLWorksheet ws, ProjectInfo project)
    {
        var activeRow = 7;

        var containerBatches = _containerRepository.GetAllByProjectIdAsync(project.Id);

        var standsRecords = project.Stands
            .GroupBy(stand => stand.Design)
            .Select(group =>
            {
                var generatedPartsData = ExcelReportHelper.GeneratePartsData(new List<Stand> { group.FirstOrDefault() });
                var partsRecords = ExcelReportHelper.GenerateAllPartsCollection(generatedPartsData);

                var exportDays = partsRecords.Max(part => part.ExportDays.Value);
                var name = group.FirstOrDefault().Design;
                var kks = group.FirstOrDefault().KKSCode;
                var unit = "шт.";
                var quantity = group.Count();
                var weight = group.FirstOrDefault().Weight.RoundUp(1);
                var width = group.FirstOrDefault().Width;
                var cost = (float)group.FirstOrDefault().StandSummCost;

                var commonCost = (quantity * cost).Ceiling();

                return new
                {
                    exportDays,
                    name,
                    kks,
                    unit,
                    quantity,
                    weight,
                    width,
                    cost,
                    commonCost
                };
            });

        var standNumber = 1;

        foreach (var stand in standsRecords)
        {
            ws.Cell($"B{activeRow}").Value = stand.exportDays.ToString(); ;
            ws.Cell($"B{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell($"C{activeRow}").Value = standNumber.ToString(); ;
            ws.Cell($"C{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            ws.Cell($"D{activeRow}").Value = stand.name?.ToString(); ;
            ws.Cell($"D{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            ws.Cell($"E{activeRow}").Value = stand.kks?.ToString(); ;
            ws.Cell($"E{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            ws.Cell($"F{activeRow}").Value = stand.unit.ToString(); ;
            ws.Cell($"F{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell($"G{activeRow}").Value = stand.quantity.ToString(); ;
            ws.Cell($"G{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell($"H{activeRow}").Value = stand.weight.ToString();
            ws.Cell($"H{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell($"I{activeRow}").Value = stand.width.ToString(); ;
            ws.Cell($"I{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell($"J{activeRow}").Value = stand.cost.ToString(); ;
            ws.Cell($"J{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            ws.Cell($"K{activeRow}").Value = stand.commonCost.ToString();
            ws.Cell($"K{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            standNumber++;
            activeRow++;
        }

        var standsPriceLabelRange = ws.Range($"C{activeRow}:J{activeRow}").Merge();
        standsPriceLabelRange.Value = $"Общее количество стендов {standNumber - 1} на сумму (без учета упаковки и транспортных расходов), без НДС, руб.";
        standsPriceLabelRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        standsPriceLabelRange.Style.Font.SetBold();

        var standsPrice = standsRecords.Sum(stand => stand.commonCost);

        var standsPriceValueCell = ws.Cell($"K{activeRow}");
        standsPriceValueCell.Value = standsPrice;
        standsPriceValueCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        standsPriceValueCell.Style.Font.SetBold();

        activeRow++;

        var transportPrice = 0.0f;

        var transportPriceLabelRange = ws.Range($"C{activeRow}:J{activeRow}").Merge();
        transportPriceLabelRange.Value = $"Транспортные расходы на сумму, без НДС, руб.";
        transportPriceLabelRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        transportPriceLabelRange.Style.Font.SetBold();

        var transportPriceValueCell = ws.Cell($"K{activeRow}");
        transportPriceValueCell.Value = transportPrice;
        transportPriceValueCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        transportPriceValueCell.Style.Font.SetBold();

        activeRow++;

        var containers = ExcelReportHelper.GenerateContainersData(await containerBatches);
        var containerPrice = containers.Sum(container => container.CommonCost.Value);

        var containerPriceLabelRange = ws.Range($"C{activeRow}:J{activeRow}").Merge();
        containerPriceLabelRange.Value = $"Стоимость упаковки на сумму, без НДС, руб.";
        containerPriceLabelRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        containerPriceLabelRange.Style.Font.SetBold();

        var containerPriceValueCell = ws.Cell($"K{activeRow}");
        containerPriceValueCell.Value = containerPrice;
        containerPriceValueCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        containerPriceValueCell.Style.Font.SetBold();

        activeRow++;

        var totalPrice = (float)standsPrice + transportPrice + containerPrice;

        var totalLabelRange = ws.Range($"C{activeRow}:J{activeRow}").Merge();
        totalLabelRange.Value = $"Итого, руб.";
        totalLabelRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        totalLabelRange.Style.Font.SetBold();

        var totalValueCell = ws.Cell($"K{activeRow}"); ;
        totalValueCell.Value = totalPrice;
        totalValueCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        totalValueCell.Style.Font.SetBold();
    }

    #endregion Заполнители

    #region Итоговые

    //Создает простую итоговую запись
    private int CreateUsualTotalRecord(int row, string title, List<EquipmentRecord> recordToCombine, IXLWorksheet ws)
    {
        var activeRow = row;

        var totalRecord = ExcelReportHelper.GenerateTotalRecord(recordToCombine);
        totalRecord.ExportDays = new ValidatedField<int?>(null, true);
        totalRecord.Name = new ValidatedField<string?>(title, true);
        totalRecord.Quantity = new ValidatedField<float?>(null, true);
        totalRecord.CostPerUnit = new ValidatedField<float?>(null, true);

        PasteRecord(activeRow, totalRecord, ws);

        ws.Cell($"B{activeRow}").Style.Font.SetBold();
        ws.Cell($"B{activeRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
        ws.Cell($"F{activeRow}").Style.Font.SetBold();
        ws.Cell($"F{activeRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

        activeRow++;
        return activeRow;
    }

    //Создает итоговую запись для трудозатрат
    private int CreateLaborTotalRecord(int row, List<EquipmentRecord> laborsRecordsList, IXLWorksheet ws)
    {
        var activeRow = row;

        var totalRecord = ExcelReportHelper.GenerateTotalRecord(laborsRecordsList);
        totalRecord.ExportDays = new ValidatedField<int?>(null, true);
        totalRecord.Name = new ValidatedField<string?>("Итого по трудозатратам:", true);
        totalRecord.CostPerUnit = new ValidatedField<float?>(null, true);

        PasteRecord(activeRow, totalRecord, ws);

        ws.Cell($"B{activeRow}").Style.Font.SetBold();
        ws.Cell($"B{activeRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
        ws.Cell($"D{activeRow}").Style.Font.SetBold();
        ws.Cell($"D{activeRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
        ws.Cell($"F{activeRow}").Style.Font.SetBold();
        ws.Cell($"F{activeRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

        activeRow++;
        return activeRow;
    }

    #endregion Итоговые
}
