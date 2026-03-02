using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportEngine.App.Model.CalculationModels;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
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



            var bracketUniversal = await _genericRepository.GetAsync<SensorBrace>(x => x.Name == settings.BracketUniversal);
            var bracketDif = await _genericRepository.GetAsync<SensorBrace>(x => x.Name == settings.BracketForDif);
            var bracketAbs = await _genericRepository.GetAsync<SensorBrace>(x => x.Name == settings.BracketForAbs);

            //всрато, неизвестно в какой таблице лежит по факту
            var steelChannel = await _genericRepository.GetAsync<FrameRoll>(x => x.Name == settings.SteelChannel);
            var clamp = await _genericRepository.GetAsync<AdditionalEquipPurpose>(x => x.Material == settings.Clamp); 

            
           

            //stand.AllAdditionalEquipPurposesInStand = new ObservableCollection<AdditionalEquipPurpose>
            //{
            //    new() { Purpose = "Швеллер", Material = settings.SteelChannel, Measure = steelChannel?.Measure, CostPerUnit = steelChannel?.Cost },
            //    new() { Purpose = "Хомуты" , Material = settings.Clamp, Measure = clamp?.Measure, CostPerUnit = clamp?.CostPerUnit},
            //    new() { Purpose = "Шильдик", Material = settings.NamePlate, Quantity = nameplatesPerStand, Measure = settings.NamePlateMeasure},
            //    new() { Purpose = "Табличка", Material = settings.NameTable, Measure = settings.NameTableMeasure},
            //    new() { Purpose = "Кронштейн универсальный",Material = settings.BracketUniversal, Measure = bracketUniversal?.Measure,CostPerUnit = bracketUniversal?.Cost},
            //    new() { Purpose = "Кронштейн перепадчика",Material = settings.BracketForDif, Measure = bracketDif?.Measure,CostPerUnit = bracketDif?.Cost},
            //    new() { Purpose = "Кронштейн абсолютника", Material = settings.BracketForAbs, Measure = bracketAbs?.Measure,CostPerUnit = bracketAbs?.Cost}
            //};
        }

        public async Task InitializeElectricalComponent(StandModel stand, StandSettingsModel settings)
        {
            float? usualConnectionBoxQuantity = 1.0f;
            float? usualCablesQuantity = 2.0f;

            //всрато, неизвестно в какой таблице лежит по факту

            var signalCable = await _genericRepository.GetAsync<CabelProduction>(x => x.Name == settings.SignalCable);
            var cableSixMm = await _genericRepository.GetAsync<CabelProduction>(x => x.Name == settings.CabelSixMm);
            var cableFourMm = await _genericRepository.GetAsync<CabelProduction>(x => x.Name == settings.CabelFourMm);
            var terminal = await _genericRepository.GetAsync<Other>(x => x.Name == settings.Terminal);

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
