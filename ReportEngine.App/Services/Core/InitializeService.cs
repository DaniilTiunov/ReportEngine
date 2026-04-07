using System.Collections.ObjectModel;
using ReportEngine.App.Model.CalculationModels;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Braces;
using ReportEngine.Domain.Entities.CalculationParameters.Enums;
using ReportEngine.Domain.Entities.ElectricComponents;
using ReportEngine.Domain.Entities.Frame;
using ReportEngine.Domain.Entities.Other;
using ReportEngine.Domain.Repositories;
using ReportEngine.Domain.Store;

namespace ReportEngine.App.Services.Core;

public class InitializeService
{
    private readonly GenericRepository _genericRepository;
    private readonly ParametersStore _parametersStore;

    public InitializeService(GenericRepository genericRepository, ParametersStore parametersStore)
    {
        _genericRepository = genericRepository;
        _parametersStore = parametersStore;
    }

    public async Task InitializeStandDefaultPurposes(StandModel standForInitialize)
    {
        var defaultSettings = new StandSettingsModel();
        await defaultSettings.LoadStandsSettingsDataAsync();

        InitializeObvAdditionalPurposes(standForInitialize);
        InitializeDrainagePurposes(standForInitialize);
        await InitializeElectricalComponent(standForInitialize, defaultSettings);
        await InitializeAdditionalEquip(standForInitialize, defaultSettings);
    }

    public void InitializeObvAdditionalPurposes(StandModel stand)
    {
        stand.ObvyazkaAdditionalComponents = new ObservableCollection<ObvyazkaAdditionalEquipPurpose>
        {
            new() { Purpose = "Доп.компонент" }
        };
    }

    public void InitializeDrainagePurposes(StandModel stand)
    {
        const float endPipeQuantityPerStand = 0.2f;
        const float pipePlugQuantityPerStand = 2.0f;

        stand.AllDrainagePurposesInStand = new ObservableCollection<DrainagePurpose>
        {
            new() { Purpose = "Основная труба", Measure = "м" },
            new() { Purpose = "Патрубок", Quantity = endPipeQuantityPerStand, Measure = "м" },
            new() { Purpose = "Заглушка основной трубы", Quantity = pipePlugQuantityPerStand, Measure = "м" },
            new() { Purpose = "Кронштейн дренажа" },
            new() { Purpose = "Клапан" }
        };
    }

    public async Task InitializeAdditionalEquip(StandModel stand, StandSettingsModel settings)
    {
        const float nameplatesPerStand = 1.0f;

        var bracketUniversalParameter =_parametersStore[CalculationParameterType.Equipments, "Clamps"];
        var bracketUniversal = _parametersStore[bracketUniversalParameter]?.Equipment;

        var bracketDifParameter = _parametersStore[CalculationParameterType.Equipments, "DiffPressureBracket"];
        var bracketDif = _parametersStore[bracketDifParameter]?.Equipment;

        var bracketAbsParameter = _parametersStore[CalculationParameterType.Equipments, "AbsPressureBracket"];
        var bracketAbs = _parametersStore[bracketAbsParameter]?.Equipment;

        var steelChannelParameter = _parametersStore[CalculationParameterType.Equipments, "ChannelBar"];
        var steelChannel = _parametersStore[steelChannelParameter]?.Equipment;

        var clampParameter = _parametersStore[CalculationParameterType.Equipments, "Clamps"];
        var clamp = _parametersStore[clampParameter]?.Equipment;

        var nameTableParameter = _parametersStore[CalculationParameterType.Equipments, "LabelPlate"];
        var nameTable = _parametersStore[nameTableParameter]?.Equipment;

        var namePlateParameter = _parametersStore[CalculationParameterType.Equipments, "Nameplate"];
        var namePlate = _parametersStore[namePlateParameter]?.Equipment;


        stand.AllAdditionalEquipPurposesInStand = new ObservableCollection<AdditionalEquipPurpose>
        {
            new()
            {
                Purpose = "Швеллер", Material = steelChannel?.Name, Measure = steelChannel?.Measure,
                CostPerUnit = steelChannel?.Cost
            },
            new()
            {
                Purpose = "Хомуты", Material =clamp?.Name, Measure = clamp?.Measure, CostPerUnit = clamp?.Cost
            },
            new()
            {
                Purpose = "Шильдик", Material = namePlate?.Name, Quantity = nameplatesPerStand,
                Measure = namePlate?.Measure, CostPerUnit = namePlate?.Cost
            },
            new()
            {
                Purpose = "Табличка", Material = nameTable?.Name, Measure = nameTable?.Measure,
                CostPerUnit = nameTable?.Cost
            },
            new()
            {
                Purpose = "Кронштейн универсальный", Material = bracketUniversal?.Name,
                Measure = bracketUniversal?.Measure, CostPerUnit = bracketUniversal?.Cost
            },
            new()
            {
                Purpose = "Кронштейн перепадчика", Material = bracketDif?.Name, Measure = bracketDif?.Measure,
                CostPerUnit = bracketDif?.Cost
            },
            new()
            {
                Purpose = "Кронштейн абсолютника", Material = bracketAbs?.Name, Measure = bracketAbs?.Measure,
                CostPerUnit = bracketAbs?.Cost
            }
        };
    }

    public async Task InitializeElectricalComponent(StandModel stand, StandSettingsModel settings)
    {
        float? usualConnectionBoxQuantity = 1.0f;
        float? usualCablesQuantity = 2.0f;

        var signalCableParameter = _parametersStore[CalculationParameterType.Equipments, "SignalCable"];
        var signalCable = _parametersStore[signalCableParameter]?.Equipment;

        var cableSixMmParameter = _parametersStore[CalculationParameterType.Equipments, "Cable6mm"];
        var cableSixMm = _parametersStore[cableSixMmParameter]?.Equipment;

        var cableFourMmParameter = _parametersStore[CalculationParameterType.Equipments, "Cable4mm"];
        var cableFourMm = _parametersStore[cableFourMmParameter]?.Equipment;

        var terminalParameter = _parametersStore[CalculationParameterType.Equipments, "Terminal"];
        var terminal = _parametersStore[terminalParameter]?.Equipment;

        stand.AllElectricalPurposesInStand = new ObservableCollection<ElectricalPurpose>
        {
            new() { Purpose = "Клеммная коробка", Quantity = usualConnectionBoxQuantity, Measure = "шт" },
            new() { Purpose = "Кабельные вводы", Quantity = 1, Measure = "шт" },
            new()
            {
                Purpose = "Сигнальный кабель", Material = signalCable?.Name, Quantity = usualCablesQuantity,
                Measure = signalCable?.Measure, CostPerUnit = signalCable?.Cost
            },
            new() { Purpose = "Металлорукав", Quantity = usualCablesQuantity, Measure = "м" },
            new()
            {
                Purpose = "Кабель 6мм", Material = cableSixMm?.Name, Quantity = (float?)settings.SensorCountOnFrame,
                Measure = cableSixMm?.Measure, CostPerUnit = cableSixMm?.Cost
            },
            new()
            {
                Purpose = "Кабель 4мм", Material = cableFourMm?.Name, Quantity = usualCablesQuantity,
                Measure = cableFourMm?.Measure, CostPerUnit = cableFourMm?.Cost
            },
            new() { Purpose = "Кронштейн коробки" },
            new()
            {
                Purpose = "Клемма", Material = terminal?.Name, Measure = terminal?.Measure,
                CostPerUnit = terminal?.Cost
            }
        };
    }
}
