using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

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
        public async Task<ProjectInfo> GetByIdAsync(int id) // Не используется
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
        public async Task<int> DeleteByIdAsync(int id) // Не используется
        {
            var entityProjectInfo = await _context.Set<ProjectInfo>().FindAsync(id);
            if (entityProjectInfo == null)
                return 0;

            _context.Set<ProjectInfo>().Remove(entityProjectInfo);
            await _context.SaveChangesAsync();
            return 1;
        }
        public async Task AddStandAsync(int projectId, Stand stand)
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

        }
    }
}
