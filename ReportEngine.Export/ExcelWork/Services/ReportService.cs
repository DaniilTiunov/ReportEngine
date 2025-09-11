using System.Linq;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;

namespace ReportEngine.Export.ExcelWork.Services;

public class ReportService : IReportService
{
    private readonly IEnumerable<IReportGenerator> _generators;

    public ReportService(IEnumerable<IReportGenerator> generators)
    {
        _generators = generators;
    }

    public async Task GenerateReportAsync(ReportType generatorType, int projectId)
    {
        var generator = _generators.FirstOrDefault(generator => generator.Type == generatorType);
        if (generator == null)
            throw new InvalidOperationException($"Генератор {generatorType} не зарегистрирован");

        
        await Task.Run(async () => await generator.GenerateAsync(projectId));
    }
}