using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities.CalculationParameters;

namespace ReportEngine.Domain.Repositories;

public class CalculationRepository
{
    private readonly ReAppContext _context;

    public CalculationRepository(ReAppContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CalculationParameter>> GetAllParametersAsync()
    {
        return await _context.Set<CalculationParameter>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<CalculationParameterGroup>> GetAllGroupsAsync()
    {
        return await _context.Set<CalculationParameterGroup>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<CalculationParameterGroup> GetGroupByTypeAsync(CalculationParameterType parametersType)
    {
        return await _context.Set<CalculationParameterGroup>()
            .AsNoTracking()
            .Include(param => param.Parameters)
            .FirstOrDefaultAsync(group => group.SettingsType == parametersType);
    }

    public async Task<List<CalculationParameter>> GetGroupParametersAsync(CalculationParameterType type)
    {
        return await _context.Set<CalculationParameterGroup>()
            .AsNoTracking()
            .Where(group => group.SettingsType == type)
            .Include(group => group.Parameters)
            .SelectMany(param => param.Parameters)
            .ToListAsync();
    }


    public async Task AddNewParameterAsync(CalculationParameter parameter)
    {
        await _context.Set<CalculationParameter>().AddAsync(parameter);
        await _context.SaveChangesAsync();
    }

    public async Task<CalculationParameterGroup> AddNewGroupAsync(CalculationParameterGroup group)
    {
        var result = await _context.Set<CalculationParameterGroup>().AddAsync(group);
        await _context.SaveChangesAsync();
        return result.Entity;
    }


    public async Task AddParameterToGroup(CalculationParameter parameter, CalculationParameterGroup group)
    {
        var existingParameter = await _context.Set<CalculationParameter>()
            .AsNoTracking()
            .FirstOrDefaultAsync(par => par.Name == parameter.Name);

        var existingGroup = await _context.Set<CalculationParameterGroup>()
            .AsNoTracking()
            .FirstOrDefaultAsync(gr => gr.SettingsType == group.SettingsType);


        //если такой группы нет - добавляем
        if (existingGroup == null) existingGroup = await AddNewGroupAsync(group);

        //связываем с группой
        parameter.ParameterGroupId = existingGroup.Id;
        parameter.CalculationParameterGroup = existingGroup;

        //если такой параметр есть - обновляем
        if (existingParameter != null)
            await UpdateParameterAsync(parameter);
        //если нет - добавляем новый
        else
            await AddNewParameterAsync(parameter);

        await _context.SaveChangesAsync();
    }


    public async Task UpdateParameterAsync(CalculationParameter parameter)
    {
        _context.Set<CalculationParameter>().Update(parameter);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteParameterAsync(CalculationParameter parameter)
    {
        _context.Set<CalculationParameter>().Remove(parameter);
        await _context.SaveChangesAsync();
    }
}
