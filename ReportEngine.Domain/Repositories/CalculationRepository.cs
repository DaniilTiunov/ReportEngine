using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.DTO;
using ReportEngine.Domain.Entities.CalculationParameters;
using ReportEngine.Domain.Entities.CalculationParameters.Enums;
using ReportEngine.Domain.Extensions;

namespace ReportEngine.Domain.Repositories;

public class CalculationRepository
{
    private readonly ReAppContext _context;

    public CalculationRepository(ReAppContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Асинхронное получение всех групп вместе с параметрами
    /// </summary>
    /// <returns>Коллекция групп, включая параметры внутри</returns>
    public async Task<IEnumerable<CalculationParameterGroup>> GetAllGroupsAsync()
    {
        return await _context.Set<CalculationParameterGroup>()
            .AsNoTracking()
            .Include(group => group.Parameters)
            .ToListAsync();
    }

    /// <summary>
    ///     Асинхронное получение группы по типу вместе с параметрами
    /// </summary>
    /// <returns>Группа, включая параметры внутри</returns>
    public async Task<CalculationParameterGroup?> GetGroupByTypeAsync(CalculationParameterType type)
    {
        return await _context.Set<CalculationParameterGroup>()
            .AsNoTracking()
            .Include(group => group.Parameters)
            .FirstOrDefaultAsync(group => group.SettingsType == type);
    }

    /// <summary>
    ///     Асинхронное получение коллекции параметров по типу группы
    /// </summary>
    /// <returns>Коллекция параметров вместе с группой</returns>
    public async Task<List<CalculationParameter>> GetAllParametersInGroupAsync(
        CalculationParameterType groupType)
    {
        return await _context.Set<CalculationParameterGroup>()
            .AsNoTracking()
            .Where(group => group.SettingsType == groupType)
            .SelectMany(group => group.Parameters)
            .ToListAsync();
    }

    public async Task<Dictionary<string, CalculationParameter>> GetByKeysAsync(
        CalculationParameterType type,
        IEnumerable<string> keys)
    {
        return await _context.Set<CalculationParameter>()
            .AsNoTracking()
            .Where(x =>
                x.CalculationParameterGroup.SettingsType == type &&
                keys.Contains(x.Key))
            .ToDictionaryAsync(x => x.Key);
    }

    public async Task<ParameterWithEquip?> GetParameterWithEquipAsync(
        int parameterId,
        int equipmentId,
        string type)
    {
        var parameter =
            await _context.CalculationParameters.FirstOrDefaultAsync(par => par.Id == parameterId);

        var entityType = Type.GetType(type)
                         ?? throw new ArgumentException($"Тип {type} не найден в сборке");

        var table = _context.SetTable(entityType)
                    ?? throw new ArgumentException($"Не удалось найти таблицу под тип {type}");

        var equipment = await table.FirstOrDefaultAsync(equip => equip.Id == equipmentId);

        return new ParameterWithEquip
        {
            Parameter = parameter,
            Equipment = equipment
        };
    }

    public async Task<CalculationParameterGroup> AddGroupAsync(CalculationParameterGroup group)
    {
        var newEntity = new CalculationParameterGroup
        {
            Id = 0,
            Name = group.Name,
            SettingsType = group.SettingsType
        };

        var result = await _context.Set<CalculationParameterGroup>().AddAsync(newEntity);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task AddParameterToGroup(CalculationParameter parameter, CalculationParameterType groupType)
    {
        var existingGroup = await _context.Set<CalculationParameterGroup>()
            .FirstOrDefaultAsync(group => group.SettingsType == groupType);


        if (existingGroup == null) throw new ArgumentException("Группы указанного типа не существует");

        parameter.ParameterGroupId = existingGroup.Id;
        existingGroup.Parameters.Add(parameter);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateGroupAsync(CalculationParameterGroup group)
    {
        var existingGroup = await _context
            .Set<CalculationParameterGroup>()
            .AsNoTracking()
            .FirstOrDefaultAsync(gr => gr.SettingsType == group.SettingsType);


        if (existingGroup == null) throw new ArgumentException("Группы указанного типа не существует");

        _context.Set<CalculationParameterGroup>().Update(existingGroup);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateParametersAsync(List<CalculationParameter> uiParameters)
    {
        // Загружаем из БД только те параметры, которые пришли с UI
        var ids = uiParameters.Select(p => p.Id).ToList();

        var dbParameters = await _context.CalculationParameters
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();

        // Преобразуем список UI-параметров в словарь для быстрого поиска по Id
        var uiMap = uiParameters.ToDictionary(p => p.Id);

        foreach (var dbParam in dbParameters)
        {
            var uiParam = uiMap[dbParam.Id];

            // Обновляем только поля параметра
            dbParam.Key = uiParam.Key;
            dbParam.Name = uiParam.Name;
            dbParam.Value = uiParam.Value;
            dbParam.Unit = uiParam.Unit;
            dbParam.Description = uiParam.Description;
            dbParam.EquipReferenceId = uiParam.EquipReferenceId;
            dbParam.EquipReferenceType = uiParam.EquipReferenceType;
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateParametersInGroup(
        CalculationParameterType groupType,
        IEnumerable<CalculationParameter> updatedParameters)
    {
        var existingGroup = await _context
            .Set<CalculationParameterGroup>()
            .Include(group => group.Parameters)
            .FirstOrDefaultAsync(group => group.SettingsType == groupType);

        if (existingGroup == null)
            throw new ArgumentException("Группы указанного типа не существует");

        foreach (var updatedParam in updatedParameters)
        {
            var existingParam = existingGroup.Parameters
                .FirstOrDefault(x => x.Id == updatedParam.Id);

            if (existingParam == null)
            {
                // новый параметр
                existingGroup.Parameters.Add(updatedParam);
            }
            else
            {
                // обновление существующего
                existingParam.Name = updatedParam.Name;
                existingParam.Value = updatedParam.Value;
                existingParam.Unit = updatedParam.Unit;
                existingParam.Description = updatedParam.Description;
                existingParam.Key = updatedParam.Key;
                existingParam.EquipReferenceId = updatedParam.EquipReferenceId;
                existingParam.EquipReferenceType = updatedParam.EquipReferenceType;
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteGroupAsync(CalculationParameterGroup group)
    {
        var existingGroup = await _context
            .Set<CalculationParameterGroup>()
            .AsNoTracking()
            .FirstOrDefaultAsync(gr => gr.SettingsType == group.SettingsType);


        if (existingGroup == null) throw new ArgumentException("Группы указанного типа не существует");

        _context.Set<CalculationParameterGroup>().Remove(existingGroup);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteParameterFromGroup(CalculationParameter parameter, CalculationParameterType groupType)
    {
        var existingGroup = await _context
            .Set<CalculationParameterGroup>()
            .FirstOrDefaultAsync(group => group.SettingsType == groupType);


        if (existingGroup == null) throw new ArgumentException("Группы указанного типа не существует");

        var existingParameter = await _context
            .Set<CalculationParameter>()
            .FirstOrDefaultAsync(param =>
                param.Name == parameter.Name && param.ParameterGroupId == parameter.ParameterGroupId);

        if (existingParameter == null) throw new ArgumentException("Указанный параметр не существует внутри группы");

        existingGroup.Parameters.Remove(existingParameter);
        await _context.SaveChangesAsync();
    }
}
