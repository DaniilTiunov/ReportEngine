using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.Domain.Repositories;

public class ContainerRepository : IContainerRepository
{
    private readonly ReAppContext _context;

    public ContainerRepository(ReAppContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ContainerBatch entity)
    {
        await _context.Set<ContainerBatch>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ContainerBatch entity)
    {
        if (entity == null) return;
        var existing = await _context.Set<ContainerBatch>()
            .Include(b => b.Containers)
            .FirstOrDefaultAsync(b => b.Id == entity.Id);

        if (existing != null)
        {
            // Удаляем связанные контейнеры только если это ожидаемо — здесь просто удаляем партию
            _context.Set<ContainerBatch>().Remove(existing);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ContainerBatch>> GetAllAsync()
    {
        return await _context.Set<ContainerBatch>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ContainerBatch> GetByIdAsync(int id)
    {
        return await _context.Set<ContainerBatch>()
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task UpdateAsync(ContainerBatch entity)
    {
        var existing = await _context.Set<ContainerBatch>()
            .FirstOrDefaultAsync(b => b.Id == entity.Id);

        if (existing != null)
            _context.Entry(existing).CurrentValues.SetValues(entity);

        await _context.SaveChangesAsync();
    }

    public async Task<int> DeleteByIdAsync(int id)
    {
        var entity = await _context.Set<ContainerBatch>().FindAsync(id);
        if (entity == null) return 0;
        _context.Set<ContainerBatch>().Remove(entity);
        await _context.SaveChangesAsync();
        return 1;
    }

    public async Task<ContainerBatch> GetByIdWithContainersAsync(int id)
    {
        return await _context.ContainersBatch
            .Include(b => b.Containers)
                .ThenInclude(c => c.Stands)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<ContainerBatch>> GetAllByProjectIdAsync(int projectId)
    {
        return await _context.ContainersBatch
            .Include(b => b.Containers)
            .Where(b => b.ProjectInfoId == projectId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddContainerToBatchAsync(int batchId, ContainerStand container)
    {
        var batch = await _context.ContainersBatch
            .Include(b => b.Containers)
            .FirstOrDefaultAsync(b => b.Id == batchId);

        if (batch == null)
            throw new ArgumentException($"Batch with ID {batchId} not found.");

        // Если контейнер уже отсутсвует в контексте — добавляем
        await _context.ContainersStand.AddAsync(container);
        batch.Containers.Add(container);

        await _context.SaveChangesAsync();
    }
}
