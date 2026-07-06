using System.ComponentModel.DataAnnotations;
using ReportEngine.Domain.Entities.CalculationParameters.Enums;

namespace ReportEngine.Domain.Entities.CalculationParameters;

public class CalculationParameterGroup
{
    [Key] public int Id { get; set; }
    public CalculationParameterType SettingsType { get; set; }

    public string? Name { get; set; }

    public List<CalculationParameter> Parameters { get; set; } = new();
}
