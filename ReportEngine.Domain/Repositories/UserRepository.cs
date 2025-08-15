using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.Domain.Repositories;

public class UserRepository : IBaseRepository<User>
{
    private readonly ReAppContext _context;

    public UserRepository(ReAppContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User user)
    {
        await _context.Set<User>().AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Set<User>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id) // Не используется
    {
        return await _context.Set<User>()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task UpdateAsync(User user)
    {
        var existingEntity = await _context.Set<User>()
            .FirstOrDefaultAsync(c => c.Id == user.Id);

        if (existingEntity != null) _context.Entry(existingEntity).CurrentValues.SetValues(user);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        if (user == null) return;

        var existingEntity = await _context.Set<User>()
            .FirstOrDefaultAsync(c => c.Id == user.Id);

        if (existingEntity != null)
        {
            _context.Set<User>().Remove(existingEntity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> DeleteByIdAsync(int id) // Не используется
    {
        var entity = await _context.Set<User>().FindAsync(id);

        if (entity == null)
            return 0;

        _context.Set<User>().Remove(entity);

        await _context.SaveChangesAsync();

        return 1;
    }
}