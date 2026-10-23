namespace ReportEngine.Import.Configuration;

public class ComponentsCellConfig
{
    public string CategoryName { get; set; }
    public string ColumnPurpose { get; set; } = "B";
    public string ColumnMaterial { get; set; } = "C";
    public string ColumnQuantity { get; set; } = "D";
    public string ColumnCostPerUnit { get; set; } = "E";
    public string ColumnMeasure { get; set; } = "F";
    public string ColumnWeight { get; set; } = "G";
    public string ColumnExportDays { get; set; } = "H";
}