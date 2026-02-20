using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ReportEngine.AtomicDomain.Database.Context;
using ReportEngine.AtomicDomain.Entities;

namespace ReportEngine.AtomicDomain.Repositories
{
    public class AtomicProjectRepository
    {
        private readonly AtomicAppContext _context;
        public AtomicProjectRepository(AtomicAppContext context)
        {
            _context = context;
        }

        public async Task AddNewProject(Project project)
        {
            await _context.AddAsync(project);
            await _context.SaveChangesAsync();
        }

        public async Task<Project?> GetProjectAsync(Expression<Func<Project, bool>> predicate)
        {
            return await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await _context.Projects
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
