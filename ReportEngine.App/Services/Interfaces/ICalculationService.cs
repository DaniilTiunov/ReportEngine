using ReportEngine.App.Model;

namespace ReportEngine.App.Services.Interfaces;

public interface ICalculationService
{
    Task CalculateProjectAsync(ProjectModel project);
}