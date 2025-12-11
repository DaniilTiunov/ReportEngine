using System.Globalization;
using IniParser;
using IniParser.Model;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs.CalculationSettings.Interfaces;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;

namespace ReportEngine.Shared.Config.IniHelpers.CalculationSettings;

public class StandSettings : IIniSettings<StandSettingsData>
{
    private static readonly FileIniDataParser _parser = new();
    private static readonly string _iniFile = DirectoryHelper.GetIniConfigPath();
    private static readonly CultureInfo _csvCulture = CultureInfo.GetCultureInfo("ru-RU");

    public static StandSettingsData ReadFromIni(IniData standData)
    {
        if (standData == null)
            return new StandSettingsData();

        double Parse(string? s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return 0.0;
            return double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, _csvCulture, out var v)
                ? v
                : 0.0;
        }

        return new StandSettingsData
        {
            SteelChannel = standData["StandsSettings"]["steelChannel"],
            NamePlate = standData["StandsSettings"]["namePlate"],
            NameTable = standData["StandsSettings"]["nameTable"],
            FrameGalvanizing = standData["StandsSettings"]["frameGalvanizing"],
            Bracket = standData["StandsSettings"]["bracket"],
            CabelSixMM = standData["StandsSettings"]["cabelSixMM"],
            CabelFourMM = standData["StandsSettings"]["cabelFourMM"],
            SignalCable = standData["StandsSettings"]["signalCable"],
            SeniorEngineer = standData["StandsSettings"]["seniorEngineer"],
            ResponsibleForAccept = standData["StandsSettings"]["responsibleForAccept"],
            SecondLevelSpecialist = standData["StandsSettings"]["secondLevelSpecialist"],
            OSiL = standData["StandsSettings"]["OSiL"],
            SensorCountOnFrame = Parse(standData["StandsSettings"]["sensorCountOnFrame"])
        };
    }

    public static void WriteToIni(IniData standData, StandSettingsData settingsData)
    {
        if (standData == null)
            return;

        string F(double v)
        {
            return v.ToString(_csvCulture);
        }

        standData["StandsSettings"]["steelChannel"] = settingsData.SteelChannel;
        standData["StandsSettings"]["namePlate"] = settingsData.NamePlate;
        standData["StandsSettings"]["nameTable"] = settingsData.NameTable;
        standData["StandsSettings"]["frameGalvanizing"] = settingsData.FrameGalvanizing;
        standData["StandsSettings"]["bracket"] = settingsData.Bracket;
        standData["StandsSettings"]["cabelSixMM"] = settingsData.CabelSixMM;
        standData["StandsSettings"]["cabelFourMM"] = settingsData.CabelFourMM;
        standData["StandsSettings"]["signalCable"] = settingsData.SignalCable;
        standData["StandsSettings"]["seniorEngineer"] = settingsData.SeniorEngineer;
        standData["StandsSettings"]["responsibleForAccept"] = settingsData.ResponsibleForAccept;
        standData["StandsSettings"]["secondLevelSpecialist"] = settingsData.SecondLevelSpecialist;
        standData["StandsSettings"]["OSiL"] = settingsData.OSiL;
        standData["StandsSettings"]["sensorCountOnFrame"] = F(settingsData.SensorCountOnFrame ?? 0.0);
    }
}