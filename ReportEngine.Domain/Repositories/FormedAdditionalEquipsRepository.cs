using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.Domain.Repositories;

public class FormedAdditionalEquipsRepository : IFormedAdditionalEquipsRepository
{
    private readonly ReAppContext _context;

    public FormedAdditionalEquipsRepository(ReAppContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FormedAdditionalEquip>> GetAllAsync()
    {
        return await _context.FormedAdditionalEquips
            .Include(a => a.Purposes)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(FormedAdditionalEquip entity)
    {
        await _context.FormedAdditionalEquips.AddAsync(entity);
        await _context.SaveChangesAsync();
    }
}