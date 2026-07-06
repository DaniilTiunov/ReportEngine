using IniParser;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs.CalculationSettings.Interfaces;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettings;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;

namespace ReportEngine.Shared.Config.IniHelpers;

public static class CalculationSettingsManager
{
    private static readonly FileIniDataParser _parser = new();
    private static readonly string _iniFile = DirectoryHelper.GetIniConfigPath();


    public static AllSettingsData LoadAllSettings()
    {
        var ini = _parser.ReadFile(_iniFile);

        return new AllSettingsData
        {
            ElectricalSettings = ElectricalSettings.ReadFromIni(ini),
            FrameSettings = FrameSettings.ReadFromIni(ini),
            HumanCostSettings = HumanCostSettings.ReadFromIni(ini),
            SandBlastSettings = SandBlastSettings.ReadFromIni(ini),
            StandSettings = StandSettings.ReadFromIni(ini)
        };
    }

    public static Task<AllSettingsData> LoadAllSettingsAsync()
    {
        return Task.Run(() => LoadAllSettings());
    }


    // Синхронная загрузка
    public static TData Load<TSettings, TData>()
        where TSettings : IIniSettings<TData>
        where TData : class, IIniData, new()
    {
        var ini = _parser.ReadFile(_iniFile);
        return TSettings.ReadFromIni(ini);
    }

    // Синхронное сохранение
    public static void Save<TSettings, TData>(TData settings)
        where TSettings : IIniSettings<TData>
        where TData : class, IIniData, new()
    {
        var ini = _parser.ReadFile(_iniFile);
        TSettings.WriteToIni(ini, settings);
        _parser.WriteFile(_iniFile, ini);
    }

    // Асинхронная загрузка
    public static Task<TData> LoadAsync<TSettings, TData>()
        where TSettings : IIniSettings<TData>
        where TData : class, IIniData, new()
    {
        return Task.Run(() => Load<TSettings, TData>());
    }

    // Асинхронное сохранение
    public static Task SaveAsync<TSettings, TData>(TData settings)
        where TSettings : IIniSettings<TData>
        where TData : class, IIniData, new()
    {
        return Task.Run(() => Save<TSettings, TData>(settings));
    }
}
