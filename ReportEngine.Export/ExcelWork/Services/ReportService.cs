using ReportEngine.Domain.Entities;
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

    //TODO: пофиксить все эти перегрузки в других местах тоже

    public async Task GenerateReportAsync(ReportType generatorType, int projectId)
    {
        var generator = _generators.FirstOrDefault(generator => generator.Type == generatorType);
        if (generator == null)
            throw new InvalidOperationException($"Генератор {generatorType} не зарегистрирован");

        await generator.GenerateAsync(projectId);
    }
        
    public async Task GenerateReportAsync(ReportType generatorType, int projectId, List<Stand>? selectedStands = null)
    {
        var generator = _generators.FirstOrDefault(generator => generator.Type == generatorType);
        if (generator == null)
            throw new InvalidOperationException($"Генератор {generatorType} не зарегистрирован");

        await generator.GenerateAsync(projectId, selectedStands);
    }
}
