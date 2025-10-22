using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.Domain.Repositories;

public class SubjectsRepository : IBaseRepository<Subject>
{
    private readonly ReAppContext _context;

    public SubjectsRepository(ReAppContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Subject subject)
    {
        await _context.Set<Subject>().AddAsync(subject);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Subject>> GetAllAsync()
    {
        return await _context.Set<Subject>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Subject?> GetByIdAsync(int id) // Не используется
    {
        return await _context.Set<Subject>()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task UpdateAsync(Subject subject)
    {
        var existingEntity = await _context.Set<Subject>()
            .FirstOrDefaultAsync(s => s.Id == subject.Id);

        if (existingEntity != null) _context.Entry(existingEntity).CurrentValues.SetValues(subject);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Subject subject)
    {
        if (subject == null) return;

        var existingEntity = await _context.Set<Subject>()
            .FirstOrDefaultAsync(s => s.Id == subject.Id);

        if (existingEntity != null)
        {
            _context.Set<Subject>().Remove(existingEntity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> DeleteByIdAsync(int id) // Не используется
    {
        var entity = await _context.Set<Subject>().FindAsync(id);

        if (entity == null)
            return 0;

        _context.Set<Subject>().Remove(entity);

        await _context.SaveChangesAsync();

        return 1;
    }
}