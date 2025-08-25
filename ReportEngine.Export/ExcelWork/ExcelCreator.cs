using System.Diagnostics;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ClosedXML.Excel;
using ReportEngine.Export.ExcelWork.ExcelSettings;
using ReportEngine.Shared.Config.Directory;

namespace ReportEngine.Export.ExcelWork;

public class ExcelCreator
{
    private readonly IProjectInfoRepository _repository;
    
    public ExcelCreator(IProjectInfoRepository Repository)
    {
        _repository = Repository;
    }

    public async void CreateListOfComponents(int projectId)
    {
        try
        {
            var project = await _repository.GetByIdAsync(projectId);
            string title = project.Company;
            string templatePath = DirectoryHelper.GetReportsTemplatePath("Ведомость комплектующих");
            string fileName = "Ведомость комплектующих.xlsx";
            string savePath = DirectoryHelper.GetReportSavePath(fileName);

            using (var wb = new XLWorkbook(templatePath))
            {
                CreateStandWorkSheetAsync(wb, project.Stands);
                
                wb.SaveAs(savePath);
            }
        }
        catch (Exception e)
        {
            Debug.Write(e.Message);
        }
    }

    private void CreateStandWorkSheetAsync(XLWorkbook wb, ICollection<Stand> stands)
    {
        foreach (var stand in stands)
        {
            string sheetName = $"Стенд_{stand.KKSCode}";
            var ws = wb.Worksheets.Add(sheetName);
        }
    }
    private void CreateProjectInfoWorkSheetAsync(XLWorkbook wb, ProjectInfo project)
    {
        var ws = wb.Worksheet("Сводная заявка");
        ws.Cell("B1").Value = project.Company;
    }
}