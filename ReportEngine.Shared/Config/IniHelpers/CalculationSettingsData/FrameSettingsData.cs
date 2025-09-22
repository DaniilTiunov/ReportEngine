using ReportEngine.Shared.Config.IniHeleprs.CalculationSettings.Interfaces;

namespace ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;

public class FrameSettingsData : IIniData
{
    public double FrameProduction { get; set; }
    public double TimeForProductionFrame { get; set; }
    public double TimeForPrepareFrame { get; set; }
    public double Painting { get; set; }
    public double TimeForPaintFrame { get; set; }
    public double TimeForPaintObv { get; set; }
    public string MaterialOne { get; set; }
    public string MaterialTwo { get; set; }
    public double CountMaterialOne { get; set; }
    public double CountMaterialTwo { get; set; }
    public double AssemblyCost { get; set; }
    public double TimeForAssemblyWork { get; set; }
    public double PillarCost { get; set; }
    public double TimePillarProduction { get; set; }
    public double ClosetWorkCost { get; set; }
    public double TimeClosetPrepare { get; set; }
}