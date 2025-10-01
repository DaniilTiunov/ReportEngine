using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;

namespace ReportEngine.Export.ExcelWork.Services.Generators;

public class SummuryReportGenerator : ComponentListReportGenerator, IReportGenerator
{
    private readonly IProjectInfoRepository _projectInfoRepository;
    
    public ReportType Type => ReportType.SummaryReport;

    public SummuryReportGenerator(IProjectInfoRepository projectInfoRepository)
        : base(projectInfoRepository)
    {
        _projectInfoRepository = projectInfoRepository;
    }
    
    public new async Task GenerateAsync(int projectId)
    {
        await base.GenerateAsync(projectId);
    }

    protected override string GetReportFileName()
    {
        return "Сводная ведомость___" +
                      DateTime.Now.ToString("dd-MM-yy___HH-mm-ss") +
                      ".xlsx";
    }
}