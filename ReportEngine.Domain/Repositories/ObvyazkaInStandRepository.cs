using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;

namespace ReportEngine.Domain.Repositories;

public class ObvyazkaInStandRepository
{
    private readonly ReAppContext _context;

    public ObvyazkaInStandRepository(ReAppContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ObvyazkaInStand>> GetObvyazkiFromStandAsync(int standId)
    {
        return await _context.Set<ObvyazkaInStand>()
            .AsNoTracking()
            .Where(p => p.StandId == standId)
            .ToListAsync();
    }

    public async Task DeleteObvyazkaPurposesAsync(int id)
    {
        var entity = await _context.ObvyazkaAdditionalEquipPurpose
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return;

        _context.ObvyazkaAdditionalEquipPurpose.Remove(entity);

        await _context.SaveChangesAsync();
    }


    public async Task UpdateObvyazkaPurposesAsync(ObvyazkaAdditionalEquipPurpose obvyazka, int obvyazkaInStandId)
    {
        if (obvyazka == null)
            return;

        if (obvyazka.Id == 0)
        {
            obvyazka.ObvyazkaInStandId = obvyazkaInStandId;
            await _context.ObvyazkaAdditionalEquipPurpose.AddAsync(obvyazka);
        }
        else
        {
            obvyazka.ObvyazkaInStandId = obvyazkaInStandId;
            _context.ObvyazkaAdditionalEquipPurpose.Update(obvyazka);
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateFromSourceObvyazkaAsync(ObvyazkaInStand sourceObv, ObvyazkaInStand targetObv)
    {
        var existing = await _context.ObvyazkaAdditionalEquipPurpose
            .Where(x => x.ObvyazkaInStandId == targetObv.Id)
            .ToListAsync();

        _context.ObvyazkaAdditionalEquipPurpose.RemoveRange(existing);

        var newComponents = (sourceObv.AdditionalComponents ?? Enumerable.Empty<ObvyazkaAdditionalEquipPurpose>())
            .Select(sourceItem => new ObvyazkaAdditionalEquipPurpose
            {
                ObvyazkaInStandId = targetObv.Id,
                Purpose = sourceItem.Purpose,
                ExportDays = sourceItem.ExportDays,
                Material = sourceItem.Material,
                Quantity = sourceItem.Quantity,
                CostPerUnit = sourceItem.CostPerUnit,
                Measure = sourceItem.Measure
            })
            .ToList();

        if (newComponents.Count > 0)
            await _context.ObvyazkaAdditionalEquipPurpose.AddRangeAsync(newComponents);

        await _context.SaveChangesAsync();
    }
}
