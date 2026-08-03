using ClosedXML.Excel;
using ReportEngine._1CIntegration.DTO;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork;
using ReportEngine.Shared.Config.IniHeleprs;

namespace ReportEngine._1CIntegration.Services;

public class SummaryReportService
{
    private readonly IProjectInfoRepository _projectRepository;

    public SummaryReportService(IProjectInfoRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task GenerateAsync(int projectId)
    {
        var project = await GetFullProjectAsync(projectId);


    }

    private string GetPath()
    {
        return Path.Combine(SettingsManager.GetReportDirectory(),
            ExcelReportHelper.CreateReportName("Сводная ведомость для 1С", "xlsx"));
    }

    private void Export(List<ExportItem> items)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("1С");

        var row = 1;

        foreach (var item in items)
        {
            ws.Cell(row, 1).Value = item.Name;
            ws.Cell(row, 2).Value = item.Unit;
            ws.Cell(row, 3).Value = item.Quantity;
            row++;
        }

        wb.SaveAs(GetPath());
    }



    private async Task<ProjectInfo?> GetFullProjectAsync(int projectId)
    {
        return await _projectRepository.GetFullProjectAsync(projectId);
    }
}
