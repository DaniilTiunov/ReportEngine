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

public class ProductionReportGenerator : IReportGenerator
{
    private readonly IProjectInfoRepository _projectInfoRepository;

    public ProductionReportGenerator(IProjectInfoRepository projectInfoRepository)
    {
        _projectInfoRepository = projectInfoRepository;
    }

    ReportType IReportGenerator.Type => ReportType.ProductionReport;

    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);

        using (var wb = new XLWorkbook())
        {
            var ws = wb.Worksheets.Add("Ведомость");

            var activeRow = CreateCommonHeader(ws, project);

            activeRow = CreateStandTableHeader(ws, project, activeRow);
            activeRow = FillStandsTable(ws, project, activeRow);

            activeRow = CreateEquipmentTableHeader(ws, project, activeRow);
            activeRow = FillEquipmentTable(ws, project, activeRow);


            ws.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cells().Style.Alignment.WrapText = true;
            ws.Columns().AdjustToContents();

            var savePath = SettingsManager.GetReportDirectory();

            var fileName = ExcelReportHelper.CreateReportName("Производство", "xlsx");
            var fullSavePath = Path.Combine(savePath, fileName);

            Debug.WriteLine("Отчёт сохранён: " + fullSavePath);
            wb.SaveAs(fullSavePath);
        }
    }

    #region Заголовки

    //создание общего заголовка
    private int CreateCommonHeader(IXLWorksheet ws, ProjectInfo project)
    {
        var activeRow = 1;

        var headerRange = ws.Range($"A{activeRow}:D{activeRow}");

        ws.Cell($"A{activeRow}").Value = $"{project.Company}";
        ws.Cell($"B{activeRow}").Value = $"{project.OrderCustomer}";
        ws.Cell($"C{activeRow}").Value = $"{project.Description}";

        headerRange.Style.Font.SetBold();
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        activeRow++;
        return activeRow;
    }

    //создание шапки таблицы стендов
    private int CreateStandTableHeader(IXLWorksheet ws, ProjectInfo project, int row)
    {
        var activeRow = row;

        var projectStringArea = ws.Range($"A{activeRow}:D{activeRow}").Merge();
        projectStringArea.Value = "Проект целиком";
        projectStringArea.Style.Font.SetBold();

        activeRow++;

        ws.Cell($"A{activeRow}").Value = "Наименование";

        var kksCodeStringArea = ws.Range($"B{activeRow}:C{activeRow}").Merge();
        kksCodeStringArea.Value = "Код KKS";

        ws.Cell($"D{activeRow}").Value = "Серийный номер";

        var headerRange = ws.Range($"A{activeRow}:D{activeRow}");
        headerRange.Style.Font.SetBold();

        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell($"A{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;


        activeRow++;

        return activeRow;
    }

    private int CreateEquipmentTableHeader(IXLWorksheet ws, ProjectInfo project, int row)
    {
        var activeRow = row;

        //область всего заголовка
        var headerRange = ws.Range($"A{activeRow}:D{activeRow + 1}");

        //область заголовка "сводная ведомость комплектующих"
        var componentsStringHeaderRange = ws.Range($"A{activeRow}:D{activeRow}").Merge();
        componentsStringHeaderRange.Value = "Сводная ведомость комплектующих";

        activeRow++;

        //названия столбцов сводной ведомости
        ws.Cell($"A{activeRow}").Value = "Наименование";
        ws.Cell($"B{activeRow}").Value = "Ед.изм.";
        ws.Cell($"C{activeRow}").Value = "Кол-во (По проекту)";
        ws.Cell($"D{activeRow}").Value = "Кол-во (По факту)";

        headerRange.Style.Font.SetBold();
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell($"A{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

        activeRow++;

        return activeRow;
    }

    //создает подзаголовок для подтаблицы и возвращает следующую строку
    private int CreateSubheaderOnWorksheet(int row, string title, IXLWorksheet ws)
    {
        var activeRow = row;

        var subHeaderRange = ws.Range($"A{activeRow}:D{activeRow}");
        subHeaderRange.Merge();
        subHeaderRange.Value = title;
        subHeaderRange.Style.Font.SetFontSize(10);
        subHeaderRange.Style.Font.SetBold();

        activeRow++;
        return activeRow;
    }

    #endregion

    #region Заполнители

    //создание таблицы стендов
    private int FillStandsTable(IXLWorksheet ws, ProjectInfo project, int startRow)
    {
        var activeRow = startRow;

        //выводим стенды
        var standsRecords = project.Stands.Select(stand => new StandInfoData
        {
            Name = new ValidatedField<string?>(stand.Design, true),
            KKS = new ValidatedField<string?>(stand.KKSCode, true),
            SerialNumber = new ValidatedField<string?>(stand.SerialNumber, true),
        });


        foreach (var standRecord in standsRecords)
        {
            PasteStandRecord(activeRow, standRecord, ws);

            ws.Cell($"A{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Range($"B{activeRow}:C{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell($"D{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            activeRow++;
        }


        //формируем общее кол-во стендов
        var standsQuantityStringArea = ws.Range($"A{activeRow}:C{activeRow}").Merge();
        standsQuantityStringArea.Value = "Количество стендов по отчету";

        ws.Cell($"D{activeRow}").Value = standsRecords.Count();


        var floorTableArea = ws.Range($"A{activeRow}:D{activeRow}");

        floorTableArea.Style.Font.SetBold();

        activeRow++;


        return activeRow;
    }
    //создание сводной ведомости комплектующих
    private int FillEquipmentTable(IXLWorksheet ws, ProjectInfo project, int startRow)
    {
        var activeRow = startRow;

        var generatedData = ExcelReportHelper.GeneratePartsData(project.Stands);

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

        return activeRow;
    }

    //Заполняет подтаблицу и возвращает следующую строку
    private int FillSubtableData(int startRow, List<EquipmentRecord> items, IXLWorksheet ws)
    {
        var currentRow = startRow;

        foreach (var item in items)
        {
            PastePartRecord(currentRow, item, ws);

            ws.Cell($"A{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell($"B{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell($"C{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            currentRow++;
        }

        return currentRow;
    }

    #endregion


    #region Вспомогательные

    //валидация и вывод в таблицу
    private void PastePartRecord(int row, EquipmentRecord record, IXLWorksheet ws)
    {

        if (record.Name.Value != null)
        {
            ws.Cell($"A{row}").Value = record.Name.Value.ToString();
        }
        if (!record.Name.IsValid)
        {
            ws.Cell($"A{row}").Value += "\n" + ExcelReportHelper.CommonErrorString;
        }



        if (record.Unit.Value != null)
        {
            ws.Cell($"B{row}").Value = record.Unit.Value.ToString();
        }
        if (!record.Unit.IsValid)
        {
            ws.Cell($"B{row}").Value += "\n" + ExcelReportHelper.CommonErrorString;
        }



        if (record.Quantity.Value.HasValue)
        {
            ws.Cell($"C{row}").Value = record.Quantity.Value.ToString();
        }
        if (!record.Quantity.IsValid)
        {
            ws.Cell($"C{row}").Value += "\n" + ExcelReportHelper.CommonErrorString;
        }
    }
    //валидация и вывод в таблицу
    private void PasteStandRecord(int row, StandInfoData record, IXLWorksheet ws)
    {

        if (record.Name.Value != null)
        {
            ws.Cell($"A{row}").Value = record.Name.Value.ToString();
        }
        if (!record.Name.IsValid)
        {
            ws.Cell($"A{row}").Value += "\n" + ExcelReportHelper.CommonErrorString;
        }

        if (record.KKS.Value != null)
        {
            ws.Range($"B{row}:C{row}").Merge().FirstCell().Value = record.KKS.Value.ToString();
        }
        if (!record.KKS.IsValid)
        {
            ws.Range($"B{row}:C{row}").Merge().FirstCell().Value += "\n" + ExcelReportHelper.CommonErrorString;
        }

        if (record.SerialNumber.Value != null)
        {
            ws.Cell($"D{row}").Value = record.SerialNumber.Value.ToString();
        }
        if (!record.SerialNumber.IsValid)
        {
            ws.Cell($"D{row}").Value += "\n" + ExcelReportHelper.CommonErrorString;
        }

    }

    #endregion
}