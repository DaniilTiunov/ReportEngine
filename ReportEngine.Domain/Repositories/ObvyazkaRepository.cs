using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.Domain.Repositories
{
    public class ObvyazkaRepository : IObvyazkaRepository
    {
        private readonly ReAppContext _context;

        public ObvyazkaRepository(ReAppContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Obvyazka obvyazka)
        {
            await _context.Set<Obvyazka>().AddAsync(obvyazka);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Obvyazka obvyazka)
        {
            if (obvyazka == null) return;

            if(obvyazka != null)
            {
                _context.Set<Obvyazka>().Remove(obvyazka);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Obvyazka>> GetAllAsync()
        {
            return await _context.Set<Obvyazka>()
                                .AsNoTracking()
                                .ToListAsync();
        }
        public async Task UpdateAsync(Obvyazka obvyazka)
        {
            var existingEntity = await _context.Set<Obvyazka>()
                .FirstOrDefaultAsync(c => c.Id == obvyazka.Id);

            if (existingEntity != null)
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(obvyazka);
            }
            await _context.SaveChangesAsync();
        }


        public Task<int> DeleteByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Obvyazka> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

    }
}
