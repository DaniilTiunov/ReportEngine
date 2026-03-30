using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;

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

    public async Task AddNewParameterAsync(CalculationParameter parameter)
    {
        await _context.Set<CalculationParameter>().AddAsync(parameter);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatedParameterAsync(CalculationParameter parameter)
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
