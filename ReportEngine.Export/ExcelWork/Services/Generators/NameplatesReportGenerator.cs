using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Export.Mapping;
using ReportEngine.Shared.Config.IniHeleprs;
using System.Diagnostics;

namespace ReportEngine.Export.ExcelWork.Services.Generators;

public class NameplatesReportGenerator : IReportGenerator
{
    private readonly IProjectInfoRepository _projectInfoRepository;


    public NameplatesReportGenerator(IProjectInfoRepository projectInfoRepository)
    {
        _projectInfoRepository = projectInfoRepository;
    }


    public ReportType Type => ReportType.NameplatesReport;


    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);

        using (var wb = new XLWorkbook())
        {
            wb.Worksheets.ToList().ForEach(ws => ws.Delete());

            var ws = wb.Worksheets.Add("Sheet1");

            var maxTablesQuantity = FillWorksheetTable(ws, project);
            CreateWorksheetTableHeader(ws, maxTablesQuantity);


            ws.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Columns().AdjustToContents();
            ws.Cells().Style.Alignment.WrapText = true;

            var savePath = SettingsManager.GetReportDirectory();
            var fileName = ExcelReportHelper.CreateReportName("Ведомость_шильдиков_табличек", "xlsx");


            var fullSavePath = Path.Combine(savePath, fileName);

            Debug.WriteLine("Отчёт сохранён: " + fullSavePath);
            wb.SaveAs(fullSavePath);
        }
    }


    private void CreateWorksheetTableHeader(IXLWorksheet ws, int maxTablesQuantity)
    {
        const int headerRow = 1;

        var startColumn = 1;
        var endColumn = startColumn;


        ws.Cell(headerRow, startColumn).Value = "№";


        //рассчитываем шапку для шильдиков
        startColumn = 2;

        var nameplatesHeaderCell = ws.Cell(headerRow, startColumn);

        var nameplatesHeaderContent = "Шильдик (40х56)\n";
        nameplatesHeaderContent += "ЭП-С.000.0000.001";
        nameplatesHeaderCell.Value = nameplatesHeaderContent;


        //рассчитываем шапку для табличек
        startColumn = 3;

        endColumn = maxTablesQuantity > 1 ? startColumn + maxTablesQuantity - 1 : startColumn;

        var startCell = ws.Cell(headerRow, startColumn);
        var endCell = ws.Cell(headerRow, endColumn);

        var tablesHeaderArea = ws.Range(startCell, endCell).Merge();


        var tablesHeaderContent = "Табличка (26х80)\n";
        tablesHeaderContent += "ЭП-С.000.0000.003";
        tablesHeaderArea.Value = tablesHeaderContent;
    }

    private int FillWorksheetTable(IXLWorksheet ws, ProjectInfo project)
    {
        var stands = project.Stands;

        var maxTables = 0;

        var activeRow = 2;
        var standNumber = 1;

        foreach (var stand in stands)
        {
            ws.Cell("A" + activeRow).Value = standNumber;


            //формируем текст шильдика
            var standNameplateText = "Стенд датчиков КИПиА\n";
            standNameplateText += $"{stand.KKSCode}\n";
            standNameplateText += $"{stand.SerialNumber}\n";
            standNameplateText += $"Дата: {DateTime.Now.ToString("MM.yyyy")}";


            //формируем текста табличек
            var standTablesStrings = stand.ObvyazkiInStand
                .SelectMany(obv => CreateSensorsListFromObvyazka(obv))
                .Select(record =>
                {
                    var nameplateText = $"{record.SensorDescription}\n";
                    nameplateText += $"{record.SensorKKS}";

                    return nameplateText;
                });


            var activeColumn = 2;

            ws.Cell(activeRow, activeColumn).Value = standNameplateText;
            activeColumn++;


            //растягиваем все найденные шильдики вдоль строки после табличек
            foreach (var tableText in standTablesStrings)
            {
                ws.Cell(activeRow, activeColumn).Value = tableText;

                activeColumn++;
            }


            maxTables = Math.Max(maxTables, standTablesStrings.Count());

            standNumber++;
            activeRow++;
        }


        //отдаем максимум найденных табличек для построения шапки
        return maxTables;
    }


    private List<RecordData> CreateSensorsListFromObvyazka(ObvyazkaInStand obv)
    {
        var resultRecords = new List<RecordData>();

        if (obv.FirstSensorKKS != null)
            resultRecords.Add(new RecordData(
                obv.FirstSensorKKS,
                obv.FirstSensorDescription ?? ""));

        if (obv.SecondSensorKKS != null)
            resultRecords.Add(new RecordData(
                obv.SecondSensorKKS,
                obv.SecondSensorDescription ?? ""));

        if (obv.ThirdSensorKKS != null)
            resultRecords.Add(new RecordData(
                obv.ThirdSensorKKS,
                obv.ThirdSensorDescription ?? ""));

        return resultRecords;
    }


    //структура для одной записи таблицы
    public struct RecordData
    {
        public string SensorKKS;
        public string SensorDescription;

        public RecordData(string sensorKKS, string sensorDescription)
        {
            SensorKKS = sensorKKS;
            SensorDescription = sensorDescription;
        }
    }
}