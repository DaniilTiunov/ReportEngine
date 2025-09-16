using IniParser;
using IniParser.Model;
using ReportEngine.Shared.CalculationSettings;
using ReportEngine.Shared.Config.Directory;
using System.Globalization;

namespace ReportEngine.Shared.Config.IniHeleprs
{
    public static class CalculationSettingsManager
    {
        private static readonly FileIniDataParser _parser = new ();
        private static readonly string _iniFile = DirectoryHelper.GetIniConfigPath();
        private static readonly CultureInfo _csvCulture = CultureInfo.GetCultureInfo("ru-RU");

        // Синхронные методы
        public static HumanCostSettings LoadHumanCostSettings()
        {
            var costData = _parser.ReadFile(_iniFile);
            return ReadFromIniData(costData);
        }

        public static void SaveHumanCostSettings(HumanCostSettings settings)
        {
            var costData = _parser.ReadFile(_iniFile);
            WriteToIniData(costData, settings);
            _parser.WriteFile(_iniFile, costData);
        }

        // Асинхронные методы
        public static Task<HumanCostSettings> LoadHumanCostSettingsAsync()
        {
            return Task.Run(() =>
            {
                var costData = _parser.ReadFile(_iniFile);
                return ReadFromIniData(costData);
            });
        }

        public static Task SaveHumanCostSettingsAsync(HumanCostSettings settings)
        {
            return Task.Run(() =>
            {
                var costData = _parser.ReadFile(_iniFile);
                WriteToIniData(costData, settings);
                _parser.WriteFile(_iniFile, costData);
            });
        }
        // Чтение из файла
        private static HumanCostSettings ReadFromIniData(IniData costData)
        {
            if (costData == null)
                return new HumanCostSettings();

            double Parse(string? s)
            {
                if (string.IsNullOrWhiteSpace(s))
                    return 0.0;
                return double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, _csvCulture, out var v) ? v : 0.0;
            }

            return new HumanCostSettings
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
        private static void WriteToIniData(IniData costData, HumanCostSettings settings)
        {
            if (costData == null)
                return;

            string F(double v) => v.ToString(_csvCulture);

            costData["HumanCostSettings"]["obvzyakaProduction"] = F(settings.ObvzyakaProduction);
            costData["HumanCostSettings"]["collectorProduction"] = F(settings.CollectorProduction);
            costData["HumanCostSettings"]["tests"] = F(settings.Tests);
            costData["HumanCostSettings"]["commonCheckStand"] = F(settings.CommonCheckStand);
            costData["HumanCostSettings"]["timeForCheckStand"] = F(settings.TimeForCheckStand);
            costData["HumanCostSettings"]["timeForFinalWork"] = F(settings.TimeForFinalWork);
            costData["HumanCostSettings"]["timeForOneDrill"] = F(settings.TimeForOneDrill);
            costData["HumanCostSettings"]["timeForCollectorBoil"] = F(settings.TimeForCollectorBoil);
            costData["HumanCostSettings"]["timeForAllChecks"] = F(settings.TimeForAllChecks);
            costData["HumanCostSettings"]["timeForPrepareAllEquipment"] = F(settings.TimeForPrepareAllEquipment);
            costData["HumanCostSettings"]["timeForDrillOneBus"] = F(settings.TimeForDrillOneBus);
            costData["HumanCostSettings"]["timeForMontageOneInput"] = F(settings.TimeForMontageOneInput);
            costData["HumanCostSettings"]["timeForOthersOperations"] = F(settings.TimeForOthersOperations);

            _parser.WriteFile(_iniFile, costData);
        }
    }
}
