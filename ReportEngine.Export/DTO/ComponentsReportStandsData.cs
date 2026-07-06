namespace ReportEngine.Export.DTO;

public class SensorRecordData
{
    public string? SensorDescription;
    public string? SensorKKS;
    public string? SensorMarkMinus;
    public string? SensorMarkPlus;

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
    public List<EquipmentRecord?> PipesList { get; set; } = new();
    public List<EquipmentRecord?> ArmaturesList { get; set; } = new();
    public List<EquipmentRecord?> TreeList { get; set; } = new();
    public List<EquipmentRecord?> KmchList { get; set; } = new();
    public List<EquipmentRecord?> DrainageParts { get; set; } = new();
    public List<EquipmentRecord?> FramesList { get; set; } = new();
    public List<EquipmentRecord?> SensorsHolders { get; set; } = new();
    public List<EquipmentRecord?> ElectricalParts { get; set; } = new();
    public List<EquipmentRecord?> OthersParts { get; set; } = new();
    public List<EquipmentRecord?> Supplies { get; set; } = new();
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
    public ValidatedField<int?> ExportDays { get; set; }
    public ValidatedField<string?> Name { get; set; }
    public ValidatedField<string?> Unit { get; set; }
    public ValidatedField<float?> Quantity { get; set; }
    public ValidatedField<float?> CostPerUnit { get; set; }
    public ValidatedField<float?> CommonCost { get; set; }
}

public class StandInfoData
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
