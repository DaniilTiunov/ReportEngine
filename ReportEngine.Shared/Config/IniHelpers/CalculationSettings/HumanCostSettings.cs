using IniParser;
using IniParser.Model;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs.CalculationSettings.Interfaces;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;
using System.Globalization;

namespace ReportEngine.Shared.Config.IniHelpers.CalculationSettings;

public class HumanCostSettings : IIniSettings<HumanCostSettingsData>
{
    private static readonly FileIniDataParser _parser = new();
    private static readonly string _iniFile = DirectoryHelper.GetIniConfigPath();
    private static readonly CultureInfo _csvCulture = CultureInfo.GetCultureInfo("ru-RU");

    public static HumanCostSettingsData ReadFromIni(IniData costData)
    {
        if (costData == null)
            return new HumanCostSettingsData();

        double Parse(string? s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return 0.0;
            return double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, _csvCulture, out var v)
                ? v
                : 0.0;
        }

        return new HumanCostSettingsData
        {
            ObvzyakaProduction = Parse(costData["HumanCostSettings"]["obvzyakaProduction"]),
            CollectorProduction = Parse(costData["HumanCostSettings"]["collectorProduction"]),
            Tests = Parse(costData["HumanCostSettings"]["tests"]),
            CommonCheckStand = Parse(costData["HumanCostSettings"]["commonCheckStand"]),
            TimeForCheckStand = Parse(costData["HumanCostSettings"]["timeForCheckStand"]),
            TimeForFinalWork = Parse(costData["HumanCostSettings"]["timeForFinalWork"]),
            TimeForOneDrill = Parse(costData["HumanCostSettings"]["timeForOneDrill"]),
            TimeForCollectorBoil = Parse(costData["HumanCostSettings"]["timeForCollectorBoil"]),
            TimeForAllChecks = Parse(costData["HumanCostSettings"]["timeForAllChecks"]),
            TimeForPrepareAllEquipment = Parse(costData["HumanCostSettings"]["timeForPrepareAllEquipment"]),
            TimeForDrillOneBus = Parse(costData["HumanCostSettings"]["timeForDrillOneBus"]),
            TimeForMontageOneInput = Parse(costData["HumanCostSettings"]["timeForMontageOneInput"]),
            TimeForOthersOperations = Parse(costData["HumanCostSettings"]["timeForOthersOperations"])
        };
    }

    // Запись в файл
    public static void WriteToIni(IniData costData, HumanCostSettingsData settingsData)
    {
        if (costData == null)
            return;

        string F(double v)
        {
            return v.ToString(_csvCulture);
        }

        costData["HumanCostSettings"]["obvzyakaProduction"] = F(settingsData.ObvzyakaProduction);
        costData["HumanCostSettings"]["collectorProduction"] = F(settingsData.CollectorProduction);
        costData["HumanCostSettings"]["tests"] = F(settingsData.Tests);
        costData["HumanCostSettings"]["commonCheckStand"] = F(settingsData.CommonCheckStand);
        costData["HumanCostSettings"]["timeForCheckStand"] = F(settingsData.TimeForCheckStand);
        costData["HumanCostSettings"]["timeForFinalWork"] = F(settingsData.TimeForFinalWork);
        costData["HumanCostSettings"]["timeForOneDrill"] = F(settingsData.TimeForOneDrill);
        costData["HumanCostSettings"]["timeForCollectorBoil"] = F(settingsData.TimeForCollectorBoil);
        costData["HumanCostSettings"]["timeForAllChecks"] = F(settingsData.TimeForAllChecks);
        costData["HumanCostSettings"]["timeForPrepareAllEquipment"] = F(settingsData.TimeForPrepareAllEquipment);
        costData["HumanCostSettings"]["timeForDrillOneBus"] = F(settingsData.TimeForDrillOneBus);
        costData["HumanCostSettings"]["timeForMontageOneInput"] = F(settingsData.TimeForMontageOneInput);
        costData["HumanCostSettings"]["timeForOthersOperations"] = F(settingsData.TimeForOthersOperations);

        _parser.WriteFile(_iniFile, costData);
    }
}