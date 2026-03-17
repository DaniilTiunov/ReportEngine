namespace ReportEngine.Export.DTO;

public class SensorRecordData
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

public class PartsStandsData
{
    public List<EquipmentRecord?>? PipesList { get; set; }
    public List<EquipmentRecord?>? ArmaturesList { get; set; }
    public List<EquipmentRecord?>? TreeList { get; set; }
    public List<EquipmentRecord?>? KmchList { get; set; }
    public List<EquipmentRecord?>? DrainageParts { get; set; }
    public List<EquipmentRecord?>? FramesList { get; set; }
    public List<EquipmentRecord?>? SensorsHolders { get; set; }
    public List<EquipmentRecord?>? ElectricalParts { get; set; }
    public List<EquipmentRecord?>? OthersParts { get; set; }
    public List<EquipmentRecord?>? Supplies { get; set; }
}


public class LaborStandsData
{
    public EquipmentRecord? FrameProduction { get; set; }
    public EquipmentRecord? ObvProduction { get; set; }
    public EquipmentRecord? CollectorProduction { get; set; }
    public EquipmentRecord? QualityTests { get; set; }
    public EquipmentRecord? Sandblasting { get; set; }
    public EquipmentRecord? PaintingWorks { get; set; }
    public EquipmentRecord? ElectricalWorks { get; set; }
    public EquipmentRecord? CommonStandCheck { get; set; }
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
