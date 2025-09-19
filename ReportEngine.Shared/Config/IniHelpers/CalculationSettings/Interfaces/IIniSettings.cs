using IniParser.Model;

namespace ReportEngine.Shared.Config.IniHeleprs.CalculationSettings.Interfaces;

public interface IIniSettings<TData>
    where TData : class, IIniData, new()
{
    static abstract TData ReadFromIni(IniData data);
    static abstract void WriteToIni(IniData data, TData settingsData);
}