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
            var fileName = "Маркировка_" + DateTime.Now.ToString("yy-MM-dd") + ".xlsx";

            var savePath = SettingsManager.GetReportDirectory();
            var fullSavePath = Path.Combine(savePath, fileName);


            using (var wb = new XLWorkbook(templatePath))
            {
                foreach (var stand in project.Stands)
                {
                    var ws = wb.Worksheets.Add($"Стенд_{stand.KKSCode}");
                    CreateTableHeader(ws);
                    CreateStandWorksheet(ws, project);
                }





                Debug.WriteLine("Отчёт сохранён: " + fullSavePath);
                wb.SaveAs(fullSavePath);
            }
        }

        private void CreateTableHeader(IXLWorksheet ws)
        {
            ws.Cell("A1").Value = "№";
            ws.Cell("B1").Value = "KKS стенда";
            ws.Cell("C1").Value = "KKS изм. контура (датчика)";
            ws.Cell("D1").Value = "Маркировка";
        }




        private void CreateStandWorksheet(IXLWorksheet ws, ProjectInfo project)
        {
            int recordNumber = 1;



            foreach (var stand in project.Stands)
            {

                foreach (var obvyazka in stand.ObvyazkiInStand)
                {
                    //каждая запись - 2 строки
                    int recordRow = recordNumber + 1;



                    string cellName = "A" + recordRow;
                    ws.Cell(cellName).Value = recordNumber;


                    cellName = "B" + recordRow;
                    ws.Cell(cellName).Value = stand.KKSCode;

                    cellName = "C" + recordRow;
                    ws.Cell(cellName).Value = obvyazka.FirstSensorKKS;

                    cellName = "D" + recordRow;
                    ws.Cell(cellName).Value = obvyazka.FirstSensorMarkPlus;

                    cellName = "D" + recordRow + 1;
                    ws.Cell(cellName).Value = obvyazka.FirstSensorMarkMinus;

                    recordNumber++;
                }
            }

        }






    }
}

