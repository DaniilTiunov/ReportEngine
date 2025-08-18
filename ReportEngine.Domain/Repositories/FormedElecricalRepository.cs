using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.Domain.Repositories;

public class FormedElectricalRepository : IFormedElectricalRepository
{
    private readonly ReAppContext _context;

    public FormedElectricalRepository(ReAppContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FormedElectricalComponent>> GetAllAsync()
    {
        return await _context.FormedElectricalComponents
            .Include(e => e.Purposes)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(FormedElectricalComponent entity)
    {
        await _context.FormedElectricalComponents.AddAsync(entity);
        await _context.SaveChangesAsync();
    }
}