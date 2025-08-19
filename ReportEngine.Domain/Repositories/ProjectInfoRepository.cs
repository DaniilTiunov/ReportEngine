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

    public async Task<IEnumerable<ProjectInfo>> GetAllAsync()
    {
        return await _context.Set<ProjectInfo>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ProjectInfo> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
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

    public async Task<int> DeleteByIdAsync(int id)
    {
        var entityProjectInfo = await _context.Set<ProjectInfo>().FindAsync(id);
        if (entityProjectInfo == null)
            return 0;

        _context.Set<ProjectInfo>().Remove(entityProjectInfo);
        await _context.SaveChangesAsync();
        return 1;
    }

    public async Task<Stand> AddStandAsync(int projectId, Stand stand)
    {
        var project = await _context.Projects
            .Include(p => p.Stands)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null) throw new ArgumentException($"Проект с ID: {projectId} не найден.");

        stand.ProjectInfoId = projectId;
        project.Stands.Add(stand);

        await _context.SaveChangesAsync();

        return stand;
    }

    public async Task UpdateStandAsync(Stand stand)
    {
        var existingStand = await _context.Set<Stand>()
            .FirstOrDefaultAsync(p => p.Id == stand.Id);

        if (existingStand != null) _context.Entry(existingStand).CurrentValues.SetValues(stand);

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
        var stand = await _context.Stands.Include(s => s.ObvyazkiInStand).FirstOrDefaultAsync(s => s.Id == standId);
        if (stand == null) throw new ArgumentException($"Стенд с ID: {standId} не найден.");

        stand.ObvyazkiInStand.Add(standObvyazka);
        await _context.SaveChangesAsync();
    }

    // Связь через StandFrame
    public async Task AddFrameToStandAsync(int standId, int frameId)
    {
        var stand = await _context.Stands.FirstOrDefaultAsync(s => s.Id == standId);
        var frame = await _context.FormedFrames.FirstOrDefaultAsync(f => f.Id == frameId);

        if (stand == null) throw new ArgumentException($"Стенд с ID: {standId} не найден.");
        if (frame == null) throw new ArgumentException($"Рама с ID: {frameId} не найдена.");

        var standFrame = new StandFrame
        {
            StandId = standId,
            FrameId = frameId,
            Stand = stand,
            Frame = frame
        };

        await _context.StandFrames.AddAsync(standFrame);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<StandFrame>> GetAllFramesInStandAsync(int standId)
    {
        return await _context.StandFrames
            .Include(sf => sf.Frame)
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
            .Where(sd => sd.StandId == standId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ObvyazkaInStand>> GetAllObvyazkiInStandAsync(int standId)
    {
        var stand = await _context.Stands
            .Include(s => s.ObvyazkiInStand)
            .FirstOrDefaultAsync(s => s.Id == standId);

        return stand?.ObvyazkiInStand ?? Enumerable.Empty<ObvyazkaInStand>();
    }

    public async Task<IEnumerable<StandElectricalComponent>> GetAllElectricalComponentsInStandAsync(int standId)
    {
        return await _context.StandElectricalComponents
            .Include(sec => sec.ElectricalComponent)
            .Where(sec => sec.StandId == standId)
            .ToListAsync();
    }

    public async Task<IEnumerable<StandAdditionalEquip>> GetAllAdditionalEquipsInStandAsync(int standId)
    {
        return await _context.StandAdditionalEquips
            .Include(sae => sae.AdditionalEquip)
            .Where(sae => sae.StandId == standId)
            .ToListAsync();
    }
}