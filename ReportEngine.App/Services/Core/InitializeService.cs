using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ReportEngine.App.Model.CalculationModels;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.Braces;
using ReportEngine.Domain.Entities.ElectricComponents;
using ReportEngine.Domain.Entities.Frame;
using ReportEngine.Domain.Entities.Other;
using ReportEngine.Domain.Repositories;

namespace ReportEngine.App.Services.Core
{
    public class InitializeService
    {
        private readonly GenericRepository _genericRepository;

        public InitializeService(GenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }



        public async Task InitializeStandDefaultPurposes(StandModel standForInitialize)
        {
            var defaultSettings = new StandSettingsModel();
            await defaultSettings.LoadStandsSettingsDataAsync();

            InitializeObvAdditionalPurposes(standForInitialize);
            InitializeDrainagePurposes(standForInitialize);
            await InitializeElectricalComponent(standForInitialize,defaultSettings);
            await InitializeAdditionalEquip(standForInitialize, defaultSettings);
                      
        }

        public void InitializeObvAdditionalPurposes(StandModel stand)
        {
            stand.ObvyazkaAdditionalComponents = new ObservableCollection<ObvyazkaAdditionalEquipPurpose>
        {
             new() { Purpose = "Доп.компонент" },
        };
        }

        public void InitializeDrainagePurposes(StandModel stand)
        {
            const float endPipeQuantityPerStand = 0.2f;
            const float pipePlugQuantityPerStand = 2.0f;

            stand.AllDrainagePurposesInStand = new ObservableCollection<DrainagePurpose>
            {
                new() { Purpose = "Основная труба" , Measure = "м"},
                new() { Purpose = "Патрубок", Quantity = endPipeQuantityPerStand, Measure = "м"},
                new() { Purpose = "Заглушка основной трубы", Quantity = pipePlugQuantityPerStand,  Measure = "м" },
                new() { Purpose = "Кронштейн дренажа" },
                new() { Purpose = "Клапан" }
            };
        }

        public async Task InitializeAdditionalEquip(StandModel stand, StandSettingsModel settings)
        {
            const float nameplatesPerStand = 1.0f;


            var bracketUniversalEntityType = _genericRepository.GetEntityTypeByName(settings.BracketUniversalEntityName ?? "") ?? typeof(SensorBrace);
            var bracketUniversal = await _genericRepository.GetByNameAsync(bracketUniversalEntityType, settings.BracketUniversal);

            var bracketDifEntityType = _genericRepository.GetEntityTypeByName(settings.BracketForDifEntityName ?? "") ?? typeof(SensorBrace);
            var bracketDif = await _genericRepository.GetByNameAsync(bracketDifEntityType, settings.BracketForDif);

            var bracketAbsEntityType = _genericRepository.GetEntityTypeByName(settings.BracketForAbsEntityName ?? "") ?? typeof(SensorBrace);
            var bracketAbs = await _genericRepository.GetByNameAsync(bracketAbsEntityType, settings.BracketForAbs);

            var steelChannelEntityType = _genericRepository.GetEntityTypeByName(settings.SteelChannelEntityName ?? "") ?? typeof(FrameRoll);
            var steelChannel = await _genericRepository.GetByNameAsync(steelChannelEntityType, settings.SteelChannel);

            var clampEntityType = _genericRepository.GetEntityTypeByName(settings.ClampEntityName ?? "") ?? typeof(Other);
            var clamp = await _genericRepository.GetByNameAsync(clampEntityType, settings.Clamp);

            var nameTableEntityType = _genericRepository.GetEntityTypeByName(settings.NameTableEntityName ?? "") ?? typeof(Other);
            var nameTable = await _genericRepository.GetByNameAsync(nameTableEntityType, settings.NameTable);

            var namePlateEntityType = _genericRepository.GetEntityTypeByName(settings.NamePlate ?? "") ?? typeof(Other);
            var namePlate = await _genericRepository.GetByNameAsync(namePlateEntityType, settings.NamePlate);


            stand.AllAdditionalEquipPurposesInStand = new ObservableCollection<AdditionalEquipPurpose>
            {
                new() { Purpose = "Швеллер", Material = settings.SteelChannel, Measure = steelChannel?.Measure, CostPerUnit = steelChannel?.Cost },
                new() { Purpose = "Хомуты" , Material = settings.Clamp, Measure = clamp?.Measure, CostPerUnit = clamp?.Cost},
                new() { Purpose = "Шильдик", Material = settings.NamePlate, Quantity = nameplatesPerStand, Measure = namePlate?.Measure,CostPerUnit = namePlate?.Cost},
                new() { Purpose = "Табличка", Material = settings.NameTable, Measure = nameTable?.Measure, CostPerUnit = nameTable?.Cost},
                new() { Purpose = "Кронштейн универсальный",Material = settings.BracketUniversal, Measure = bracketUniversal?.Measure,CostPerUnit = bracketUniversal?.Cost},
                new() { Purpose = "Кронштейн перепадчика",Material = settings.BracketForDif, Measure = bracketDif?.Measure,CostPerUnit = bracketDif?.Cost},
                new() { Purpose = "Кронштейн абсолютника", Material = settings.BracketForAbs, Measure = bracketAbs?.Measure,CostPerUnit = bracketAbs?.Cost}
            };
        }

        public async Task InitializeElectricalComponent(StandModel stand, StandSettingsModel settings)
        {
            float? usualConnectionBoxQuantity = 1.0f;
            float? usualCablesQuantity = 2.0f;


            var signalCableEntityType = _genericRepository.GetEntityTypeByName(settings.SignalCableEntityName ?? "") ?? typeof(CabelProduction);
            var signalCable = await _genericRepository.GetByNameAsync(signalCableEntityType, settings.SignalCable);

            var cableSixMmEntityType = _genericRepository.GetEntityTypeByName(settings.CabelSixMmEntityName ?? "") ?? typeof(CabelProduction);
            var cableSixMm = await _genericRepository.GetByNameAsync(cableSixMmEntityType, settings.CabelSixMm);

            var cableFourMmEntityType = _genericRepository.GetEntityTypeByName(settings.CabelFourMmEntityName ?? "") ?? typeof(CabelProduction);
            var cableFourMm = await _genericRepository.GetByNameAsync(cableFourMmEntityType, settings.CabelFourMm);

            var terminalEntityType = _genericRepository.GetEntityTypeByName(settings.TerminalEntityName ?? "") ?? typeof(CabelProduction);
            var terminal = await _genericRepository.GetByNameAsync(terminalEntityType, settings.Terminal);

            stand.AllElectricalPurposesInStand = new ObservableCollection<ElectricalPurpose>
            {
                new() { Purpose = "Клеммная коробка" ,Quantity = usualConnectionBoxQuantity, Measure = "шт"},
                new() { Purpose = "Кабельные вводы" , Quantity = 1, Measure = "шт"},
                new() { Purpose = "Сигнальный кабель", Material = settings.SignalCable, Quantity = usualCablesQuantity , Measure = signalCable?.Measure, CostPerUnit = signalCable?.Cost},
                new() { Purpose = "Металлорукав" , Quantity = usualCablesQuantity, Measure = "м"},
                new() { Purpose = "Кабель 6мм", Material = settings.CabelSixMm, Quantity = (float?) settings.SensorCountOnFrame , Measure = cableSixMm?.Measure, CostPerUnit = cableSixMm?.Cost },
                new() { Purpose = "Кабель 4мм", Material = settings.CabelFourMm, Quantity = usualCablesQuantity, Measure = cableFourMm?.Measure, CostPerUnit = cableFourMm?.Cost },
                new() { Purpose = "Кронштейн коробки" },
                new() { Purpose = "Клемма", Material = settings.Terminal, Measure = terminal?.Measure, CostPerUnit = terminal?.Cost }
            };
        }
    }
}
