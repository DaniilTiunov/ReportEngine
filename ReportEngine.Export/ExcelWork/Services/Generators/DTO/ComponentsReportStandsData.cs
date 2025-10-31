using SixLabors.ImageSharp;
using System.Data.SqlTypes;
using System.Linq;
using System.Numerics;

namespace ReportEngine.Export.ExcelWork.Services.Generators.DTO;


public record PartsStandsData(
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

public record LaborStandsData(
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
    public ValidatedField<int?> ExportDays { get; set; }
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


    //public static ValidatedField<T> Sum(IEnumerable<ValidatedField<T>> fields)
 
}