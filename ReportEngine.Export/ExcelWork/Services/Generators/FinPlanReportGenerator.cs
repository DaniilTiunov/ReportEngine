using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Other;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Export.Mapping;
using ReportEngine.Shared.Config.IniHeleprs;

//using System.Diagnostics;

namespace ReportEngine.Export.ExcelWork.Services.Generators;

public class FinPlanReportGenerator : IReportGenerator
{
    private readonly IContainerRepository _containerRepository;
    private readonly IGenericBaseRepository<Container, Container> _genericBaseRepository;

    private readonly IProjectInfoRepository _projectInfoRepository;

    //IGenericBaseRepository<IBaseEquip, Container> не работает без сервиса
    public FinPlanReportGenerator(IProjectInfoRepository projectInfoRepository,
        IContainerRepository containerRepository,
        IGenericBaseRepository<Container, Container> genericBaseRepository)
    {
        _projectInfoRepository = projectInfoRepository;
        _containerRepository = containerRepository;
        _genericBaseRepository = genericBaseRepository;
    }

    public ReportType Type => ReportType.FinPlanReport;


    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);

        using (var wb = new XLWorkbook())
        {
            var ws = wb.Worksheets.Add("MainSheet");


            var activeRow = 1;
            ;
            activeRow = CreateWorksheetTableHeader(ws, activeRow);
            ;
            activeRow++;
            ;
            activeRow = CreateProjectInformationTable(ws, project, activeRow);
            ;
            activeRow++;
            ;
            activeRow = await CreateSelfcostTable(ws, project, activeRow).ConfigureAwait(false);
            ;
            activeRow += 2;
            ;
            CreateRentTable(ws, project, activeRow);
            ;

            ws.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cells().Style.Alignment.WrapText = true;
            ws.Columns().AdjustToContents();

            var savePath = SettingsManager.GetReportDirectory();

            var fileName = ExcelReportHelper.CreateReportName("Финплан", "xlsx");
            var fullSavePath = Path.Combine(savePath, fileName);

            // Debug.WriteLine("Отчёт сохранён: " + fullSavePath);
            wb.SaveAs(fullSavePath);
        }
    }


    private int CreateWorksheetTableHeader(IXLWorksheet ws, int startRow)
    {
        var activeRow = startRow;

        var headerRange = ws.Range($"A{activeRow}:I{activeRow}");

        headerRange.Merge();
        headerRange.Value = "Финансовый план СТЕНДЫ";


        headerRange.Style.Font.FontSize = 16;
        headerRange.Style.Font.SetBold();

        activeRow++;

        return activeRow;
    }

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
        var activeRow = startRow;

        var projectContainers = await _containerRepository.GetAllByProjectIdAsync(project.Id);


        var headerRange = ws.Range($"A{activeRow}:I{activeRow}");
        headerRange.Merge();
        headerRange.Value = "Себестоимость";

        headerRange.Style.Font.FontSize = 14;
        headerRange.Style.Font.SetBold();

        activeRow++;

        var materialAndEquipmentCost = project.Stands
            .Select(stand => stand.StandSummCost)
            .Sum();

        var containers = projectContainers.SelectMany(batch => batch.Containers)
            .GroupBy(c => c.Name);


        var repContainers = await _genericBaseRepository.GetAllAsync();


        var results = repContainers.Join(
            containers,
            right => right.Name,
            left => left.Key,
            (right, left) => right);


        return activeRow;
    }

    private void CreateRentTable(IXLWorksheet ws, ProjectInfo project, int startRow)
    {
        var activeRow = startRow;

        var tableRecords = new List<(string name, float price, string unit)>();

        tableRecords.Add(("Стоимость продажи", 0.0f, "руб. без НДС"));
        tableRecords.Add(("Маржинальный доход", 0.0f, "руб"));
        tableRecords.Add(("Наценка", 0.0f, "%"));
        tableRecords.Add(("Ожидаемая рентабельность", 0.0f, "%"));



        var headerRange = ws.Range($"A{activeRow}:I{activeRow}").Merge();
        headerRange.Value = "Стоимость продажи и рентабельность";
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Font.SetBold();

        activeRow++;

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



    }




}