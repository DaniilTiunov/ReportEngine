namespace ReportEngine.Export.ExcelWork.Services.Generators.DTO;

public record ComponentsReportStandsData(
    List<(string name, string unit, string quantity)> PipesList,
    List<(string name, string unit, string quantity)> ArmaturesList,
    List<(string name, string unit, string quantity)> TreeList,
    List<(string name, string unit, string quantity)> KmchList,
    List<(string name, string unit, string quantity)> DrainageParts,
    List<(string name, string unit, string quantity)> FramesList,
    List<(string name, string unit, string quantity)> SensorsHolders,
    List<(string name, string unit, string quantity)> ElectricalParts,
    List<(string name, string unit, string quantity)> OthersParts,
    List<(string name, string unit, string quantity)> Supplies
);

public record SummaryReportStandsData(
    List<ReportStandData> PipesList,
    List<ReportStandData> ArmaturesList,
    List<ReportStandData> TreeList,
    List<ReportStandData> KmchList,
    List<ReportStandData> DrainageParts,
    List<ReportStandData> FramesList,
    List<ReportStandData> SensorsHolders,
    List<ReportStandData> ElectricalParts,
    List<ReportStandData> OthersParts,
    List<ReportStandData> Supplies
);

public record SummaryReportLaborData(
    ReportStandData frameProduction,
    ReportStandData obvProduction,
    ReportStandData collectorProduction,
    ReportStandData qualityTests,
    ReportStandData sandblasting,
    ReportStandData paintingWorks,
    ReportStandData electricalWorks,
    ReportStandData commonStandCheck
 );

public struct ReportStandData
{
    public int? ExportDays {  get; set; } 
    public string? Name { get; set; }
    public string? Unit { get; set; }
    public float? Quantity { get; set; }
    public float? CostPerUnit { get; set; }
    public float? CommonCost { get; set; }


    public bool QuantityValid { get; set; }
    public bool CostPerUnitValid { get; set; }
    public bool CommonCostValid { get; set;  }
    
}