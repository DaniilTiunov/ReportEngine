using System.ComponentModel;

namespace ReportEngine.Import.Dto;

public class ComponentDto
{
    public string? Purpose { get; set; }
    public string? Material { get; set; }
    public string? Quantity { get; set; }
    public string? CostPerUnit { get; set; }
    public string? Measure { get; set; }
    public string? Weight { get; set; }
    public string? ExportDays { get; set; }
    public bool? IsAutoCalculationEnabled { get; set; } = false;




    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(Purpose)
               && string.IsNullOrWhiteSpace(Material)
               && string.IsNullOrWhiteSpace(Quantity)
               && string.IsNullOrWhiteSpace(CostPerUnit)
               && string.IsNullOrWhiteSpace(Measure)
               && string.IsNullOrWhiteSpace(Weight)
               && string.IsNullOrWhiteSpace(ExportDays);
    }
}