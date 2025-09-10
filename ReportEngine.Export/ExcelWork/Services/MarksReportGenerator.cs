using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs;
using System.Diagnostics;

namespace ReportEngine.Export.ExcelWork.Services
{
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

                //foreach (var stand in project.Stands)
                //{
                //var ws = wb.Worksheets.Add($"Стенд_{stand.KKSCode}");


                // }


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

            //headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            headerRange.Style.Font.SetBold();
        }    

        private void FillWorksheet(IXLWorksheet ws, ProjectInfo project)
        {

            int recordNumber = 1;

            foreach (var stand in project.Stands)
            {
                foreach (var obvyazka in stand.ObvyazkiInStand)
                {
                    //каждая запись - 2 строки
                    int upperRecordRow = recordNumber * 2;
                    int lowerRecordRow = upperRecordRow + 1;



                    string startRangeCell = "A" + upperRecordRow;
                    string endRangeCell = "A" + lowerRecordRow;
                    var unionCellsRange = ws.Range($"{startRangeCell}:{endRangeCell}").Merge();
                    unionCellsRange.Value = recordNumber;

                    Debug.WriteLine("Стартовая ячейка" + startRangeCell);
                    Debug.WriteLine("конечная ячейка" + endRangeCell);

                    startRangeCell = "B" + upperRecordRow;
                    endRangeCell = "B" + lowerRecordRow;
                    unionCellsRange = ws.Range($"{startRangeCell}:{endRangeCell}").Merge();
                    unionCellsRange.Value = stand.KKSCode;
                    



                    startRangeCell = "C" + upperRecordRow;
                    endRangeCell = "C" + lowerRecordRow;
                    unionCellsRange = ws.Range($"{startRangeCell}:{endRangeCell}").Merge();
                    unionCellsRange.Value = obvyazka.FirstSensorKKS;


                    startRangeCell = "D" + upperRecordRow;
                    endRangeCell = "D" + lowerRecordRow;
                    ws.Cell(startRangeCell).Value = obvyazka.FirstSensorMarkPlus;
                    ws.Cell(endRangeCell).Value = obvyazka.FirstSensorMarkMinus;

                    recordNumber++;
                }
            }

        }




    }
}

