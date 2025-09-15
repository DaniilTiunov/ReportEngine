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

    public async Task<IEnumerable<FormedAdditionalEquip>> GetAllWithPurposesAsync()
    {
        return await _context.FormedAdditionalEquips
            .Include(ae => ae.Purposes)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<FormedAdditionalEquip> GetByIdWithPurposesAsync(int id)
    {
        return await _context.FormedAdditionalEquips
            .Include(ae => ae.Purposes)
            .FirstOrDefaultAsync(ae => ae.Id == id);
    }

    public async Task UpdateAsync(AdditionalEquipPurpose purpose)
    {
        if (purpose == null) return;

        if (purpose.Id == 0)
        {
            if (purpose.FormedAdditionalEquipId == 0)
                throw new ArgumentException("Для добавления новой цели необходим FormedAdditionalEquipId");

            await _context.AdditionalEquipPurposes.AddAsync(purpose);
            await _context.SaveChangesAsync();
            return;
        }

        var existing = await _context.AdditionalEquipPurposes.FindAsync(purpose.Id);
        if (existing == null) return;

        existing.Purpose = purpose.Purpose;
        existing.Material = purpose.Material;
        existing.Quantity = purpose.Quantity;
        existing.CostPerUnit = purpose.CostPerUnit;
        existing.Measure = purpose.Measure;

        _context.AdditionalEquipPurposes.Update(existing);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePurposeAsync(int purposeId)
    {
        var entity = await _context.AdditionalEquipPurposes.FindAsync(purposeId);
        if (entity == null) return;
        _context.AdditionalEquipPurposes.Remove(entity);
        await _context.SaveChangesAsync();
    }
}