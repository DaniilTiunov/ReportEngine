using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.Domain.Repositories;

public class FormedDrainagesRepository : IFormedDrainagesRepository
{
    private readonly ReAppContext _context;

    public FormedDrainagesRepository(ReAppContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FormedDrainage>> GetAllAsync()
    {
        return await _context.FormedDrainages.AsNoTracking().ToListAsync();
    }

    public async Task<FormedDrainage> GetByIdAsync(int id)
    {
        return await _context.FormedDrainages.FindAsync(id);
    }

    public async Task AddAsync(FormedDrainage entity)
    {
        await _context.FormedDrainages.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(FormedDrainage entity)
    {
        _context.FormedDrainages.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(FormedDrainage entity)
    {
        _context.FormedDrainages.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<int> DeleteByIdAsync(int id)
    {
        var entity = await _context.FormedDrainages.FindAsync(id);
        if (entity == null) return 0;
        _context.FormedDrainages.Remove(entity);
        await _context.SaveChangesAsync();
        return 1;
    }

    public async Task<IEnumerable<FormedDrainage>> GetAllWithPurposesAsync()
    {
        return await _context.FormedDrainages
            .Include(fd => fd.Purposes)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<FormedDrainage> GetByIdWithPurposesAsync(int id)
    {
        return await _context.FormedDrainages
            .Include(fd => fd.Purposes)
            .FirstOrDefaultAsync(fd => fd.Id == id);
    }

    public async Task UpdateAsync(DrainagePurpose purpose)
    {
        if (purpose == null) return;

        if (purpose.Id == 0)
        {
            if (purpose.FormedDrainageId == 0)
                throw new ArgumentException("Для добавления новой цели необходим FormedDrainageId");

            await _context.DrainagePurposes.AddAsync(purpose);
            await _context.SaveChangesAsync();
            return;
        }

        var existing = await _context.DrainagePurposes.FindAsync(purpose.Id);
        if (existing == null) return;

        existing.Purpose = purpose.Purpose;
        existing.Material = purpose.Material;
        existing.Quantity = purpose.Quantity;
        existing.CostPerUnit = purpose.CostPerUnit;
        existing.Measure = purpose.Measure;

        _context.DrainagePurposes.Update(existing);
        await _context.SaveChangesAsync();
    }


    public async Task DeletePurposeAsync(int purposeId)
    {
        var entity = await _context.DrainagePurposes.FindAsync(purposeId);
        if (entity == null) return;
        _context.DrainagePurposes.Remove(entity);
        await _context.SaveChangesAsync();
    }
}