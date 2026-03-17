namespace ReportEngine.Export.DTO;

public struct SensorRecordData
{
    public string SensorKKS;
    public string SensorDescription;
    public string SensorMarkPlus;
    public string SensorMarkMinus;

    public SensorRecordData(string sensorKKS, string sensorDescription, string sensorMarkPlus, string sensorMarkMinus)
    {
        SensorKKS = sensorKKS;
        SensorDescription = sensorDescription;
        SensorMarkPlus = sensorMarkPlus;
        SensorMarkMinus = sensorMarkMinus;
    }
}

public struct PartsStandsData
{
    public List<EquipmentRecord?> PipesList { get; set; }
    public List<EquipmentRecord?> ArmaturesList { get; set; }
    public List<EquipmentRecord?> TreeList { get; set; }
    public List<EquipmentRecord?> KmchList { get; set; }
    public List<EquipmentRecord?> DrainageParts { get; set; }
    public List<EquipmentRecord?> FramesList { get; set; }
    public List<EquipmentRecord?> SensorsHolders { get; set; }
    public List<EquipmentRecord?> ElectricalParts { get; set; }
    public List<EquipmentRecord?> OthersParts { get; set; }
    public List<EquipmentRecord?> Supplies { get; set; }
}


public class LaborStandsData
{
    public EquipmentRecord? frameProduction { get; set; }
    public EquipmentRecord? obvProduction { get; set; }
    public EquipmentRecord? collectorProduction { get; set; }
    public EquipmentRecord? qualityTests { get; set; }
    public EquipmentRecord? sandblasting { get; set; }
    public EquipmentRecord? paintingWorks { get; set; }
    public EquipmentRecord? electricalWorks { get; set; }
    public EquipmentRecord? commonStandCheck { get; set; }
}

public class EquipmentRecord
{
    public ValidatedField<int?>? ExportDays { get; set; }
    public ValidatedField<string?>? Name { get; set; }
    public ValidatedField<string?>? Unit { get; set; }
    public ValidatedField<float?>? Quantity { get; set; }
    public ValidatedField<float?>? CostPerUnit { get; set; }
    public ValidatedField<float?>? CommonCost { get; set; }
}

public class StandInfoData
{
    public ValidatedField<string?>? Name { get; set; }
    public ValidatedField<string?>? KKS { get; set; }
    public ValidatedField<string?>? SerialNumber { get; set; }
}

public class ValidatedField<T>
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
