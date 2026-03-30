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
            SteelChannelEntityName = standData["StandsSettings"]["steelChannelEntityName"],
            NamePlate = standData["StandsSettings"]["namePlate"],
            NamePlateEntityName = standData["StandsSettings"]["namePlateEntityName"],
            NameTable = standData["StandsSettings"]["nameTable"],
            NameTableEntityName = standData["StandsSettings"]["nameTableEntityName"],
            FrameGalvanizing = standData["StandsSettings"]["frameGalvanizing"],
            Bracket = standData["StandsSettings"]["bracket"],
            BracketEntityName = standData["StandsSettings"]["bracketEntityName"],
            BracketForDif = standData["StandsSettings"]["bracketForDif"],
            BracketForDifEntityName = standData["StandsSettings"]["bracketForDifEntityName"],
            BracketForAbs = standData["StandsSettings"]["bracketForAbs"],
            BracketForAbsEntityName = standData["StandsSettings"]["bracketForAbsEntityName"],
            BracketUniversal = standData["StandsSettings"]["bracketUniversal"],
            BracketUniversalEntityName = standData["StandsSettings"]["bracketUniversalEntityName"],
            CabelSixMm = standData["StandsSettings"]["cabelSixMm"],
            CabelSixMmEntityName = standData["StandsSettings"]["cabelSixMmEntityName"],
            CabelFourMm = standData["StandsSettings"]["cabelFourMm"],
            CabelFourMmEntityName = standData["StandsSettings"]["cabelFourMmEntityName"],
            SignalCable = standData["StandsSettings"]["signalCable"],
            SignalCableEntityName = standData["StandsSettings"]["signalCableEntityName"],
            SeniorEngineer = standData["StandsSettings"]["seniorEngineer"],
            ResponsibleForAccept = standData["StandsSettings"]["responsibleForAccept"],
            SecondLevelSpecialist = standData["StandsSettings"]["secondLevelSpecialist"],
            OSiL = standData["StandsSettings"]["OSiL"],
            SensorCountOnFrame = Parse(standData["StandsSettings"]["sensorCountOnFrame"]),
            Clamp = standData["StandsSettings"]["clamp"],
            ClampEntityName = standData["StandsSettings"]["clampEntityName"],
            Terminal = standData["StandsSettings"]["terminal"],
            TerminalEntityName = standData["StandsSettings"]["terminalEntityName"]
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
        standData["StandsSettings"]["steelChannelEntityName"] = settingsData.SteelChannelEntityName;
        standData["StandsSettings"]["namePlate"] = settingsData.NamePlate;
        standData["StandsSettings"]["namePlateEntityName"] = settingsData.NamePlateEntityName;
        standData["StandsSettings"]["nameTable"] = settingsData.NameTable;
        standData["StandsSettings"]["nameTableEntityName"] = settingsData.NameTableEntityName;
        standData["StandsSettings"]["frameGalvanizing"] = settingsData.FrameGalvanizing;
        standData["StandsSettings"]["bracket"] = settingsData.Bracket;
        standData["StandsSettings"]["bracketEntityName"] = settingsData.BracketEntityName;
        standData["StandsSettings"]["bracketForDif"] = settingsData.BracketForDif;
        standData["StandsSettings"]["bracketForDifEntityName"] = settingsData.BracketForDifEntityName;
        standData["StandsSettings"]["bracketForAbs"] = settingsData.BracketForAbs;
        standData["StandsSettings"]["bracketForAbsEntityName"] = settingsData.BracketForAbsEntityName;
        standData["StandsSettings"]["bracketUniversal"] = settingsData.BracketUniversal;
        standData["StandsSettings"]["bracketUniversalEntityName"] = settingsData.BracketUniversalEntityName;
        standData["StandsSettings"]["cabelSixMm"] = settingsData.CabelSixMm;
        standData["StandsSettings"]["cabelSixMmEntityName"] = settingsData.CabelSixMmEntityName;
        standData["StandsSettings"]["cabelFourMm"] = settingsData.CabelFourMm;
        standData["StandsSettings"]["cabelFourMmEntityName"] = settingsData.CabelFourMmEntityName;
        standData["StandsSettings"]["signalCable"] = settingsData.SignalCable;
        standData["StandsSettings"]["signalCableEntityName"] = settingsData.SignalCableEntityName;
        standData["StandsSettings"]["seniorEngineer"] = settingsData.SeniorEngineer;
        standData["StandsSettings"]["responsibleForAccept"] = settingsData.ResponsibleForAccept;
        standData["StandsSettings"]["secondLevelSpecialist"] = settingsData.SecondLevelSpecialist;
        standData["StandsSettings"]["OSiL"] = settingsData.OSiL;
        standData["StandsSettings"]["sensorCountOnFrame"] = F(settingsData.SensorCountOnFrame ?? 0.0);
        standData["StandsSettings"]["clamp"] = settingsData.Clamp;
        standData["StandsSettings"]["clampEntityName"] = settingsData.ClampEntityName;
        standData["StandsSettings"]["terminal"] = settingsData.Terminal;
        standData["StandsSettings"]["terminalEntityName"] = settingsData.TerminalEntityName;
    }
}
