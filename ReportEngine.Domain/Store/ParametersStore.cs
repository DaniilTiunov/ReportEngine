using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ReportEngine.Domain.DTO;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.CalculationParameters;
using ReportEngine.Domain.Entities.CalculationParameters.Enums;
using ReportEngine.Domain.Repositories;

namespace ReportEngine.Domain.Store;

public class ParametersStore
{
    private readonly CalculationRepository _calculationRepository;

    private readonly Dictionary<CalculationParameterType, Dictionary<string, CalculationParameter>> _allSettings = new();

    private readonly Dictionary<CalculationParameter, ParameterWithEquip?> _parameterEquipsPairs = new();

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

        //сворачиваем все параметры в список
        var allParameters = _allSettings
            .SelectMany(groupDictionary => groupDictionary.Value)
            .Select(keyParamPair => keyParamPair.Value)
            .ToList();

        foreach (var parameter in allParameters)
        {
            //если нет ссылки на внешний компонент - пропускаем
            if (!parameter.EquipReferenceId.HasValue || string.IsNullOrEmpty(parameter.EquipReferenceType))
            {
                _parameterEquipsPairs[parameter] = null;
                continue;
            }

            //в противном случае запрашиваем инфу с базы
            _parameterEquipsPairs[parameter] =
                await _calculationRepository.GetParameterWithEquipAsync(parameter.Id, parameter.EquipReferenceId.Value, parameter.EquipReferenceType);
        }
    }

    public CalculationParameter GetCurrentParameter(CalculationParameterType type, string key)
    {
        if (_allSettings.TryGetValue(type, out var dict)
            && dict.TryGetValue(key, out var value))
            return value;

        throw new KeyNotFoundException
            ($"Ключ '{key}' или тип группы '{type}' не найдены.");
    }


    public ParameterWithEquip? GetParameterEquip(CalculationParameter parameter)
    {
        if (_parameterEquipsPairs.TryGetValue(parameter,out var resultParameterEquip))
            return resultParameterEquip;

        throw new KeyNotFoundException
            ($"Параметр '{parameter.Name}' не найден в словаре.");
    }

    public (CalculationParameter Parameter, ParameterWithEquip? Equip)
        GetParameterData(CalculationParameterType type, string key)
    {
        var parameter = GetCurrentParameter(type, key);
        var equip = GetParameterEquip(parameter);

        return (parameter, equip);
    }

    public ParameterWithEquip? this[CalculationParameter parameter]
        => GetParameterEquip(parameter);

    public CalculationParameter this[CalculationParameterType type, string key]
        => GetCurrentParameter(type, key);
}
