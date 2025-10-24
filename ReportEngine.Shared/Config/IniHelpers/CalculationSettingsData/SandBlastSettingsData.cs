using ReportEngine.Shared.Config.IniHeleprs.CalculationSettings.Interfaces;

namespace ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;

public class SandBlastSettingsData : IIniData
{
    public double SandBlastWork { get; set; }
    public double TimeSandBlastWork { get; set; }
}