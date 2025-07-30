using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.Domain.Repositories
{
    public class GenericEquipRepository<T, TEntity> : IGenericBaseRepository<T, TEntity>
        where T : IBaseEquip
        where TEntity : class
    {
        private readonly ReAppContext _context;

        public GenericEquipRepository(ReAppContext context)
        {
            _context = context;
        }
        public async Task AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(TEntity entity)
        {
            if (entity != null)
                _context.Set<TEntity>().Remove(entity);

            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>()
                 .AsNoTracking()
                 .ToListAsync();
        }
    }
}
