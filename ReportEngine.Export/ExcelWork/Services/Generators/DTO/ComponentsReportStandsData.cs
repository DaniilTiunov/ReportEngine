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
    public ValidatedField<int?> ExportDays {  get; set; } 
    public ValidatedField<string?> Name { get; set; }
    public ValidatedField<string?> Unit { get; set; }
    public ValidatedField<float?> Quantity { get; set; }
    public ValidatedField<float?> CostPerUnit { get; set; }
    public ValidatedField<float?> CommonCost { get; set; }

}

public struct ValidatedField<T>
{
    public T Value { get; set; }
    public bool IsValid { get; set; }

    public ValidatedField(T value, bool isValid)
    {
        Value = value;
        IsValid = isValid;
    }

}