using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using System.Diagnostics;

namespace ReportEngine.Domain.Repositories
{
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

            if (existingProject != null)
            {
                _context.Entry(existingProject).CurrentValues.SetValues(project);
            }

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

            if (project == null)
            {
                throw new ArgumentException($"Проект с ID: {projectId} не найден.");
            }

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
            {
                _context.Entry(existingStand).CurrentValues.SetValues(stand);
            }

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

        public async Task AddDrainageToStandAsync(int standId, FormedDrainage drainage)
        {
            var stand = await _context.Stands.Include(s => s.FormedDrainages).FirstOrDefaultAsync(s => s.Id == standId);
            if (stand == null) throw new ArgumentException($"Стенд с ID: {standId} не найден.");

            stand.FormedDrainages.Add(drainage);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<StandFrame>> GetAllFramesInStandAsync(int standId)
        {
            return await _context.StandFrames
                .Include(sf => sf.Frame)
                .Where(sf => sf.StandId == standId)
                .ToListAsync();
        }

        public async Task<IEnumerable<FormedDrainage>> GetAllDrainagesInStandAsync(int standId)
        {
            var stand = await _context.Stands
                .Include(s => s.FormedDrainages)
                .FirstOrDefaultAsync(s => s.Id == standId);

            return stand?.FormedDrainages ?? Enumerable.Empty<FormedDrainage>();
        }

        public async Task<IEnumerable<ObvyazkaInStand>> GetAllObvyazkiInStandAsync(int standId)
        {
            var stand = await _context.Stands
                .Include(s => s.ObvyazkiInStand)
                .FirstOrDefaultAsync(s => s.Id == standId);

            return stand?.ObvyazkiInStand ?? Enumerable.Empty<ObvyazkaInStand>();
        }
    }
}