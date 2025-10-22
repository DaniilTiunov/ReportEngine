using IniParser;
using IniParser.Model;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs.CalculationSettings.Interfaces;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;
using System.Globalization;

namespace ReportEngine.Shared.Config.IniHelpers.CalculationSettings;

public class ElectricalSettings : IIniSettings<ElectricalSettingsData>
{
    private static readonly FileIniDataParser _parser = new();
    private static readonly string _iniFile = DirectoryHelper.GetIniConfigPath();
    private static readonly CultureInfo _csvCulture = CultureInfo.GetCultureInfo("ru-RU");

    public static ElectricalSettingsData ReadFromIni(IniData electricalData)
    {
        if (electricalData == null)
            return new ElectricalSettingsData();

        double Parse(string? s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return 0.0;
            return double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, _csvCulture, out var v)
                ? v
                : 0.0;
        }

        return new ElectricalSettingsData
        {
            ElectricalMontage = Parse(electricalData["ElectricalSettings"]["electricalMontage"]),
            TimeMontageWire = Parse(electricalData["ElectricalSettings"]["timeMontageWire"]),
            TimeMontageCable = Parse(electricalData["ElectricalSettings"]["timeMontageCable"])
        };
    }

    public static void WriteToIni(IniData sandData, ElectricalSettingsData settingsData)
    {
        if (sandData == null)
            return;

        string F(double v)
        {
            return v.ToString(_csvCulture);
        }

        sandData["ElectricalSettings"]["electricalMontage"] = F(settingsData.ElectricalMontage);
        sandData["ElectricalSettings"]["timeMontageWire"] = F(settingsData.TimeMontageWire);
        sandData["ElectricalSettings"]["timeMontageCable"] = F(settingsData.TimeMontageCable);

        _parser.WriteFile(_iniFile, sandData);
    }
}