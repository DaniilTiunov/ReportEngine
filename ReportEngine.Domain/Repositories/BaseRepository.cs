using Microsoft.CodeAnalysis;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Domain.Entities;

namespace ReportEngine.Domain.Repositories
{
    internal class BaseRepository : IBaseRepository<ProjectInfo>
    {
        public Task AddAsync(ProjectInfo entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(ProjectInfo entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProjectInfo>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProjectInfo> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(ProjectInfo entity)
        {
            throw new NotImplementedException();
        }
    }
}
