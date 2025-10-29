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

    public async Task AddContainerToBatchAsync(int batchId, ContainerStand container)
    {
        var batch = await _context.ContainersBatch
            .Include(b => b.Containers)
            .ThenInclude(c => c.Stands)
            .FirstOrDefaultAsync(b => b.Id == batchId);

        if (batch == null)
            throw new ArgumentException($"Batch with ID {batchId} not found.");

        if (container.Id != 0)
        {
            // Если партия уже содержит контейнер с таким Id — ничего не делаем.
            if (batch.Containers.Any(c => c.Id == container.Id))
                return;

            var existing = await _context.ContainersStand
                .Include(c => c.Stands)
                .FirstOrDefaultAsync(c => c.Id == container.Id);

            if (existing == null)
            {
                // Если в БД нет такого контейнера, создаём новый как раньше (редкий кейс).
                container.ContainerBatchId = batchId;
                if (container.ProjectInfoId == 0)
                    container.ProjectInfoId = batch.ProjectInfoId;
                await _context.ContainersStand.AddAsync(container);
                batch.Containers.Add(container);
            }
            else
            {
                // Привязываем существующий контейнер к партии
                existing.ContainerBatchId = batchId;
                if (!batch.Containers.Any(c => c.Id == existing.Id))
                    batch.Containers.Add(existing);
                _context.ContainersStand.Update(existing);
            }
        }
        else
        {
            // Новый контейнер
            container.ContainerBatchId = batchId;
            if (container.ProjectInfoId == 0)
                container.ProjectInfoId = batch.ProjectInfoId;
            await _context.ContainersStand.AddAsync(container);
            batch.Containers.Add(container);
        }

        // Обновляем счётчики партии
        batch.ContainersCount = batch.Containers.Count;
        batch.StandsCount = batch.Containers.Sum(c => c.StandsCount);

        await _context.SaveChangesAsync();
    }

    public async Task RemoveContainerFromBatchAsync(int batchId, int containerId)
    {
        var batch = await _context.ContainersBatch
            .Include(b => b.Containers)
            .ThenInclude(c => c.Stands)
            .FirstOrDefaultAsync(b => b.Id == batchId);

        if (batch == null)
            throw new ArgumentException($"Batch with ID {batchId} not found.");

        var container = batch.Containers.FirstOrDefault(c => c.Id == containerId);
        if (container == null)
            return;

        // отвязываем стенды внутри контейнера (обнуляем их ContainerStandId)
        var stands = await _context.Stands.Where(s => s.ContainerStandId == containerId).ToListAsync();
        foreach (var s in stands)
        {
            s.ContainerStandId = null;
            _context.Stands.Update(s);
        }

        // удаляем контейнер
        _context.ContainersStand.Remove(container);
        batch.Containers.Remove(container);

        // обновляем счётчики партии
        batch.ContainersCount = batch.Containers.Count;
        batch.StandsCount = batch.Containers.Sum(c => c.StandsCount);

        await _context.SaveChangesAsync();
    }

    public async Task AddStandToContainerAsync(int containerId, int standId)
    {
        var container = await _context.ContainersStand
            .Include(c => c.Stands)
            .FirstOrDefaultAsync(c => c.Id == containerId);
        if (container == null) throw new ArgumentException($"Container with ID {containerId} not found.");

        var stand = await _context.Stands.FirstOrDefaultAsync(s => s.Id == standId);
        if (stand == null) throw new ArgumentException($"Stand with ID {standId} not found.");

        // Защита от двойной привязки
        if (stand.ContainerStandId == containerId)
            return;

        // Привязываем
        stand.ContainerStandId = containerId;
        _context.Stands.Update(stand);

        // Обновляем контейнерные данные
        if (!container.Stands.Any(s => s.Id == standId))
            container.Stands.Add(stand);

        container.StandsCount = container.Stands.Count;
        container.StandsWeight = container.Stands.Sum(s => s.Weight);

        // Обновляем родительскую партию (если есть)
        if (container.ContainerBatchId.HasValue)
        {
            var batch = await _context.ContainersBatch
                .Include(b => b.Containers)
                .ThenInclude(c => c.Stands)
                .FirstOrDefaultAsync(b => b.Id == container.ContainerBatchId.Value);

            if (batch != null)
            {
                batch.ContainersCount = batch.Containers.Count;
                batch.StandsCount = batch.Containers.Sum(c => c.StandsCount);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveStandFromContainerAsync(int containerId, int standId)
    {
        var container = await _context.ContainersStand
            .Include(c => c.Stands)
            .FirstOrDefaultAsync(c => c.Id == containerId);
        if (container == null) throw new ArgumentException($"Container with ID {containerId} not found.");

        var stand = container.Stands.FirstOrDefault(s => s.Id == standId);
        if (stand == null) return;

        // Отвязка стенда
        stand.ContainerStandId = null;
        _context.Stands.Update(stand);

        container.Stands.Remove(stand);
        container.StandsCount = container.Stands.Count;
        container.StandsWeight = container.Stands.Sum(s => s.Weight);

        // Обновляем родительскую партию (если есть)
        if (container.ContainerBatchId.HasValue)
        {
            var batch = await _context.ContainersBatch
                .Include(b => b.Containers)
                .ThenInclude(c => c.Stands)
                .FirstOrDefaultAsync(b => b.Id == container.ContainerBatchId.Value);

            if (batch != null)
            {
                batch.ContainersCount = batch.Containers.Count;
                batch.StandsCount = batch.Containers.Sum(c => c.StandsCount);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteContainerAsync(int containerId)
    {
        var container = await _context.ContainersStand
            .Include(c => c.Stands)
            .FirstOrDefaultAsync(c => c.Id == containerId);
        if (container == null) return;

        // Отвязать стенды
        var stands = container.Stands.ToList();
        foreach (var s in stands)
        {
            s.ContainerStandId = null;
            _context.Stands.Update(s);
        }

        // Если контейнер был в партии, обновим партию
        if (container.ContainerBatchId.HasValue)
        {
            var batch = await _context.ContainersBatch
                .Include(b => b.Containers)
                .ThenInclude(c => c.Stands)
                .FirstOrDefaultAsync(b => b.Id == container.ContainerBatchId.Value);

            if (batch != null)
            {
                batch.Containers.Remove(container);
                batch.ContainersCount = batch.Containers.Count;
                batch.StandsCount = batch.Containers.Sum(c => c.StandsCount);
            }
        }

        _context.ContainersStand.Remove(container);
        await _context.SaveChangesAsync();
    }
}
