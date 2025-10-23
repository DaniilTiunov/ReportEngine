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

public struct ReportStandData
{
    public string Name { get; set; }
    public string Unit { get; set; }
    public string Quantity { get; set; }
    public string CostPerUnit { get; set; }
    public string CommonCost { get; set; }
}