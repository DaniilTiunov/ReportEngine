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
        await DeleteByIdAsync(entity.Id);
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
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task UpdateAsync(ContainerBatch entity)
    {
        var existingEntity = await _context.Set<ContainerBatch>()
            .Include(b => b.Containers)
            .FirstOrDefaultAsync(b => b.Id == entity.Id);

        if (existingEntity == null)
            throw new Exception("Партия не найдена");


        _context.Entry(existingEntity).CurrentValues.SetValues(entity);

        var incomingContainers = entity.Containers.ToDictionary(c => c.Id);
        var existingContainers = existingEntity.Containers.ToDictionary(c => c.Id);

        foreach (var existingContainer in existingEntity.Containers.ToList())
            if (!incomingContainers.ContainsKey(existingContainer.Id))
                existingEntity.Containers.Remove(existingContainer);

        foreach (var container in entity.Containers)
            if (container.Id == 0)
            {
                // Новый
                container.ContainerBatchId = existingEntity.Id;
                existingEntity.Containers.Add(container);
            }
            else if (existingContainers.TryGetValue(container.Id, out var tracked))
            {
                _context.Entry(tracked).CurrentValues.SetValues(container);
            }

        await _context.SaveChangesAsync();
    }

    // Удаление партии + всех контейнеров в ней (без удаления стендов — только отвязка)
    public async Task<int> DeleteByIdAsync(int id)
    {
        // Транзакция для консистентности
        using var tx = await _context.Database.BeginTransactionAsync();

        var batch = await _context.ContainersBatch
            .Include(b => b.Containers)
            .ThenInclude(c => c.Stands)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (batch == null) return 0;

        // Для каждой упаковки: отвязать стенды и удалить упаковку
        foreach (var container in batch.Containers.ToList())
        {
            // Отвязать стенды — не удалять сами стенды
            var stands = await _context.Stands.Where(s => s.ContainerStandId == container.Id).ToListAsync();
            foreach (var s in stands)
            {
                s.ContainerStandId = null;
                _context.Stands.Update(s);
            }

            // Удаляем контейнер
            _context.ContainersStand.Remove(container);
        }

        // Удаляем саму партию
        _context.ContainersBatch.Remove(batch);

        await _context.SaveChangesAsync();
        await tx.CommitAsync();

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
            .ThenInclude(c => c.Stands)
            .ThenInclude(s => s.StandFrames)
            .ThenInclude(sf => sf.Frame)
            .Where(b => b.ProjectInfoId == projectId)
            .AsNoTracking()
            .ToListAsync();
    }


    public async Task<IEnumerable<ContainerBatch>> GetAllProjectBatchesInfoAsync(int projectId)
    {
        return await _context.ContainersBatch
            .Include(b => b.Containers)
            .ThenInclude(c => c.Stands)
            .Where(b => b.ProjectInfoId == projectId)
            .AsNoTracking()
            .ToListAsync();
    }


    public async Task UpdateContainerAsync(ContainerStand container)
    {
        _context.Update(container);
        await _context.SaveChangesAsync();
    }

    public async Task AddContainerToBatchAsync(int batchId, ContainerStand container)
    {
        var batch = await _context.ContainersBatch
            .Include(b => b.Containers)
            .FirstOrDefaultAsync(b => b.Id == batchId);

        if (batch == null)
            throw new ArgumentException($"Batch with ID {batchId} not found.");

        if (container.Id != 0)
        {
            if (batch.Containers.Any(c => c.Id == container.Id))
                return;

            var existing = await _context.ContainersStand
                .FirstOrDefaultAsync(c => c.Id == container.Id);

            if (existing == null)
            {
                container.ContainerBatchId = batchId;
                await _context.ContainersStand.AddAsync(container);
            }
            else
            {
                existing.ContainerBatchId = batchId;
            }
        }
        else
        {
            container.ContainerBatchId = batchId;
            await _context.ContainersStand.AddAsync(container);
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveContainerFromBatchAsync(int batchId, int containerId)
    {
        var batch = await _context.ContainersBatch
            .Include(b => b.Containers)
            .FirstOrDefaultAsync(b => b.Id == batchId);

        if (batch == null)
            throw new ArgumentException($"Batch with ID {batchId} not found.");

        var container = batch.Containers.FirstOrDefault(c => c.Id == containerId);
        if (container == null)
            return;

        var stands = await _context.Stands
            .Where(s => s.ContainerStandId == containerId)
            .ToListAsync();

        foreach (var s in stands)
            s.ContainerStandId = null;

        _context.ContainersStand.Remove(container);

        await _context.SaveChangesAsync();
    }

    public async Task AddStandToContainerAsync(int containerId, int standId)
    {
        var stand = await _context.Stands
            .FirstOrDefaultAsync(s => s.Id == standId);

        if (stand == null)
            throw new ArgumentException($"Stand with ID {standId} not found.");

        // защита от повторной привязки
        if (stand.ContainerStandId == containerId)
            return;

        // просто меняем FK
        stand.ContainerStandId = containerId;

        await _context.SaveChangesAsync();
    }

    public async Task RemoveStandFromContainerAsync(int containerId, int standId)
    {
        var stand = await _context.Stands
            .FirstOrDefaultAsync(s => s.Id == standId);

        if (stand == null)
            return;

        stand.ContainerStandId = null;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteContainerAsync(int containerId)
    {
        var container = await _context.ContainersStand
            .Include(c => c.Stands)
            .FirstOrDefaultAsync(c => c.Id == containerId);

        if (container == null) return;

        foreach (var s in container.Stands)
            s.ContainerStandId = null;

        _context.ContainersStand.Remove(container);

        await _context.SaveChangesAsync();
    }
}
