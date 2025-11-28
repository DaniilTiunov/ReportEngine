namespace ReportEngine.Export.DTO;


public struct SensorRecordData
{
    public string SensorKKS;
    public string SensorDescription;

    public SensorRecordData(string sensorKKS, string sensorDescription)
    {
        SensorKKS = sensorKKS;
        SensorDescription = sensorDescription;
    }
}

public record PartsStandsData(
    List<EquipmentRecord> PipesList,
    List<EquipmentRecord> ArmaturesList,
    List<EquipmentRecord> TreeList,
    List<EquipmentRecord> KmchList,
    List<EquipmentRecord> DrainageParts,
    List<EquipmentRecord> FramesList,
    List<EquipmentRecord> SensorsHolders,
    List<EquipmentRecord> ElectricalParts,
    List<EquipmentRecord> OthersParts,
    List<EquipmentRecord> Supplies
);

public record LaborStandsData(
    EquipmentRecord frameProduction,
    EquipmentRecord obvProduction,
    EquipmentRecord collectorProduction,
    EquipmentRecord qualityTests,
    EquipmentRecord sandblasting,
    EquipmentRecord paintingWorks,
    EquipmentRecord electricalWorks,
    EquipmentRecord commonStandCheck
 );

public struct EquipmentRecord
{
    public ValidatedField<int?> ExportDays { get; set; }
    public ValidatedField<string?> Name { get; set; }
    public ValidatedField<string?> Unit { get; set; }
    public ValidatedField<float?> Quantity { get; set; }
    public ValidatedField<float?> CostPerUnit { get; set; }
    public ValidatedField<float?> CommonCost { get; set; }

}

public struct StandInfoData
{
    public ValidatedField<string?> Name { get; set; }
    public ValidatedField<string?> KKS { get; set; }
    public ValidatedField<string?> SerialNumber { get; set; }
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