using System.Diagnostics;
using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Export.Mapping;
using ReportEngine.Shared.Config.IniHeleprs;

namespace ReportEngine.Export.ExcelWork.Services.Generators;

public class ContainerReportGenerator : IReportGenerator
{
    private readonly IContainerRepository _containerRepository;
    private readonly IProjectInfoRepository _projectInfoRepository;

    public ContainerReportGenerator(IProjectInfoRepository projectInfoRepository,
        IContainerRepository containerRepository)
    {
        _projectInfoRepository = projectInfoRepository;
        _containerRepository = containerRepository;
    }

    public ReportType Type => ReportType.ContainerReport;

    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);

        using (var wb = new XLWorkbook())
        {
            var ws = wb.Worksheets.Add("MainSheet");

            CreateWorksheetTableHeader(ws);
            await FillWorksheetTable(ws, project);


            ws.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cells().Style.Alignment.WrapText = true;
            ws.Columns().AdjustToContents();

            var savePath = SettingsManager.GetReportDirectory();


            var fileName = ExcelReportHelper.CreateReportName("Тара", "xlsx");
            var fullSavePath = Path.Combine(savePath, fileName);

            Debug.WriteLine("Отчёт сохранён: " + fullSavePath);
            wb.SaveAs(fullSavePath);
        }
    }

    private void CreateWorksheetTableHeader(IXLWorksheet ws)
    {
        var headerRange = ws.Range("A1:I1");

        headerRange.Cell(1, 1).Value = "№ места";
        headerRange.Cell(1, 2).Value = "№ места в ящике";
        headerRange.Cell(1, 3).Value = "Наименование оборудования и комплектующих";
        headerRange.Cell(1, 4).Value = "Серийный №";
        headerRange.Cell(1, 5).Value = "Код KKS";
        headerRange.Cell(1, 6).Value = "Количество";
        headerRange.Cell(1, 7).Value = "Ширина рамы, мм";
        headerRange.Cell(1, 8).Value = "Масса, кг";
        headerRange.Cell(1, 9).Value = "Упаковка";


        headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);

        headerRange.Style.Font.SetBold();
    }

    private async Task FillWorksheetTable(IXLWorksheet ws, ProjectInfo project)
    {
        const string dbErrorString = "Ошибка загрузки данных из БД";
        //создаем объекты всех контейнеров
        var containerBatches = await _containerRepository.GetAllByProjectIdAsync(project.Id);


        var containers = containerBatches
            .SelectMany(batch => batch.Containers)
            .Where(container => container.Stands.Any())
            .Select(container => new
            {
                containerInstance = container,
                containerContent = container.Stands
            });


        // выводим в таблицу

        var containerStartRow = 2;
        var standActiveRow = containerStartRow;

        var containerNumber = 1;


        foreach (var container in containers)
        {
            var placeInContainerNumber = 1;


            //сначала выводим все стенды

            foreach (var stand in container.containerContent)
            {
                var placeInContainerString = $"{containerNumber}.{placeInContainerNumber}";

                ws.Cell($"B{standActiveRow}").Value = placeInContainerString;
                ws.Cell($"C{standActiveRow}").Value = stand.Design ?? dbErrorString;
                ws.Cell($"D{standActiveRow}").Value = stand.SerialNumber ?? dbErrorString;
                ws.Cell($"E{standActiveRow}").Value = stand.KKSCode ?? dbErrorString;
                ws.Cell($"F{standActiveRow}").Value = "1"; //пока костыль
                ws.Cell($"G{standActiveRow}").Value = stand.StandFrames.FirstOrDefault()?.Frame.Width;

                standActiveRow++;
                placeInContainerNumber++;
            }

            //вычисляем где закончили выводиться стенды
            //заполняем инфу о таре
            var containerEndRow = standActiveRow - 1;


            var containerNumberRange = ws.Range($"A{containerStartRow}:A{containerEndRow}").Merge();
            containerNumberRange.Value = containerNumber;

            var commonContainerWeight = (container.containerInstance.ContainerWeight ?? 0f) +
                                        container.containerInstance.StandsWeight;

            var containerWeightRange = ws.Range($"H{containerStartRow}:H{containerEndRow}").Merge();
            containerWeightRange.Value = commonContainerWeight;

            var containerPackRange = ws.Range($"I{containerStartRow}:I{containerEndRow}").Merge();
            containerPackRange.Value = container.containerInstance.Name;


            containerNumber++;

            containerStartRow = containerEndRow + 1;
            standActiveRow = containerStartRow;
        }
    }
}