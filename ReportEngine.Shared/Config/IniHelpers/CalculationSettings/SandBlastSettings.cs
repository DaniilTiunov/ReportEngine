using System.Globalization;
using IniParser;
using IniParser.Model;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs.CalculationSettings.Interfaces;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;

namespace ReportEngine.Shared.Config.IniHelpers.CalculationSettings;

public class SandBlastSettings : IIniSettings<SandBlasteSettingsData>
{
    private static readonly FileIniDataParser _parser = new();
    private static readonly string _iniFile = DirectoryHelper.GetIniConfigPath();
    private static readonly CultureInfo _csvCulture = CultureInfo.GetCultureInfo("ru-RU");

    public static SandBlasteSettingsData ReadFromIni(IniData sandData)
    {
        if (sandData == null)
            return new SandBlasteSettingsData();

        double Parse(string? s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return 0.0;
            return double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, _csvCulture, out var v)
                ? v
                : 0.0;
        }

        return new SandBlasteSettingsData
        {
            SandBlastWork = Parse(sandData["SandBlastingSettings"]["sandBlastWork"]),
            TimeSandBlastWork = Parse(sandData["SandBlastingSettings"]["timeSandBlastWork"])
        };
    }

    public static void WriteToIni(IniData sandData, SandBlasteSettingsData settingsData)
    {
        if (sandData == null)
            return;

        string F(double v)
        {
            return v.ToString(_csvCulture);
        }

        sandData["SandBlastingSettings"]["sandBlastWork"] = F(settingsData.SandBlastWork);
        sandData["SandBlastingSettings"]["timeSandBlastWork"] = F(settingsData.TimeSandBlastWork);

        _parser.WriteFile(_iniFile, sandData);
    }
}