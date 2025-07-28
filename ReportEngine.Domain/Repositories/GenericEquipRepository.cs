using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.Domain.Repositories
{
    public class GenericEquipRepository<T> : IGenericBaseRepository<T> where T : BaseEquip
    {
        private readonly ReAppContext _context;

        public GenericEquipRepository(ReAppContext context)
        {
            _context = context;
        }
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(T entity)
        {
            if (entity != null)
                _context.Set<T>().Remove(entity);

            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>()
                 .AsNoTracking()
                 .ToListAsync();
        }
    }
}
