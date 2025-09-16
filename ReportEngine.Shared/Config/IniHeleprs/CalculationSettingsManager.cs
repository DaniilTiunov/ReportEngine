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

            return new HumanCostSettings
            {
                ObvzyakaProduction = Convert.ToDouble(costData["HumanCostSettings"]["obvzyakaProduction"], CultureInfo.InvariantCulture),
                CollectorProduction = Convert.ToDouble(costData["HumanCostSettings"]["collectorProduction"], CultureInfo.InvariantCulture),
                Tests = Convert.ToDouble(costData["HumanCostSettings"]["tests"], CultureInfo.InvariantCulture),
                CommonCheckStand = Convert.ToDouble(costData["HumanCostSettings"]["commonCheckStand"], CultureInfo.InvariantCulture),
                TimeForCheckStand = Convert.ToDouble(costData["HumanCostSettings"]["timeForCheckStand"], CultureInfo.InvariantCulture),
                TimeForFinalWork = Convert.ToDouble(costData["HumanCostSettings"]["timeForFinalWork"], CultureInfo.InvariantCulture),
                TimeForOneDrill = Convert.ToDouble(costData["HumanCostSettings"]["timeForOneDrill"], CultureInfo.InvariantCulture),
                TimeForCollectorBoil = Convert.ToDouble(costData["HumanCostSettings"]["timeForCollectorBoil"], CultureInfo.InvariantCulture),
                TimeForAllChecks = Convert.ToDouble(costData["HumanCostSettings"]["timeForAllChecks"], CultureInfo.InvariantCulture),
                TimeForPrepareAllEquipment = Convert.ToDouble(costData["HumanCostSettings"]["timeForPrepareAllEquipment"], CultureInfo.InvariantCulture)       ,
                TimeForDrillOneBus = Convert.ToDouble(costData["HumanCostSettings"]["timeForDrillOneBus"], CultureInfo.InvariantCulture),
                TimeForMontageOneInput = Convert.ToDouble(costData["HumanCostSettings"]["timeForMontageOneInput"], CultureInfo.InvariantCulture),
                TimeForOthersOperations = Convert.ToDouble(costData["HumanCostSettings"]["timeForOthersOperations"], CultureInfo.InvariantCulture)
            };
        }
        // Запись в файл
        private static void WriteToIniData(IniData costData, HumanCostSettings settings)
        {
            if (costData == null)
                return;

            costData["HumanCostSettings"]["obvzyakaProduction"] = settings.ObvzyakaProduction.ToString();
            costData["HumanCostSettings"]["collectorProduction"] = settings.CollectorProduction.ToString();
            costData["HumanCostSettings"]["tests"] = settings.Tests.ToString();
            costData["HumanCostSettings"]["commonCheckStand"] = settings.CommonCheckStand.ToString();
            costData["HumanCostSettings"]["timeForCheckStand"] = settings.TimeForCheckStand.ToString();
            costData["HumanCostSettings"]["timeForFinalWork"] = settings.TimeForFinalWork.ToString();
            costData["HumanCostSettings"]["timeForOneDrill"] = settings.TimeForOneDrill.ToString();
            costData["HumanCostSettings"]["timeForCollectorBoil"] = settings.TimeForCollectorBoil.ToString();
            costData["HumanCostSettings"]["timeForAllChecks"] = settings.TimeForAllChecks.ToString();
            costData["HumanCostSettings"]["timeForPrepareAllEquipment"] = settings.TimeForPrepareAllEquipment.ToString();
            costData["HumanCostSettings"]["timeForDrillOneBus"] = settings.TimeForDrillOneBus.ToString();
            costData["HumanCostSettings"]["timeForMontageOneInput"] = settings.TimeForMontageOneInput.ToString();
            costData["HumanCostSettings"]["timeForOthersOperations"] = settings.TimeForOthersOperations.ToString();

            _parser.WriteFile(_iniFile, costData);
        }
    }
}
