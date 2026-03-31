namespace ReportEngine.Domain.Store;

public static class StoreKeys
{
    public static readonly string[] StandsRequired =
    {
        "LeadEngineer",
        "AcceptanceSupervisor",
        "SpecialistL2"
    };

    public static readonly string[] ElectricRequired =
    {
        "ElectricalInstallCost",
        "WireInstallTime",
        "CableInstallTime",
    };

    public static readonly string[] HumanCostRequired =
    {
        "PipeworkFabCost",
        "ManifoldFabCost",
        "TestBenchTestCost",
        "TestBenchInspectCost",
        "TestBenchGalvCost",
        "StandInspectTime",
        "FinalWorkTime",
        "HoleDrillTime",
        "CollectorWeldTime",
        "AllTestsTime",
        "EquipmentPrepTime",
        "BusHoleDrillTime",
        "InputInstallTime",
        "OtherOpsTime"
    };

    public static readonly string[] FramesSettingsRequired =
    {
        "FrameFabCost",
        "TestBenchPaintCost",
        "FrameProdTime",
        "FramePaintTime",
        "FramePrepTime",
        "PipeworkPaintTime",
        "MaterialOneQuantity",
        "MaterialTwoQuantity",
        "DisassemblableFrameCost",
        "DisassemblableFrameFabTime",
        "StandFabCost",
        "StandFabTime",
        "CabinetWorkCost",
        "CabinetPrepTime"
    };

    public static readonly string[] EquipmentsSettingsRequired =
    {
        "MaterialOne",
        "MaterialTwo",
        "ChannelBar",
        "Nameplate",
        "LabelPlate",
        "Galvanizing",
        "Bracket",
        "UniversalBracket",
        "DiffPressureBracket",
        "AbsPressureBracket",
        "Cable6mm",
        "Cable6mmQuantity",
        "Cable4mm",
        "SignalCable",
        "Clamps",
        "Terminal"
    };

    public static readonly string[] SandBlastSettingsRequired =
    {
        "SandblastingCost",
        "SandblastingTime"
    };
}
