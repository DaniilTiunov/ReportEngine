using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office.Y2022.FeaturePropertyBag;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.IniHeleprs;
using System.Diagnostics;

namespace ReportEngine.Export.ExcelWork.Services
{
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

                var quantityInfo = FillWorksheetTable(ws, project);
                CreateWorksheetTableHeader(ws, quantityInfo);



                ws.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                ws.Columns().AdjustToContents();

                var savePath = SettingsManager.GetReportDirectory();
                var fileName = "Ведомость_шильдиков_табличек___" + DateTime.Now.ToString("dd-MM-yy___HH-mm-ss") + ".xlsx";


                var fullSavePath = Path.Combine(savePath, fileName);

                Debug.WriteLine("Отчёт сохранён: " + fullSavePath);
                wb.SaveAs(fullSavePath);
            }
        }


        private void CreateWorksheetTableHeader(IXLWorksheet ws, (int maxTablesQuantity, int maxNameplatesQuantity) quantityInfo)
        {
            const int headerRow = 1;

            int startColumn = 1;

            ws.Cell(headerRow, startColumn).Value = "№";


            //рассчитываем шапку для шильдиков
            startColumn = 2;
            int endColumn = quantityInfo.maxTablesQuantity>0 ? startColumn+quantityInfo.maxTablesQuantity-1 : startColumn;

            var startCell = ws.Cell(headerRow,startColumn);
            var endCell = ws.Cell(headerRow, endColumn);
         
            var tablesHeaderArea = ws.Range(startCell,endCell).Merge();
            tablesHeaderArea.Value = "Шильдик";


            //рассчитываем шапку для табличек
            startColumn = endColumn + 1;
            endColumn = quantityInfo.maxTablesQuantity > 0 ? startColumn + quantityInfo.maxNameplatesQuantity - 1 : startColumn;

            startCell = ws.Cell(headerRow, startColumn);
            endCell = ws.Cell(headerRow, endColumn);

            var nameplatesHeaderArea = ws.Range(startCell, endCell).Merge();


            nameplatesHeaderArea.Value = "Табличка";

        }

        private (int maxTablesQuantity, int maxNameplatesQuantity) FillWorksheetTable(IXLWorksheet ws, ProjectInfo project)
        {

            var stands = project.Stands;

            int maxTables = 0;
            int maxNameplates = 0;


            int activeRow = 2;
            int standNumber = 1;

            foreach (var stand in stands)
            {

                ws.Cell("A" + activeRow).Value = standNumber;



                var standTables = stand.StandAdditionalEquips
                    .SelectMany(equip => equip.AdditionalEquip.Purposes)
                    .Where(purpose => purpose?.Material?.Contains("Табличка") ?? false);

                var standTablesStrings = standTables.Select(_ =>
                {
                    string standTableText = "Стенд датчиков КИПиА\n";
                    standTableText += $"{stand.KKSCode}\n";
                    standTableText += $"{stand.SerialNumber}\n";
                    standTableText += $"Дата: {DateTime.Now.ToString("dd.MM.yyyy")}";

                    return standTableText;
                });

                var standNameplates = stand.StandAdditionalEquips
                                           .SelectMany(equip => equip.AdditionalEquip.Purposes)
                                           .Where(purpose => purpose?.Material?.Contains("Шильдик") ?? false);

                var standNameplatesStrings = standNameplates.Select(_ =>
                {
                    string standNameplateText = "Содержимое таблички\n";
                    standNameplateText += $"{stand.KKSCode}\n";

                    return standNameplateText;
                });



                int activeColumn = 2;

                //растягиваем все найденные таблички вдоль строки
                foreach (var tableString in standTablesStrings)
                {

                    ws.Cell(activeRow, activeColumn).Value = tableString;

                    activeColumn++;

                }

                //растягиваем все найденные шильдики вдоль строки после табличек
                foreach (var nameplateString in standNameplatesStrings)
                {

                    ws.Cell(activeRow, activeColumn).Value = nameplateString;

                    activeColumn++;

                }


                maxTables = Math.Max(maxTables, standTablesStrings.Count());
                maxNameplates = Math.Max(maxNameplates, standNameplatesStrings.Count());

                standNumber++;
                activeRow++;
            }


            Debug.WriteLine($"maxTables: {maxTables}");
            Debug.WriteLine($"maxNameplates: {maxNameplates}");



            //отдаем максимум найденных табличек/шильдиков для построения шапки
            return (maxTablesQuantity: maxTables,
                    maxNameplatesQuantity: maxNameplates);
        }

    }
}
