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

    public async Task<IEnumerable<FormedElectricalComponent>> GetAllWithPurposesAsync()
    {
        return await _context.FormedElectricalComponents
            .Include(ec => ec.Purposes)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<FormedElectricalComponent> GetByIdWithPurposesAsync(int id)
    {
        return await _context.FormedElectricalComponents
            .Include(ec => ec.Purposes)
            .FirstOrDefaultAsync(ec => ec.Id == id);
    }

    public async Task UpdateAsync(ElectricalPurpose purpose)
    {
        if (purpose == null) return;

        if (purpose.Id == 0)
        {
            if (purpose.FormedElectricalComponentId == 0)
                throw new ArgumentException("Для добавления новой цели необходим FormedElectricalComponentId");

            await _context.ElectricalPurposes.AddAsync(purpose);
            await _context.SaveChangesAsync();
            return;
        }

        var existing = await _context.ElectricalPurposes.FindAsync(purpose.Id);
        if (existing == null) return;

        existing.Purpose = purpose.Purpose;
        existing.Material = purpose.Material;
        existing.Quantity = purpose.Quantity;
        existing.CostPerUnit = purpose.CostPerUnit;
        existing.Measure = purpose.Measure;

        _context.ElectricalPurposes.Update(existing);
        await _context.SaveChangesAsync();
    }


    public async Task DeletePurposeAsync(int purposeId)
    {
        var entity = await _context.Set<ElectricalPurpose>().FindAsync(purposeId);
        if (entity == null) return;
        _context.Set<ElectricalPurpose>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}