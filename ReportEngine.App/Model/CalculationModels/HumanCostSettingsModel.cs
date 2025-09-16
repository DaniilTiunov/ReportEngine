using ReportEngine.App.ViewModels;
using ReportEngine.Shared.CalculationSettings;
using ReportEngine.Shared.Config.IniHeleprs;

namespace ReportEngine.App.Model.CalculationModels
{
    public class HumanCostSettingsModel : BaseViewModel
    {
        private double _obvzyakaProduction;
        private double _collectorProduction;
        private double _tests;
        private double _commonCheckStand;
        private double _timeForCheckStand;
        private double _timeForFinalWork;
        private double _timeForOneDrill;
        private double _timeForCollectorBoil;
        private double _timeForAllChecks;
        private double _timeForPrepareAllEquipment;
        private double _timeForDrillOneBus;
        private double _timeForMontageOneInput;
        private double _timeForOthersOperations;
        public double ObvzyakaProduction 
        { 
            get => _obvzyakaProduction; 
            set => Set(ref _obvzyakaProduction, value); 
        }

        public double CollectorProduction 
        { 
            get => _collectorProduction; 
            set => Set(ref _collectorProduction, value); 
        }

        public double Tests 
        { 
            get => _tests; 
            set => Set(ref _tests, value); 
        }

        public double CommonCheckStand 
        { 
            get => _commonCheckStand; 
            set => Set(ref _commonCheckStand, value); 
        }

        public double TimeForCheckStand 
        { 
            get => _timeForCheckStand; 
            set => Set(ref _timeForCheckStand, value); 
        }

        public double TimeForFinalWork 
        { 
            get => _timeForFinalWork; 
            set => Set(ref _timeForFinalWork, value); 
        }

        public double TimeForOneDrill 
        { 
            get => _timeForOneDrill; 
            set => Set(ref _timeForOneDrill, value); 
        }

        public double TimeForCollectorBoil 
        { 
            get => _timeForCollectorBoil; 
            set => Set(ref _timeForCollectorBoil, value); 
        }

        public double TimeForAllChecks 
        { 
            get => _timeForAllChecks; 
            set => Set(ref _timeForAllChecks, value); 
        }

        public double TimeForPrepareAllEquipment 
        { 
            get => _timeForPrepareAllEquipment; 
            set => Set(ref _timeForPrepareAllEquipment, value); 
        }

        public double TimeForDrillOneBus 
        { 
            get => _timeForDrillOneBus; 
            set => Set(ref _timeForDrillOneBus, value); 
        }

        public double TimeForMontageOneInput 
        { 
            get => _timeForMontageOneInput; 
            set => Set(ref _timeForMontageOneInput, value); 
        }

        public double TimeForOthersOperations
        {
            get => _timeForOthersOperations; 
            set => Set(ref _timeForOthersOperations, value);
        }

        public async Task LoadDataFromIniAsync()
        {
            var iniData = await CalculationSettingsManager.LoadHumanCostSettingsAsync();

            if (iniData == null) return;

            ObvzyakaProduction = iniData.ObvzyakaProduction;
            CollectorProduction = iniData.CollectorProduction;
            Tests = iniData.Tests;
            CommonCheckStand = iniData.CommonCheckStand;
            TimeForCheckStand = iniData.TimeForCheckStand;
            TimeForFinalWork = iniData.TimeForFinalWork;
            TimeForOneDrill = iniData.TimeForOneDrill;
            TimeForCollectorBoil = iniData.TimeForCollectorBoil;
            TimeForAllChecks = iniData.TimeForAllChecks;
            TimeForPrepareAllEquipment = iniData.TimeForPrepareAllEquipment;
            TimeForDrillOneBus = iniData.TimeForDrillOneBus;
            TimeForMontageOneInput = iniData.TimeForMontageOneInput;
            TimeForOthersOperations = iniData.TimeForOthersOperations;
        }
        public async Task SaveDataToIniAsync()
        {
            var iniData = new HumanCostSettings()
            {
                ObvzyakaProduction = ObvzyakaProduction,
                CollectorProduction = CollectorProduction,
                Tests = Tests,
                CommonCheckStand = CommonCheckStand,
                TimeForCheckStand = TimeForCheckStand,
                TimeForFinalWork = TimeForFinalWork,
                TimeForOneDrill = TimeForOneDrill,
                TimeForCollectorBoil = TimeForCollectorBoil,
                TimeForAllChecks = TimeForAllChecks,
                TimeForPrepareAllEquipment = TimeForPrepareAllEquipment,
                TimeForDrillOneBus = TimeForDrillOneBus,
                TimeForMontageOneInput = TimeForMontageOneInput,
                TimeForOthersOperations = TimeForOthersOperations
            };
            
            await CalculationSettingsManager.SaveHumanCostSettingsAsync(iniData);
        }
    }
}
