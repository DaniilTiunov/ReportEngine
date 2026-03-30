using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.Domain.Repositories;

public class ProjectInfoRepository : IProjectInfoRepository
{
    private readonly ReAppContext _context;

    public ProjectInfoRepository(ReAppContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ProjectInfo entity)
    {
        await _context.Set<ProjectInfo>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Stand>> GetProjectWithStandsAsync(int projectId)
    {
        return await _context.Set<ProjectInfo>()
            .AsNoTracking()
            .Where(p => p.Id == projectId)
            .SelectMany(p => p.Stands)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjectInfo>> GetAllAsync()
    {
        return await _context.Set<ProjectInfo>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjectInfo>> GetAllWithSandsAsync()
    {
        return await _context.Set<ProjectInfo>()
            .Include(p => p.Stands)
            .ThenInclude(s => s.StandAdditionalEquips)
            .ThenInclude(sae => sae.AdditionalEquip.Purposes)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ProjectInfo> GetByIdAsync(int id)
    {
        return await _context.Set<ProjectInfo>()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task UpdateAsync(ProjectInfo project)
    {
        var existingProject = await _context.Set<ProjectInfo>()
            .Include(p => p.Stands)
            .FirstOrDefaultAsync(p => p.Id == project.Id);

        if (existingProject != null) _context.Entry(existingProject).CurrentValues.SetValues(project);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ProjectInfo project)
    {
        if (project == null) return;

        var existingProject = await _context.Set<ProjectInfo>()
            .FirstOrDefaultAsync(p => p.Id == project.Id);

        if (existingProject != null)
        {
            _context.Set<ProjectInfo>().Remove(existingProject);
            await _context.SaveChangesAsync();
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteStandAsync(int projectId, int standId)
    {
        if (standId == null)
            return;

        var existingStand = await _context.Set<Stand>()
            .FirstOrDefaultAsync(s => s.Id == standId);

        if (existingStand != null)
            _context.Set<Stand>().Remove(existingStand);

        var additionalFormedEquips = await _context.Set<FormedAdditionalEquip>()
            .Where(fe => _context.Set<StandAdditionalEquip>()
                .Any(sae => sae.StandId == standId && sae.AdditionalEquipId == fe.Id))
            .ToListAsync();

        var electricalFormedEquips = await _context.Set<FormedElectricalComponent>()
            .Where(fe => _context.Set<StandElectricalComponent>()
                .Any(sae => sae.StandId == standId && sae.ElectricalComponentId == fe.Id))
            .ToListAsync();

        var drainagesFormedEquips = await _context.Set<FormedDrainage>()
            .Where(fe => _context.Set<StandDrainage>()
                .Any(sae => sae.StandId == standId && sae.DrainageId == fe.Id))
            .ToListAsync();

        _context.RemoveRange(additionalFormedEquips);
        _context.RemoveRange(electricalFormedEquips);
        _context.RemoveRange(drainagesFormedEquips);

        await _context.SaveChangesAsync();
    }

    public async Task<int> DeleteByIdAsync(int id)
    {
        var entityProjectInfo = await _context.Set<ProjectInfo>().FindAsync(id);
        if (entityProjectInfo == null)
            return 0;

        _context.Set<ProjectInfo>().Remove(entityProjectInfo);
        await _context.SaveChangesAsync();
        return 1;
    }

    public async Task<IEnumerable<Stand>> AddStandsGroupAsync(int projectId, IEnumerable<Stand> stands)
    {
        var project = await _context.Projects
            .Include(p => p.Stands)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
            throw new ArgumentException($"Проект с ID: {projectId} не найден.");

        foreach (var stand in stands) stand.ProjectInfoId = projectId;

        _context.Stands.AddRange(stands);

        await _context.SaveChangesAsync();

        return stands;
    }

    public async Task<Stand> AddStandAsync(int projectId, Stand stand)
    {
        var project = await _context.Projects
            .Include(p => p.Stands)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
            throw new ArgumentException($"Проект с ID: {projectId} не найден.");

        stand.ProjectInfoId = projectId;
        project.Stands.Add(stand);

        await _context.SaveChangesAsync();

        return stand;
    }

    public async Task UpdateStandAsync(Stand stand)
    {
        var existingStand = await _context.Set<Stand>()
            .FirstOrDefaultAsync(p => p.Id == stand.Id);

        if (existingStand != null)
            _context.Entry(existingStand).CurrentValues.SetValues(stand);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateStandsGroupAsync(IEnumerable<Stand> stands)
    {
        var allStandsId = stands.Select(stand => stand.Id).ToList();

        var existingStands = await _context.Set<Stand>()
            .Where(stand => allStandsId.Contains(stand.Id))
            .ToDictionaryAsync(stand => stand.Id);

        foreach (var stand in stands)
            if (existingStands.TryGetValue(stand.Id, out var existingStand))
                _context.Entry(existingStand).CurrentValues.SetValues(stand);

        await _context.SaveChangesAsync();
    }

    public async Task<ProjectInfo> GetStandsByIdAsync(int projectId)
    {
        return await _context.Projects
            .Include(p => p.Stands)
            .FirstOrDefaultAsync(p => p.Id == projectId);
    }

    public async Task AddStandObvyazkaAsync(int standId, ObvyazkaInStand standObvyazka)
    {
        var stand = await _context.Stands.Include(s => s.ObvyazkiInStand)
            .FirstOrDefaultAsync(s => s.Id == standId);

        if (stand == null)
            throw new ArgumentException($"Стенд с ID: {standId} не найден.");

        // Проверяем, существует ли Obvyazka
        var obvyazkaExists = await _context.Obvyazki
            .AnyAsync(o => o.Id == standObvyazka.ObvyazkaId);

        if (!obvyazkaExists)
            throw new ArgumentException($"Обвязка с ID {standObvyazka.ObvyazkaId} не найдена.");

        standObvyazka.Id = 0;

        stand.ObvyazkiInStand.Add(standObvyazka);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteObvFromStandAsync(int standId, int obvyazkaInStandId)
    {
        var entity = await _context.Set<ObvyazkaInStand>()
            .FirstOrDefaultAsync(o => o.StandId == standId && o.Id == obvyazkaInStandId);

        if (entity == null)
            return;

        _context.Set<ObvyazkaInStand>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateObvInStandAsync(int standId, ObvyazkaInStand standObvyazka)
    {
        var existingObvyazka = await _context.Set<ObvyazkaInStand>()
            .FirstOrDefaultAsync(obv => obv.Id == standObvyazka.Id && obv.StandId == standId);

        if (existingObvyazka != null)
        {
            // Обновляем значения
            _context.Entry(existingObvyazka).CurrentValues.SetValues(standObvyazka);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteFrameFromStandAsync(int frameInStandId)
    {
        var entity = await _context.Set<StandFrame>()
            .FirstOrDefaultAsync(o => o.Id == frameInStandId);

        if (entity == null)
            return;

        _context.Set<StandFrame>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    // Связь через StandFrame
    public async Task AddFrameToStandAsync(int standId, int frameId)
    {
        var stand = await _context.Stands.FirstOrDefaultAsync(s => s.Id == standId);
        var frame = await _context.FormedFrames.FirstOrDefaultAsync(f => f.Id == frameId);

        if (stand == null)
            throw new ArgumentException($"Стенд с ID: {standId} не найден.");

        if (frame == null)
            throw new ArgumentException($"Рама с ID: {frameId} не найдена.");

        var standFrame = new StandFrame
        {
            StandId = standId,
            FrameId = frameId,
            Stand = stand,
            Frame = frame
        };

        await _context.StandFrames.AddAsync(standFrame);
        var result = await _context.SaveChangesAsync();

        if (result == 0)
            throw new Exception("StandFrame не был добавлен в базу данных.");
    }

    public async Task<IEnumerable<StandFrame>> GetAllFramesInStandAsync(int standId)
    {
        return await _context.StandFrames
            .Include(sf => sf.Frame)
            .ThenInclude(fc => fc.Components)
            .Where(sf => sf.StandId == standId)
            .ToListAsync();
    }

    // Связь через StandDrainage
    public async Task AddDrainageToStandAsync(int standId, int drainageId)
    {
        var stand = await _context.Stands.FirstOrDefaultAsync(s => s.Id == standId);
        var drainage = await _context.Set<FormedDrainage>().FirstOrDefaultAsync(d => d.Id == drainageId);

        if (stand == null) throw new ArgumentException($"Стенд с ID: {standId} не найден.");
        if (drainage == null) throw new ArgumentException($"Дренаж с ID: {drainageId} не найден.");

        var standDrainage = new StandDrainage
        {
            StandId = standId,
            DrainageId = drainageId,
            Stand = stand,
            Drainage = drainage
        };

        await _context.Set<StandDrainage>().AddAsync(standDrainage);
        await _context.SaveChangesAsync();
    }

    public async Task AddElectricalComponentToStandAsync(int standId, int electricalComponentId)
    {
        var stand = await _context.Stands.FirstOrDefaultAsync(s => s.Id == standId);
        var electricalComponent =
            await _context.FormedElectricalComponents.FirstOrDefaultAsync(e => e.Id == electricalComponentId);

        if (stand == null) throw new ArgumentException($"Стенд с ID: {standId} не найден.");
        if (electricalComponent == null)
            throw new ArgumentException($"Электрокомпонент с ID: {electricalComponentId} не найден.");

        var standElectricalComponent = new StandElectricalComponent
        {
            StandId = standId,
            ElectricalComponentId = electricalComponentId,
            Stand = stand,
            ElectricalComponent = electricalComponent
        };

        await _context.StandElectricalComponents.AddAsync(standElectricalComponent);
        await _context.SaveChangesAsync();
    }

    public async Task AddAdditionalEquipToStandAsync(int standId, int additionalEquipId)
    {
        var stand = await _context.Stands.FirstOrDefaultAsync(s => s.Id == standId);
        var additionalEquip = await _context.FormedAdditionalEquips.FirstOrDefaultAsync(a => a.Id == additionalEquipId);

        if (stand == null) throw new ArgumentException($"Стенд с ID: {standId} не найден.");
        if (additionalEquip == null)
            throw new ArgumentException($"Комплектующее с ID: {additionalEquipId} не найдено.");

        var standAdditionalEquip = new StandAdditionalEquip
        {
            StandId = standId,
            AdditionalEquipId = additionalEquipId,
            Stand = stand,
            AdditionalEquip = additionalEquip
        };

        await _context.StandAdditionalEquips.AddAsync(standAdditionalEquip);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<StandDrainage>> GetAllDrainagesInStandAsync(int standId)
    {
        return await _context.Set<StandDrainage>()
            .Include(sd => sd.Drainage)
            .ThenInclude(d => d.Purposes)
            .Where(sd => sd.StandId == standId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ObvyazkaInStand>> GetAllObvyazkiInStandAsync(int standId)
    {
        var stand = await _context.Stands
            .Include(s => s.ObvyazkiInStand)
            .ThenInclude(obv => obv.AdditionalComponents)
            .FirstOrDefaultAsync(s => s.Id == standId);

        return stand?.ObvyazkiInStand ?? Enumerable.Empty<ObvyazkaInStand>();
    }

    public async Task<IEnumerable<StandElectricalComponent>> GetAllElectricalComponentsInStandAsync(int standId)
    {
        return await _context.StandElectricalComponents
            .Include(sec => sec.ElectricalComponent)
            .ThenInclude(e => e.Purposes)
            .Where(sec => sec.StandId == standId)
            .ToListAsync();
    }

    public async Task<IEnumerable<StandAdditionalEquip>> GetAllAdditionalEquipsInStandAsync(int standId)
    {
        return await _context.StandAdditionalEquips
            .Include(sae => sae.AdditionalEquip)
            .ThenInclude(e => e.Purposes)
            .Where(sae => sae.StandId == standId)
            .ToListAsync();
    }
}
