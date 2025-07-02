using Microsoft.CodeAnalysis;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Domain.Entities;



namespace ReportEngine.Domain.Repositories
{
    internal class BaseRepository : IBaseRepository<Entities.ProjectInfo>
    {
        public Task AddAsync(Entities.ProjectInfo entity)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(Entities.ProjectInfo entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Entities.ProjectInfo entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Entities.ProjectInfo entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Entities.ProjectInfo>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Entities.ProjectInfo> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Entities.ProjectInfo entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Entities.ProjectInfo entity)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Entities.ProjectInfo>> IBaseRepository<Entities.ProjectInfo>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<Entities.ProjectInfo> IBaseRepository<Entities.ProjectInfo>.GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
