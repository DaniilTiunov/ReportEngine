using ReportEngine.Domain.Entities.CalculationParameters;
using ReportEngine.Domain.Repositories;

namespace ReportEngine.App.Store;

public class ParametersStore
{
    private readonly CalculationRepository _calculationRepository;

    private readonly Dictionary<CalculationParameterType, Dictionary<string, CalculationParameter>> _allSettings
        = new();


    public ParametersStore(CalculationRepository calculationRepository)
    {
        _calculationRepository = calculationRepository;
    }

    public async Task LoadSettingsDataAsync()
    {
        _allSettings.Clear();

        _allSettings[CalculationParameterType.ElectricCost] =
            await _calculationRepository.GetByKeysAsync(CalculationParameterType.ElectricCost, StoreKeys.ElectricRequired);
        _allSettings[CalculationParameterType.StandCost] =
            await _calculationRepository.GetByKeysAsync(CalculationParameterType.StandCost, StoreKeys.StandsRequired);
        _allSettings[CalculationParameterType.HumanCost] =
            await _calculationRepository.GetByKeysAsync(CalculationParameterType.HumanCost, StoreKeys.HumanCostRequired);
        _allSettings[CalculationParameterType.FrameCost] =
            await _calculationRepository.GetByKeysAsync(CalculationParameterType.FrameCost, StoreKeys.FramesSettingsRequired);
        _allSettings[CalculationParameterType.SandBlastCost] =
            await _calculationRepository.GetByKeysAsync(CalculationParameterType.SandBlastCost, StoreKeys.SandBlastSettingsRequired);
        _allSettings[CalculationParameterType.Equipments] =
            await _calculationRepository.GetByKeysAsync(CalculationParameterType.Equipments, StoreKeys.EquipmentsSettingsRequired);
    }

    public CalculationParameter GetCurrentParameter(CalculationParameterType type, string key)
    {
        if (_allSettings.TryGetValue(type, out var dict)
            && dict.TryGetValue(key, out var value))
            return value;

        throw new KeyNotFoundException
            ($"Ключ '{key}' или тип группы '{type}' не найдены.");
    }

    public CalculationParameter this[CalculationParameterType type, string key]
        => GetCurrentParameter(type, key);
}
