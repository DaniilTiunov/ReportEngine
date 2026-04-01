using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities.CalculationParameters;
using ReportEngine.Domain.Entities.CalculationParameters.Enums;

namespace ReportEngine.Domain.Repositories;

public class CalculationRepository
{
    private readonly ReAppContext _context;

    public CalculationRepository(ReAppContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CalculationParameterGroup>> GetAllGroupsAsync()
    {
        return await _context.Set<CalculationParameterGroup>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<CalculationParameterGroup?> GetGroupByTypeAsync(CalculationParameterType type)
    {
        return await _context.Set<CalculationParameterGroup>()
           .AsNoTracking()
           .Include(group => group.Parameters)
           .FirstOrDefaultAsync(group => group.SettingsType == type);
    }


    public async Task<CalculationParameterGroup> AddGroupAsync(CalculationParameterGroup group)
    {
        var newEntity = new CalculationParameterGroup()
        {
            Id = 0,
            Name = group.Name,
            SettingsType = group.SettingsType,
        };

        var result = await _context.Set<CalculationParameterGroup>().AddAsync(newEntity);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task UpdateGroupAsync(CalculationParameterGroup group)
    {
        var existingGroup = await _context
              .Set<CalculationParameterGroup>()
              .AsNoTracking()
              .FirstOrDefaultAsync(gr => gr.SettingsType == group.SettingsType);


        if (existingGroup == null)
        {
            throw new ArgumentException("Группы указанного типа не существует");
        }

        _context.Set<CalculationParameterGroup>().Update(existingGroup);
        await _context.SaveChangesAsync();

    }

    public async Task DeleteGroupAsync(CalculationParameterGroup group)
    {
        var existingGroup = await _context
              .Set<CalculationParameterGroup>()
              .AsNoTracking()
              .FirstOrDefaultAsync(gr => gr.SettingsType == group.SettingsType);


        if (existingGroup == null)
        {
            throw new ArgumentException("Группы указанного типа не существует");
        }

        _context.Set<CalculationParameterGroup>().Remove(existingGroup);
        await _context.SaveChangesAsync();

    }


    public async Task AddParameterToGroup(CalculationParameter parameter, CalculationParameterType groupType)
    {
        var existingGroup = await _context.Set<CalculationParameterGroup>()
            .FirstOrDefaultAsync(group => group.SettingsType == groupType);


        if (existingGroup == null)
        {
            throw new ArgumentException("Группы указанного типа не существует");
        }

        parameter.ParameterGroupId = existingGroup.Id;
        existingGroup.Parameters.Add(parameter);

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
            }
        }

        await _context.SaveChangesAsync();
    }


    public async Task DeleteParameterFromGroup(CalculationParameter parameter, CalculationParameterType groupType)
    {
        var existingGroup = await _context
                   .Set<CalculationParameterGroup>()
                   .FirstOrDefaultAsync(group => group.SettingsType == groupType);


        if (existingGroup == null)
        {
            throw new ArgumentException("Группы указанного типа не существует");
        }

        var existingParameter = await _context
            .Set<CalculationParameter>()
            .FirstOrDefaultAsync(param => param.Name == parameter.Name && param.ParameterGroupId == parameter.ParameterGroupId);

        if (existingParameter == null)
        {
            throw new ArgumentException("Указанный параметр не существует внутри группы");
        }

        existingGroup.Parameters.Remove(existingParameter);
        await _context.SaveChangesAsync();
    }

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
}

