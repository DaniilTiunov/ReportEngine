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
    List<ReportRecordData> PipesList,
    List<ReportRecordData> ArmaturesList,
    List<ReportRecordData> TreeList,
    List<ReportRecordData> KmchList,
    List<ReportRecordData> DrainageParts,
    List<ReportRecordData> FramesList,
    List<ReportRecordData> SensorsHolders,
    List<ReportRecordData> ElectricalParts,
    List<ReportRecordData> OthersParts,
    List<ReportRecordData> Supplies
);

public record SummaryReportLaborData(
    ReportRecordData frameProduction,
    ReportRecordData obvProduction,
    ReportRecordData collectorProduction,
    ReportRecordData qualityTests,
    ReportRecordData sandblasting,
    ReportRecordData paintingWorks,
    ReportRecordData electricalWorks,
    ReportRecordData commonStandCheck
 );

public struct ReportRecordData
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
