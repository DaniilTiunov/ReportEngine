using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Other;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Generators.DTO;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Export.Mapping;
using ReportEngine.Shared.Config.IniHeleprs;

//using System.Diagnostics;

namespace ReportEngine.Export.ExcelWork.Services.Generators;



public class FinPlanReportGenerator : IReportGenerator
{
    

    private readonly IProjectInfoRepository _projectInfoRepository;

    //IGenericBaseRepository<IBaseEquip, Container> не работает без сервиса
    public FinPlanReportGenerator(IProjectInfoRepository projectInfoRepository)
    {
        _projectInfoRepository = projectInfoRepository;
    }

    public ReportType Type => ReportType.FinPlanReport;



    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);

        using (var wb = new XLWorkbook())
        {
            var ws = wb.Worksheets.Add("MainSheet");


            var activeRow = 1;
            
            activeRow = CreateHeader(activeRow, "Финансовый план СТЕНДЫ", ws);     
            activeRow++;

            activeRow = CreateProjectInformationTable(ws, project, activeRow);
            activeRow++;

            activeRow = CreateHeader(activeRow, "Себестоимость", ws);
            activeRow = CreateSelfcostTable(ws, project, activeRow);

            activeRow += 2;

            activeRow = CreateHeader(activeRow, "Стоимость продажи и рентабельность", ws);     
            activeRow = CreateRentTable(ws, project, activeRow);
            

            ws.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cells().Style.Alignment.WrapText = true;
            ws.Columns().AdjustToContents();

            var savePath = SettingsManager.GetReportDirectory();

            var fileName = ExcelReportHelper.CreateReportName("Финплан", "xlsx");
            var fullSavePath = Path.Combine(savePath, fileName);

            wb.SaveAs(fullSavePath);
        }
    }



    #region Вспомогательные
    private void PasteRecord(int row, ReportRecordData record, IXLWorksheet ws)
    {
        var recordNameRange = ws.Range($"A{row}:E{row}").Merge();
        recordNameRange.Value = record.Name.Value;
        recordNameRange.Style.Font.SetBold();
        recordNameRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        var recordPriceRange = ws.Range($"F{row}:G{row}").Merge();
        recordPriceRange.Value = record.CommonCost.Value;
        recordPriceRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        var unitPriceRange = ws.Range($"H{row}:I{row}").Merge();
        unitPriceRange.Value = record.Unit.Value;
        unitPriceRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
    }

    private void PasteSeparatorRow(int row, IXLWorksheet ws)
    {
        var emptyRowRange = ws.Range($"A{row}:I{row}").Merge();
        emptyRowRange.Value = "";
    }

    private int CreateHeader(int row, string headerTitle, IXLWorksheet ws)
    {
        var activeRow = row;

        var headerRange = ws.Range($"A{activeRow}:I{activeRow}").Merge();
        headerRange.Value = headerTitle;
        headerRange.Style.Font.SetBold();
        headerRange.Style.Font.SetFontSize(14);
        headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

        activeRow++;
        return activeRow;
    }

    #endregion


    private int CreateProjectInformationTable(IXLWorksheet ws, ProjectInfo project, int startRow)
    {
        var activeRow = startRow;

        var tableRecords = new Dictionary<string, string>
        {
            { "Заказчик:", $"{project.Company}" },
            { "Объект:", $"{project.Object}" },
            { "Заказ покупателя:", $"{project.OrderCustomer}" },
            { "Руководитель проекта:", $"{project.Manager}" }
        };

        foreach (var record in tableRecords)
        {
            var nameRange = ws.Range($"A{activeRow}:C{activeRow}").Merge();
            nameRange.Value = record.Key;

            nameRange.Style.Font.SetBold();
            nameRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

            var valueRange = ws.Range($"D{activeRow}:I{activeRow}").Merge(); ;
            valueRange.Value = record.Value;

            valueRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

            activeRow++;
        }


        return activeRow;
    }

    private int CreateSelfcostTable(IXLWorksheet ws, ProjectInfo project, int startRow)
    {
        var activeRow = startRow;

        var headerRange = ws.Range($"A{activeRow}:I{activeRow}");
        headerRange.Merge();
        headerRange.Value = "Себестоимость";
        headerRange.Style.Font.FontSize = 14;
        headerRange.Style.Font.SetBold();

        activeRow++;

        

        var generatedEquipmentsData = ExcelReportHelper.GeneratePartsData(project.Stands);
        var equipmentRecords = ExcelReportHelper.GenerateAllPartsCollection(generatedEquipmentsData);
        var equipmentTotalCostRecord = ExcelReportHelper.GenerateTotalRecord(equipmentRecords);

        equipmentTotalCostRecord.Name = new ValidatedField<string?>("Стоимость оборудования и материалов",true);
        equipmentTotalCostRecord.Unit = new ValidatedField<string?>("руб. без НДС", true);

        PasteRecord(activeRow, equipmentTotalCostRecord, ws);
        activeRow++;




        var containersCost = (float?) null;

        var containersCostRecord = new ReportRecordData
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string>("Тара", true),
            Unit = new ValidatedField<string?>("руб. без НДС", true),
            Quantity = new ValidatedField<float?>(null, true),
            CostPerUnit = new ValidatedField<float?>(containersCost, true),
            CommonCost = new ValidatedField<float?>(null, true)
        };



        PasteRecord(activeRow, containersCostRecord, ws); //тут тара
        activeRow++;

        PasteSeparatorRow(activeRow, ws);
        activeRow++;



        var generatedLaborData = ExcelReportHelper.GenerateLaborData(project.Stands);
        var laborRecords = ExcelReportHelper.GenerateAllLaborsCollection(generatedLaborData);
        var laborTotalCostRecord = ExcelReportHelper.GenerateTotalRecord(laborRecords);

        laborTotalCostRecord.Name = new ValidatedField<string>("Трудозатраты", true);
        laborTotalCostRecord.Unit = new ValidatedField<string?>("чел. * мес.", true);

        PasteRecord(activeRow, laborTotalCostRecord, ws);
        activeRow++;

        var laborFundRecord = new ReportRecordData
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Фонд оплаты труда", true),
            Unit = new ValidatedField<string?>("руб.", true),
            Quantity = new ValidatedField<float?>(null, true),
            CostPerUnit = new ValidatedField<float?>(null, true),
            CommonCost = new ValidatedField<float?>(null, true)
        };

        PasteRecord(activeRow, laborFundRecord, ws);
        activeRow++;

        PasteSeparatorRow(activeRow, ws);
        activeRow++;

        var bussinessTripCostsRecord = new ReportRecordData
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Командировочные расходы", true),
            Unit = new ValidatedField<string?>("чел. * мес.", true),
            Quantity = new ValidatedField<float?>(null, true),
            CostPerUnit = new ValidatedField<float?>(null, true),
            CommonCost = new ValidatedField<float?>(null, true)
        };

        PasteRecord(activeRow, bussinessTripCostsRecord, ws);
        activeRow++;

        var customerDeliveryRecord = new ReportRecordData
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Доставка до заказчика", true),
            Unit = new ValidatedField<string?>("руб. без НДС", true),
            Quantity = new ValidatedField<float?>(null, true),
            CostPerUnit = new ValidatedField<float?>(null, true),
            CommonCost = new ValidatedField<float?>(null, true)
        };

        PasteRecord(activeRow, customerDeliveryRecord, ws);
        activeRow++;



        var transportAndPrepareWork = new ReportRecordData
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Транспортно-заготовительные работы (1% от стоимости оборудования)", true),
            Unit = new ValidatedField<string?>("руб. без НДС", true),
            Quantity = new ValidatedField<float?>(null, true),
            CostPerUnit = new ValidatedField<float?>(null, true),
            CommonCost = new ValidatedField<float?>(equipmentTotalCostRecord.CommonCost.Value * 0.01f, true)
        };

        PasteRecord(activeRow, transportAndPrepareWork, ws);
        activeRow++;

        PasteSeparatorRow(activeRow, ws);   
        activeRow++;







        var summaryPlannedCostRecord = new ReportRecordData
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Суммарная плановая себестоимость", true),
            Unit = new ValidatedField<string?>("руб. без НДС", true),
            Quantity = new ValidatedField<float?>(null, true),
            CostPerUnit = new ValidatedField<float?>(null, true),
            CommonCost = new ValidatedField<float?>(null, true)
        };

        PasteRecord(activeRow, summaryPlannedCostRecord, ws);
        activeRow++;

        
        var unexpectedExpensesPercentRecord = new ReportRecordData
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Непредвиденные затраты", true),
            Unit = new ValidatedField<string?>("%", true),
            Quantity = new ValidatedField<float?>(null, true),
            CostPerUnit = new ValidatedField<float?>(null, true),
            CommonCost = new ValidatedField<float?>(null, true)
        };

        PasteRecord(activeRow, unexpectedExpensesPercentRecord, ws);
        activeRow++;

        var unexpectedExpensesRecord = new ReportRecordData
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Непредвиденные затраты", true),
            Unit = new ValidatedField<string?>("руб. без НДС", true),
            Quantity = new ValidatedField<float?>(null, true),
            CostPerUnit = new ValidatedField<float?>(null, true),
            CommonCost = new ValidatedField<float?>(null, true)
        };

        PasteRecord(activeRow, unexpectedExpensesPercentRecord, ws);
        activeRow++;

        var totalRecord = new ReportRecordData
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Итого:", true),
            Unit = new ValidatedField<string?>("руб. без НДС", true),
            Quantity = new ValidatedField<float?>(null, true),
            CostPerUnit = new ValidatedField<float?>(null, true),
            CommonCost = new ValidatedField<float?>(null, true)
        };

        PasteRecord(activeRow, unexpectedExpensesPercentRecord, ws);
        activeRow++;








        //var projectContainers = await _containerRepository.GetAllByProjectIdAsync(project.Id);







        //    ("Суммарная плановая себестоимость", 0.0f, "руб. без НДС"),
        //    ("Непредвиденные затраты", 0.0f, "%"),
        //    ("Непредвиденные затраты", 0.0f, "руб. без НДС"),
        //    ("Итого:", 0.0f, "руб. без НДС")
        //}
        //;



        //var materialAndEquipmentCost = project.Stands
        //    .Select(stand => stand.StandSummCost)
        //    .Sum();

        //var containers = projectContainers.SelectMany(batch => batch.Containers)
        //    .GroupBy(c => c.Name);


        //var repContainers = await _genericBaseRepository.GetAllAsync();


        //var results = repContainers.Join(
        //    containers,
        //    right => right.Name,
        //    left => left.Key,
        //    (right, left) => right);


        return activeRow;
    }

    private int CreateRentTable(IXLWorksheet ws, ProjectInfo project, int startRow)
    {
        var activeRow = startRow;

        var headerRange = ws.Range($"A{activeRow}:I{activeRow}").Merge();
        headerRange.Value = "Стоимость продажи и рентабельность";
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Font.SetBold();

        activeRow++;


        var tableRecords = new List<(string name, float price, string unit)>
        {
            ("Стоимость продажи", 0.0f, "руб. без НДС"),
            ("Маржинальный доход", 0.0f, "руб"),
            ("Наценка", 0.0f, "%"),
            ("Ожидаемая рентабельность", 0.0f, "%")
        };


        foreach (var record in tableRecords)
        {
            var titleRange = ws.Range($"A{activeRow}:E{activeRow}").Merge();
            titleRange.Value = record.name;

            var valueRange = ws.Range($"F{activeRow}:G{activeRow}").Merge();
            valueRange.Value = record.price;

            var unitRange = ws.Range($"H{activeRow}:I{activeRow}").Merge();
            unitRange.Value = record.unit;

            activeRow++;
        }

        return activeRow;

    }




}