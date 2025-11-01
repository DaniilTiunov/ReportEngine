using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Generators.DTO;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Export.Mapping;
using ReportEngine.Shared.Config.IniHeleprs;
using System.Diagnostics;

namespace ReportEngine.Export.ExcelWork.Services.Generators;

public class ComponentListReportGenerator : IReportGenerator
{
    private readonly IProjectInfoRepository _projectInfoRepository;

    public ComponentListReportGenerator(IProjectInfoRepository projectInfoRepository)
    {
        _projectInfoRepository = projectInfoRepository;
    }

    public ReportType Type => ReportType.ComponentsListReport;

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

    //валидация и вывод в таблицу
    private void PasteRecord(int row, EquipmentRecord record, IXLWorksheet ws)
    {

        if (record.Name.Value != null)
        {
            ws.Cell($"B{row}").Value = record.Name.Value.ToString();
        }
        if (!record.Name.IsValid)
        {
            ws.Cell($"B{row}").Value += "\n" + ExcelReportHelper.CommonErrorString;
        }



        if (record.Unit.Value != null)
        {
            ws.Cell($"C{row}").Value = record.Unit.Value.ToString();
        }
        if (!record.Unit.IsValid)
        {
            ws.Cell($"C{row}").Value += "\n" + ExcelReportHelper.CommonErrorString;
        }



        if (record.Quantity.Value.HasValue)
        {
            ws.Cell($"D{row}").Value = record.Quantity.Value.ToString();
        }
        if (!record.Quantity.IsValid)
        {
            ws.Cell($"D{row}").Value += "\n" + ExcelReportHelper.CommonErrorString;
        }
    }

    #endregion



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

        headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Font.SetBold();
    }

    //создание заголовка для сводной заявки
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

        headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Font.SetBold();
    }

    //создает подзаголовок для подтаблицы и возвращает следующую строку
    private int CreateSubheaderOnWorksheet(int row, string title, IXLWorksheet ws)
    {
        var activeRow = row;

        var subHeaderRange = ws.Range($"B{activeRow}:D{activeRow}").Merge();
        subHeaderRange.Value = title;
        subHeaderRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        subHeaderRange.Style.Font.SetFontSize(10);
        subHeaderRange.Style.Font.SetBold();

        activeRow++;
        return activeRow;
    }

    #endregion



    #region Заполнители

    //Заполняет таблицу на листе (для стенда)
    private void FillStandTable(IXLWorksheet ws, Stand stand)
    {
        var generatedData = ExcelReportHelper.GeneratePartsData(new List<Stand> { stand });

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

    //создание сводной ведомости
    private void FillCommonListTable(IXLWorksheet ws, ProjectInfo project)
    {
        var generatedData = ExcelReportHelper.GeneratePartsData(project.Stands);

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

    //Заполняет подтаблицу и возвращает следующую строку
    private int FillSubtableData(int startRow, List<EquipmentRecord> items, IXLWorksheet ws)
    {
        var currentRow = startRow;

        foreach (var item in items)
        {
            PasteRecord(currentRow, item, ws);

            ws.Cell($"B{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell($"C{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell($"D{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            currentRow++;
        }

        return currentRow;
    }

    #endregion
}