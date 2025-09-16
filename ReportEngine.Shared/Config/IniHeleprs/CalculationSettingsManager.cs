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
                ObvzyakaProduction = float.Parse(costData["HumanCostSettings"]["obvzyakaProduction"]),
                CollectorProduction = float.Parse(costData["HumanCostSettings"]["collectorProduction"]),
                Tests = float.Parse(costData["HumanCostSettings"]["tests"]),
                CommonCheckStand = float.Parse(costData["HumanCostSettings"]["commonCheckStand"]),
                TimeForCheckStand = float.Parse(costData["HumanCostSettings"]["timeForCheckStand"]),
                TimeForFinalWork = float.Parse(costData["HumanCostSettings"]["timeForFinalWork"]),
                TimeForOneDrill = float.Parse(costData["HumanCostSettings"]["timeForOneDrill"]),
                TimeForCollectorBoil = float.Parse(costData["HumanCostSettings"]["timeForCollectorBoil"]),
                TimeForAllChecks = float.Parse(costData["HumanCostSettings"]["timeForAllChecks"]),
                TimeForPrepareAllEquipment = float.Parse(costData["HumanCostSettings"]["timeForPrepareAllEquipment"])       ,
                TimeForDrillOneBus = float.Parse(costData["HumanCostSettings"]["timeForDrillOneBus"]),
                TimeForMontageOneInput = float.Parse(costData["HumanCostSettings"]["timeForMontageOneInput"]),
                TimeForOthersOperations = float.Parse(costData["HumanCostSettings"]["timeForOthersOperations"])
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
