using System.Globalization;
using IniParser;
using IniParser.Model;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs.CalculationSettings.Interfaces;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;

namespace ReportEngine.Shared.Config.IniHelpers.CalculationSettings;

public class FrameSettings : IIniSettings<FrameSettingsData>
{
    private static readonly FileIniDataParser _parser = new();
    private static readonly string _iniFile = DirectoryHelper.GetIniConfigPath();
    private static readonly CultureInfo _csvCulture = CultureInfo.GetCultureInfo("ru-RU");

    public static FrameSettingsData ReadFromIni(IniData frameData)
    {
        if (frameData == null)
            return new FrameSettingsData();

        double Parse(string? s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return 0.0;
            return double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, _csvCulture, out var v)
                ? v
                : 0.0;
        }

        return new FrameSettingsData
        {
            FrameProduction = Parse(frameData["FrameSettings"]["frameProduction"]),
            TimeForProductionFrame = Parse(frameData["FrameSettings"]["timeForProductionFrame"]),
            TimeForPrepareFrame = Parse(frameData["FrameSettings"]["timeForPrepareFrame"]),
            Painting = Parse(frameData["FrameSettings"]["painting"]),
            TimeForPaintFrame = Parse(frameData["FrameSettings"]["timeForPaintFrame"]),
            TimeForPaintObv = Parse(frameData["FrameSettings"]["timeForPaintObv"]),
            MaterialOne = frameData["FrameSettings"]["materialOne"],
            MaterialTwo = frameData["FrameSettings"]["materialTwo"],
            CountMaterialOne = Parse(frameData["FrameSettings"]["countMaterialOne"]),
            CountMaterialTwo = Parse(frameData["FrameSettings"]["countMaterialTwo"]),
            AssemblyCost = Parse(frameData["FrameSettings"]["assemblyCost"]),
            TimeForAssemblyWork = Parse(frameData["FrameSettings"]["timeForAssemblyWork"]),
            PillarCost = Parse(frameData["FrameSettings"]["pillarCost"]),
            TimePillarProduction = Parse(frameData["FrameSettings"]["timePillarProduction"]),
            ClosetWorkCost = Parse(frameData["FrameSettings"]["closetWorkCost"]),
            TimeClosetPrepare = Parse(frameData["FrameSettings"]["timeClosetPrepare"])
        };
    }

    public static void WriteToIni(IniData frameData, FrameSettingsData settingsData)
    {
        if (frameData == null)
            return;

        string F(double v)
        {
            return v.ToString(_csvCulture);
        }

        frameData["FrameSettings"]["frameProduction"] = F(settingsData.FrameProduction);
        frameData["FrameSettings"]["timeForProductionFrame"] = F(settingsData.TimeForProductionFrame);
        frameData["FrameSettings"]["timeForPrepareFrame"] = F(settingsData.TimeForPrepareFrame);
        frameData["FrameSettings"]["painting"] = F(settingsData.Painting);
        frameData["FrameSettings"]["timeForPaintFrame"] = F(settingsData.TimeForPaintFrame);
        frameData["FrameSettings"]["timeForPaintObv"] = F(settingsData.TimeForPaintObv);
        frameData["FrameSettings"]["materialOne"] = settingsData.MaterialOne;
        frameData["FrameSettings"]["materialTwo"] = settingsData.MaterialTwo;
        frameData["FrameSettings"]["countMaterialOne"] = F(settingsData.CountMaterialOne);
        frameData["FrameSettings"]["countMaterialTwo"] = F(settingsData.CountMaterialTwo);
        frameData["FrameSettings"]["assemblyCost"] = F(settingsData.AssemblyCost);
        frameData["FrameSettings"]["timeForAssemblyWork"] = F(settingsData.TimeForAssemblyWork);
        frameData["FrameSettings"]["pillarCost"] = F(settingsData.PillarCost);
        frameData["FrameSettings"]["timePillarProduction"] = F(settingsData.TimePillarProduction);
        frameData["FrameSettings"]["closetWorkCost"] = F(settingsData.ClosetWorkCost);
        frameData["FrameSettings"]["timeClosetPrepare"] = F(settingsData.TimeClosetPrepare);

        _parser.WriteFile(_iniFile, frameData);
    }
}