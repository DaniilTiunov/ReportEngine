using IniParser;
using IniParser.Model;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs.CalculationSettings.Interfaces;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;
using System.Globalization;


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
        ;
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
            SteelChannelMeasure = standData["StandsSettings"]["steelChannelMeasure"],
            NamePlate = standData["StandsSettings"]["namePlate"],
            NamePlateMeasure = standData["StandsSettings"]["namePlateMeasure"],
            NameTable = standData["StandsSettings"]["nameTable"],
            NameTableMeasure = standData["StandsSettings"]["nameTableMeasure"],
            FrameGalvanizing = standData["StandsSettings"]["frameGalvanizing"],
            Bracket = standData["StandsSettings"]["bracket"],
            BracketMeasure = standData["StandsSettings"]["bracketMeasure"],
            BracketForDif = standData["StandsSettings"]["bracketForDif"],
            BracketForDifMeasure = standData["StandsSettings"]["bracketForDifMeasure"],
            BracketForAbs = standData["StandsSettings"]["bracketForAbs"],
            BracketForAbsMeasure = standData["StandsSettings"]["bracketForAbsMeasure"],
            BracketUniversal = standData["StandsSettings"]["bracketUniversal"],
            BracketUniversalMeasure = standData["StandsSettings"]["bracketUniversalMeasure"],
            CabelSixMm = standData["StandsSettings"]["cabelSixMm"],
            CabelSixMmMeasure = standData["StandsSettings"]["cabelSixMmMeasure"],
            CabelFourMm = standData["StandsSettings"]["cabelFourMm"],
            CabelFourMmMeasure = standData["StandsSettings"]["cabelFourMmMeasure"],
            SignalCable = standData["StandsSettings"]["signalCable"],
            SignalCableMeasure = standData["StandsSettings"]["signalCableMeasure"],
            SeniorEngineer = standData["StandsSettings"]["seniorEngineer"],
            ResponsibleForAccept = standData["StandsSettings"]["responsibleForAccept"],
            SecondLevelSpecialist = standData["StandsSettings"]["secondLevelSpecialist"],
            OSiL = standData["StandsSettings"]["OSiL"],
            SensorCountOnFrame = Parse(standData["StandsSettings"]["sensorCountOnFrame"]),
            Clamp = standData["StandsSettings"]["clamp"],
            ClampMeasure = standData["StandsSettings"]["clampMeasure"],
            Terminal = standData["StandsSettings"]["terminal"],
            TerminalMeasure = standData["StandsSettings"]["terminalMeasure"]
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
        standData["StandsSettings"]["steelChannelMeasure"] = settingsData.SteelChannelMeasure;
        standData["StandsSettings"]["namePlate"] = settingsData.NamePlate;
        standData["StandsSettings"]["namePlateMeasure"] = settingsData.NamePlateMeasure;
        standData["StandsSettings"]["nameTable"] = settingsData.NameTable;
        standData["StandsSettings"]["nameTableMeasure"] = settingsData.NameTableMeasure;
        standData["StandsSettings"]["frameGalvanizing"] = settingsData.FrameGalvanizing;
        standData["StandsSettings"]["bracket"] = settingsData.Bracket;
        standData["StandsSettings"]["bracketMeasure"] = settingsData.BracketMeasure;
        standData["StandsSettings"]["bracketForDif"] = settingsData.BracketForDif;
        standData["StandsSettings"]["bracketForDifMeasure"] = settingsData.BracketForDifMeasure;
        standData["StandsSettings"]["bracketForAbs"] = settingsData.BracketForAbs;
        standData["StandsSettings"]["bracketForAbsMeasure"] = settingsData.BracketForAbsMeasure;
        standData["StandsSettings"]["bracketUniversal"] = settingsData.BracketUniversal;
        standData["StandsSettings"]["bracketUniversalMeasure"] = settingsData.BracketUniversalMeasure;
        standData["StandsSettings"]["cabelSixMm"] = settingsData.CabelSixMm;
        standData["StandsSettings"]["cabelSixMmMeasure"] = settingsData.CabelSixMmMeasure;
        standData["StandsSettings"]["cabelFourMm"] = settingsData.CabelFourMm;
        standData["StandsSettings"]["cabelFourMmMeasure"] = settingsData.CabelFourMmMeasure;
        standData["StandsSettings"]["signalCable"] = settingsData.SignalCable;
        standData["StandsSettings"]["signalCableMeasure"] = settingsData.SignalCableMeasure;
        standData["StandsSettings"]["seniorEngineer"] = settingsData.SeniorEngineer;
        standData["StandsSettings"]["responsibleForAccept"] = settingsData.ResponsibleForAccept;
        standData["StandsSettings"]["secondLevelSpecialist"] = settingsData.SecondLevelSpecialist;
        standData["StandsSettings"]["OSiL"] = settingsData.OSiL;
        standData["StandsSettings"]["sensorCountOnFrame"] = F(settingsData.SensorCountOnFrame ?? 0.0);
        standData["StandsSettings"]["clamp"] = settingsData.Clamp;
        standData["StandsSettings"]["clampMeasure"] = settingsData.ClampMeasure;
        standData["StandsSettings"]["terminal"] = settingsData.Terminal;
        standData["StandsSettings"]["terminalMeasure"] = settingsData.TerminalMeasure;
    }
}