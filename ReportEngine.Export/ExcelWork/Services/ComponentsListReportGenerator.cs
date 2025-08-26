using System.Diagnostics;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.Directory;

namespace ReportEngine.Export.ExcelWork.Services;

public class ComponentsListReportGenerator : IReportGenerator
{
    private readonly IProjectInfoRepository _projectInfoRepository;
    
    public ReportType Type => ReportType.ComponentsListReport;

    public ComponentsListReportGenerator(IProjectInfoRepository projectInfoRepository)
    {
        _projectInfoRepository = projectInfoRepository;
    }
    
    public async Task GenerateAsync(int projectId)
    {
        try
        {
            var project = await _projectInfoRepository.GetByIdAsync(projectId);
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

    private void DrawReportHeader(XLWorkbook wb, ICollection<Stand> stands)
    {
        
    }
}