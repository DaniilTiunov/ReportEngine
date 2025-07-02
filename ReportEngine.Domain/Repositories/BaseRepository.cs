using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.Domain.Repositories
{
    internal class BaseRepository : IBaseRepository<ProjectInfo>
    {
        private readonly ReAppContext _context;

        public BaseRepository(ReAppContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProjectInfo entity)
        {
            _context.Add(entity);
            
        }

        public async Task DeleteAsync(ProjectInfo entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProjectInfo>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ProjectInfo> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(ProjectInfo entity)
        {
            throw new NotImplementedException();
        }
    }
}
