using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.Domain.Repositories
{
    public class GenericEquipRepository : IBaseRepository<BaseEquip>
    {
        private readonly ReAppContext _context;

        public GenericEquipRepository(ReAppContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BaseEquip entity)
        {
            await _context.Set<BaseEquip>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(BaseEquip entity)
        {
            if (entity != null)
                _context.Set<BaseEquip>().Remove(entity);
            
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<BaseEquip>> GetAllAsync()
        {
            return await _context.Set<BaseEquip>()
                 .AsNoTracking()
                 .ToListAsync();
        }



        public Task<BaseEquip> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task UpdateAsync(BaseEquip entity)
        {
            throw new NotImplementedException();
        }
        public Task<int> DeleteByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
