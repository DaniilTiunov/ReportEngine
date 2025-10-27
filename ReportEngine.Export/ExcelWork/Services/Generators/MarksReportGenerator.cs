using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Export.Mapping;
using ReportEngine.Shared.Config.IniHeleprs;
using System.Diagnostics;

namespace ReportEngine.Export.ExcelWork.Services.Generators;

public class MarksReportGenerator : IReportGenerator
{
    private readonly IProjectInfoRepository _projectInfoRepository;

    public MarksReportGenerator(IProjectInfoRepository projectInfoRepository)
    {
        _projectInfoRepository = projectInfoRepository;
    }

    public ReportType Type => ReportType.MarksReport;

    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);


        using (var wb = new XLWorkbook())
        {
            var ws = wb.Worksheets.Add("Проект");

            CreateWorksheetTableHeader(ws);
            FillWorksheetTable(ws, project);


            ws.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;


            ws.Cells().Style.Alignment.WrapText = true;
            ws.Columns().AdjustToContents();

            var savePath = SettingsManager.GetReportDirectory();

            var fileName = ExcelReportHelper.CreateReportName("Маркировка", "xlsx");
            var fullSavePath = Path.Combine(savePath, fileName);

            Debug.WriteLine("Отчёт сохранён: " + fullSavePath);
            wb.SaveAs(fullSavePath);
        }
    }

    private void CreateWorksheetTableHeader(IXLWorksheet ws)
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

    private void FillWorksheetTable(IXLWorksheet ws, ProjectInfo project)
    {
        //формируем все необходимые записи
        var allRecords = project.Stands
            .SelectMany(
                stand => stand.ObvyazkiInStand,
                (stand, obv) => new
                {
                    selectedStand = stand,
                    obvyazka = obv
                })
            .SelectMany(obvInfo => CreateObvyazkaRecords(obvInfo.obvyazka, obvInfo.selectedStand))
            .ToList();

        var recordNumber = 1;
        const int recordRowOffset = 2;


        //выводим сформированный список
        foreach (var item in allRecords)
        {
            var upperRecordRow = recordNumber * recordRowOffset;
            var lowerRecordRow = upperRecordRow + 1;


            ws.Range($"A{upperRecordRow}:A{lowerRecordRow}").Merge().Value = recordNumber;


            ws.Range($"B{upperRecordRow}:B{lowerRecordRow}").Merge().Value =
                $"{item.StandKKS} ({item.StandSerialNumber})";


            ws.Range($"C{upperRecordRow}:C{lowerRecordRow}").Merge().Value = item.SensorKKS;


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
            resultRecords.Add(new RecordData(
                stand.SerialNumber ?? "",
                stand.KKSCode ?? "",
                obvyazka.FirstSensorKKS,
                obvyazka.FirstSensorMarkPlus ?? "",
                obvyazka.FirstSensorMarkMinus ?? ""));

        if (obvyazka.SecondSensorKKS != null)
            resultRecords.Add(new RecordData(
                stand.SerialNumber ?? "",
                stand.KKSCode ?? "",
                obvyazka.SecondSensorKKS,
                obvyazka.SecondSensorMarkPlus ?? "",
                obvyazka.SecondSensorMarkMinus ?? ""));

        if (obvyazka.ThirdSensorKKS != null)
            resultRecords.Add(new RecordData(
                stand.SerialNumber ?? "",
                stand.KKSCode ?? "",
                obvyazka.ThirdSensorKKS,
                obvyazka.ThirdSensorMarkPlus ?? "",
                obvyazka.ThirdSensorMarkMinus ?? ""));

        return resultRecords;
    }


    //структура для одной записи таблицы
    public struct RecordData
    {
        public string StandSerialNumber;
        public string StandKKS;
        public string SensorKKS;
        public string SensorMarkPlus;
        public string SensorMarkMinus;


        public RecordData(string standSerialNumber, string standKKS, string sensorKKS, string sensorMarkPlus,
            string sensorMarkMinus)
        {
            StandSerialNumber = standSerialNumber;
            StandKKS = standKKS;
            SensorKKS = sensorKKS;
            SensorMarkPlus = sensorMarkPlus;
            SensorMarkMinus = sensorMarkMinus;
        }
    }
}