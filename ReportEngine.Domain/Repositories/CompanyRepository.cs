using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.Domain.Repositories;

public class CompanyRepository : IBaseRepository<Company>
{
    private readonly ReAppContext _context;

    public CompanyRepository(ReAppContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Company company)
    {
        await _context.Set<Company>().AddAsync(company);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Company>> GetAllAsync()
    {
        return await _context.Set<Company>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Company?> GetByIdAsync(int id) // Не используется
    {
        return await _context.Set<Company>()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task UpdateAsync(Company company)
    {
        var existingEntity = await _context.Set<Company>()
            .FirstOrDefaultAsync(c => c.Id == company.Id);

        if (existingEntity != null) _context.Entry(existingEntity).CurrentValues.SetValues(company);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Company comapny)
    {
        if (comapny == null) return;

        var existingEntity = await _context.Set<Company>()
            .FirstOrDefaultAsync(c => c.Id == comapny.Id);

        if (existingEntity != null)
        {
            _context.Set<Company>().Remove(existingEntity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> DeleteByIdAsync(int id) // Не используется
    {
        var entity = await _context.Set<Company>().FindAsync(id);

        if (entity == null)
            return 0;

        _context.Set<Company>().Remove(entity);

        await _context.SaveChangesAsync();

        return 1;
    }
}