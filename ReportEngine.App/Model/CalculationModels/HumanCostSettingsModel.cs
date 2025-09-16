using ReportEngine.App.ViewModels;
using ReportEngine.Shared.CalculationSettings;
using ReportEngine.Shared.Config.IniHeleprs;

namespace ReportEngine.App.Model.CalculationModels
{
    public class HumanCostSettingsModel : BaseViewModel
    {
        private float _obvzyakaProduction;
        private float _collectorProduction;
        private float _tests;
        private float _commonCheckStand;
        private float _timeForCheckStand;
        private float _timeForFinalWork;
        private float _timeForOneDrill;
        private float _timeForCollectorBoil;
        private float _timeForAllChecks;
        private float _timeForPrepareAllEquipment;
        private float _timeForDrillOneBus;
        private float _timeForMontageOneInput;
        private float _timeForOthersOperations;
        public float ObvzyakaProduction 
        { 
            get => _obvzyakaProduction; 
            set => Set(ref _obvzyakaProduction, value); 
        }

        public float CollectorProduction 
        { 
            get => _collectorProduction; 
            set => Set(ref _collectorProduction, value); 
        }

        public float Tests 
        { 
            get => _tests; 
            set => Set(ref _tests, value); 
        }

        public float CommonCheckStand 
        { 
            get => _commonCheckStand; 
            set => Set(ref _commonCheckStand, value); 
        }

        public float TimeForCheckStand 
        { 
            get => _timeForCheckStand; 
            set => Set(ref _timeForCheckStand, value); 
        }

        public float TimeForFinalWork 
        { 
            get => _timeForFinalWork; 
            set => Set(ref _timeForFinalWork, value); 
        }

        public float TimeForOneDrill 
        { 
            get => _timeForOneDrill; 
            set => Set(ref _timeForOneDrill, value); 
        }

        public float TimeForCollectorBoil 
        { 
            get => _timeForCollectorBoil; 
            set => Set(ref _timeForCollectorBoil, value); 
        }

        public float TimeForAllChecks 
        { 
            get => _timeForAllChecks; 
            set => Set(ref _timeForAllChecks, value); 
        }

        public float TimeForPrepareAllEquipment 
        { 
            get => _timeForPrepareAllEquipment; 
            set => Set(ref _timeForPrepareAllEquipment, value); 
        }

        public float TimeForDrillOneBus 
        { 
            get => _timeForDrillOneBus; 
            set => Set(ref _timeForDrillOneBus, value); 
        }

        public float TimeForMontageOneInput 
        { 
            get => _timeForMontageOneInput; 
            set => Set(ref _timeForMontageOneInput, value); 
        }

        public float TimeForOthersOperations
        {
            get => _timeForOthersOperations; 
            set => Set(ref _timeForOthersOperations, value);
        }

        public void LoadDataFromIni()
        {
            var iniData = CalculationSettingsManager.LoadHumanCostSettings();

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
        public void SaveDataToIni()
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
            CalculationSettingsManager.SaveHumanCostSettings(iniData);
        }
    }
}
