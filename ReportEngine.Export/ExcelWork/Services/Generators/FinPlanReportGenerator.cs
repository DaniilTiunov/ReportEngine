using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Other;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Generators.DTO;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Export.Mapping;
using ReportEngine.Shared.Config.IniHeleprs;
using System.Threading.Tasks;


namespace ReportEngine.Export.ExcelWork.Services.Generators;



public class FinPlanReportGenerator : IReportGenerator
{
    

    private readonly IProjectInfoRepository _projectInfoRepository;
    private readonly IContainerRepository _containerRepository;

    public FinPlanReportGenerator(IProjectInfoRepository projectInfoRepository,
                                    IContainerRepository containerRepository)
    {
        _projectInfoRepository = projectInfoRepository;
        _containerRepository = containerRepository;
    }

    public ReportType Type => ReportType.FinPlanReport;



    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);

        using (var wb = new XLWorkbook())
        {
            wb.Worksheets.Add("Стоимость продажи");
            wb.Worksheets.Add("Наценка");

            var activeRow = 1;

            foreach (var ws in wb.Worksheets)
            {
                activeRow = 1;

                activeRow = CreateHeader(activeRow, "Финансовый план СТЕНДЫ", ws);
                activeRow++;


                //заполняем информацию о проекте
                activeRow = CreateProjectInformationTable(ws, project, activeRow);
                activeRow++;

                //заполняем таблицу себестоимости
                activeRow = CreateHeader(activeRow, "Себестоимость", ws);
                activeRow = await CreateSelfcostTable(ws, project, activeRow);

                activeRow += 2;

                //заполняем таблицу рентабельности
                activeRow = CreateHeader(activeRow, "Стоимость продажи и рентабельность", ws);
                activeRow = CreateRentTable(ws, project, activeRow);

                ws.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                ws.Cells().Style.Alignment.WrapText = true;
                ws.Columns().AdjustToContents();


            }
            
            

            var savePath = SettingsManager.GetReportDirectory();

            var fileName = ExcelReportHelper.CreateReportName("Финплан", "xlsx");
            var fullSavePath = Path.Combine(savePath, fileName);

            wb.SaveAs(fullSavePath);
        }
    }



    #region Вспомогательные

    private IXLRange PasteRecord(int row, ReportRecordData record, IXLWorksheet ws)
    {
        var recordNameRange = ws.Range($"A{row}:E{row}").Merge();
        recordNameRange.Value = record.Name.Value;
        recordNameRange.Style.Font.SetBold();
        recordNameRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        var recordPriceRange = ws.Range($"F{row}:G{row}").Merge();
        recordPriceRange.Value = record.CommonCost.Value.ToString();
        recordPriceRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        var unitPriceRange = ws.Range($"H{row}:I{row}").Merge();
        unitPriceRange.Value = record.Unit.Value;
        unitPriceRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


        return recordPriceRange;
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



    #region Заполнители
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

    private async Task<int> CreateSelfcostTable(IXLWorksheet ws, ProjectInfo project, int startRow)
    {
        var containerBatches = await _containerRepository.GetAllByProjectIdAsync(project.Id);

        var activeRow = startRow;




        var sumCellList = new List<IXLRange>();

            
        var generatedEquipmentsData = ExcelReportHelper.GeneratePartsData(project.Stands);
        var equipmentRecords = ExcelReportHelper.GenerateAllPartsCollection(generatedEquipmentsData);
        var equipmentTotalCostRecord = ExcelReportHelper.GenerateTotalRecord(equipmentRecords);

        equipmentTotalCostRecord.Name = new ValidatedField<string?>("Стоимость оборудования и материалов",true);
        equipmentTotalCostRecord.Unit = new ValidatedField<string?>("руб. без НДС", true);

        var equimpmentValueRange = PasteRecord(activeRow, equipmentTotalCostRecord, ws);
        sumCellList.Add(equimpmentValueRange);

        activeRow++;




        
        
        var containers = ExcelReportHelper.GenerateContainersData(containerBatches);
        var containersTotalCost = ExcelReportHelper.GenerateTotalRecord(containers);

        var containersCostRecord = new ReportRecordData
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string>("Тара", true),
            Unit = new ValidatedField<string?>("руб. без НДС", true),
            Quantity = new ValidatedField<float?>(null, true),
            CostPerUnit = new ValidatedField<float?>(null, true),
            CommonCost = new ValidatedField<float?>(containersTotalCost.CommonCost.Value, true)
        };

        var containersValueRange = PasteRecord(activeRow, containersCostRecord, ws);
        sumCellList.Add(containersValueRange);

        activeRow++;

        
       

        PasteSeparatorRow(activeRow, ws);
        activeRow++;



        var generatedLaborData = ExcelReportHelper.GenerateLaborData(project.Stands);
        var laborRecords = ExcelReportHelper.GenerateAllLaborsCollection(generatedLaborData);
        var laborTotalCostRecord = ExcelReportHelper.GenerateTotalRecord(laborRecords);

        laborTotalCostRecord.Name = new ValidatedField<string>("Трудозатраты", true);
        laborTotalCostRecord.Unit = new ValidatedField<string?>("чел. * мес.", true);

        var laborValueRange = PasteRecord(activeRow, laborTotalCostRecord, ws);
        sumCellList.Add(laborValueRange);
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

        var laborFundValueRange = PasteRecord(activeRow, laborFundRecord, ws);
        sumCellList.Add(laborFundValueRange);
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

        var bussinessTripValueRange = PasteRecord(activeRow, bussinessTripCostsRecord, ws);
        sumCellList.Add(bussinessTripValueRange);
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

        var customerDeliveryValueRange = PasteRecord(activeRow, customerDeliveryRecord, ws);
        sumCellList.Add(customerDeliveryValueRange);
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

        var transportAndPrepareWorkValueRange = PasteRecord(activeRow, transportAndPrepareWork, ws);
        sumCellList.Add(transportAndPrepareWorkValueRange);
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


        var cellsAdresses = sumCellList.Select(range => range.FirstCell().Address.ToString());
        string formulaString = "=" + String.Join("+", cellsAdresses); 
        ws.Range($"F{activeRow}:G{activeRow}").FirstCell().FormulaA1 = formulaString;

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

        PasteRecord(activeRow, unexpectedExpensesRecord, ws);
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

        PasteRecord(activeRow, totalRecord, ws);


        var tableRange = ws.Range($"A{startRow}:I{activeRow}");
        tableRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
        tableRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);


        activeRow++;
        return activeRow;
    }

    private int CreateRentTable(IXLWorksheet ws, ProjectInfo project, int startRow)
    {
        var activeRow = startRow;

        var tableRecords = new List<ReportRecordData>
        {
            new ReportRecordData() {
                Name = new ValidatedField<string?>("Стоимость продажи", true),
                Unit = new ValidatedField<string?>("руб. без НДС", true),
                CommonCost = new ValidatedField<float?>(0.0f, true) },

             new ReportRecordData() {
                Name = new ValidatedField<string?>("Маржинальный доход", true),
                Unit = new ValidatedField<string?>("руб.", true),
                CommonCost = new ValidatedField<float?>(0.0f, true) },

              new ReportRecordData() {
                Name = new ValidatedField<string?>("Наценка", true),
                Unit = new ValidatedField<string?>("%", true),
                CommonCost = new ValidatedField<float?>(0.0f, true) },

               new ReportRecordData() {
                Name = new ValidatedField<string?>("Ожидаемая рентабельность", true),
                Unit = new ValidatedField<string?>("%", true),
                CommonCost = new ValidatedField<float?>(0.0f, true) }
        };


        foreach (var record in tableRecords)
        {
            PasteRecord(activeRow, record, ws);
            activeRow++;
        }

        var tableRange = ws.Range($"A{startRow}:I{activeRow-1}");
        tableRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
        tableRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

        return activeRow;

    }
    
    #endregion
}