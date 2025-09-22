using ReportEngine.Shared.Config.IniHeleprs.CalculationSettings.Interfaces;

namespace ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;

public class ElectricalSettingsData : IIniData
{
    public double ElectricalMontage { get; set; }
    public double TimeMontageWire { get; set; }
    public double TimeMontageCable { get; set; }
}