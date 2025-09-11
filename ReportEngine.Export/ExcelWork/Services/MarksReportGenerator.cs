using System.Diagnostics;
using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs;

namespace ReportEngine.Export.ExcelWork.Services;

public class MarksReportGenerator : IReportGenerator
{
    private readonly IProjectInfoRepository _projectInfoRepository;
    public ReportType Type => ReportType.MarksReport;
    
    public MarksReportGenerator(IProjectInfoRepository projectInfoRepository)
    {
        _projectInfoRepository = projectInfoRepository;
    }
    
    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);

        var templatePath = DirectoryHelper.GetReportsTemplatePath("Маркировка");
        var fileName = "Маркировка___" + DateTime.Now.ToString("yy-MM-dd___HH-mm-ss") + ".xlsx";

        var savePath = SettingsManager.GetReportDirectory();
        var fullSavePath = Path.Combine(savePath, fileName);


        using (var wb = new XLWorkbook(templatePath))
        {
            var ws = wb.Worksheets.Add("MainSheet");

            CreateTableHeader(ws);
            FillWorksheet(ws, project);

            ws.Columns().AdjustToContents();
            ws.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            Debug.WriteLine("Отчёт сохранён: " + fullSavePath);
            wb.SaveAs(fullSavePath);
        }
    }

    private void CreateTableHeader(IXLWorksheet ws)
    {
        var headerRange = ws.Range("A1:D1");

        headerRange.Cell(1, 1).Value = "№";
        headerRange.Cell(1, 2).Value = "KKS стенда";
        headerRange.Cell(1, 3).Value = "KKS изм. контура (датчика)";
        headerRange.Cell(1, 4).Value = "Маркировка";

        headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);

        headerRange.Style.Font.SetBold();
    }

    private void FillWorksheet(IXLWorksheet ws, ProjectInfo project)
    {
        //формируем все нуеобходимые записи
        var allRecords = project.Stands
            .SelectMany(stand => stand.ObvyazkiInStand
            .SelectMany(obvyazka => CreateObvyazkaRecords(obvyazka, stand)))
            .ToList();

        var recordNumber = 1;
        const int recordRowOffset = 2;

        foreach (var item in allRecords)
        {
            var upperRecordRow = recordNumber * recordRowOffset;
            var lowerRecordRow = upperRecordRow + 1;

            // Объединение ячеек для номера записи
            ws.Range($"A{upperRecordRow}:A{lowerRecordRow}").Merge().Value = recordNumber;

            // Объединение ячеек для standKKS
            ws.Range($"B{upperRecordRow}:B{lowerRecordRow}").Merge().Value = item.StandKKS;

            // Объединение ячеек для sensorKKS
            ws.Range($"C{upperRecordRow}:C{lowerRecordRow}").Merge().Value = item.SensorKKS;

            // Запись разных значений в D (без объединения)
            ws.Cell($"D{upperRecordRow}").Value = item.SensorMarkPlus;
            ws.Cell($"D{lowerRecordRow}").Value = item.SensorMarkMinus;

            recordNumber++;
        }
    }

    //формирует список записей для одной обвязки
    private List<RecordData> CreateObvyazkaRecords(ObvyazkaInStand obvyazka, Stand stand)
    {
        var resultRecords = new List<RecordData>();

        if (obvyazka.FirstSensorKKS != null)
            resultRecords.Add(new RecordData(stand.KKSCode,
                obvyazka.FirstSensorKKS,
                obvyazka.FirstSensorMarkPlus ?? "",
                obvyazka.FirstSensorMarkMinus ?? ""));

        if (obvyazka.SecondSensorKKS != null)
            resultRecords.Add(new RecordData(stand.KKSCode,
                obvyazka.SecondSensorKKS,
                obvyazka.SecondSensorMarkPlus ?? "",
                obvyazka.SecondSensorMarkMinus ?? ""));

        if (obvyazka.ThirdSensorKKS != null)
            resultRecords.Add(new RecordData(stand.KKSCode,
                obvyazka.ThirdSensorKKS,
                obvyazka.ThirdSensorMarkPlus ?? "",
                obvyazka.ThirdSensorMarkMinus ?? ""));

        return resultRecords;
    }

    //структура для одной записи таблицы
    public struct RecordData
    {
        public string StandKKS;
        public string SensorKKS;
        public string SensorMarkPlus;
        public string SensorMarkMinus;

        public RecordData(string standKKS, string sensorKKS, string sensorMarkPlus, string sensorMarkMinus)
        {
            StandKKS = standKKS;
            SensorKKS = sensorKKS;
            SensorMarkPlus = sensorMarkPlus;
            SensorMarkMinus = sensorMarkMinus;
        }
    }
}