using System.Diagnostics;
using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs;

namespace ReportEngine.Export.ExcelWork.Services;

public class ContainerReportGenerator : IReportGenerator
{
    private readonly IProjectInfoRepository _projectInfoRepository;

    public ContainerReportGenerator(IProjectInfoRepository projectInfoRepository)
    {
        _projectInfoRepository = projectInfoRepository;
    }

    public ReportType Type => ReportType.ContainerReport;

    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);

        var templatePath = DirectoryHelper.GetReportsTemplatePath("Тара");
        var fileName = "Тара___" + DateTime.Now.ToString("yy-MM-dd___HH-mm-ss") + ".xlsx";

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

    private void FillWorksheet(IXLWorksheet ws, ProjectInfo project)
    {
        
        var tableRecords = project.Stands
           .Select(stand => new
           {
               Name = stand.NN.ToString(),
               SerialNumber = stand.SerialNumber,
               CodeKKS = stand.KKSCode,
               Quantity = "1",//потом поправить на конкретное число
               FrameWidth = stand.Width.ToString()
           });

        


        var recordNumber = 1;

        foreach (var record in tableRecords)
        {
            //для отступа от шапки
            var recordRow = recordNumber + 1;

            var place = "1." + recordNumber;

            ws.Cell($"B{recordRow}").Value = place;
            ws.Cell($"C{recordRow}").Value = record.Name;
            ws.Cell($"D{recordRow}").Value = record.SerialNumber;
            ws.Cell($"E{recordRow}").Value = record.CodeKKS;
            ws.Cell($"F{recordRow}").Value = record.Quantity;
            ws.Cell($"G{recordRow}").Value = record.FrameWidth;

            recordNumber++;
        }
    }


}