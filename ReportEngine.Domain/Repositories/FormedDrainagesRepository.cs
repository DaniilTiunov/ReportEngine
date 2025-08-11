using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.Domain.Repositories
{
    public class FormedDrainagesRepository : IFormedDrainagesRepository
    {
        private readonly ReAppContext _context;

        public FormedDrainagesRepository(ReAppContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FormedDrainage>> GetAllAsync()
        {
            return await _context.FormedDrainages.AsNoTracking().ToListAsync();
        }
        public async Task<FormedDrainage> GetByIdAsync(int id)
        {
            return await _context.FormedDrainages.FindAsync(id);
        }
        public async Task AddAsync(FormedDrainage entity)
        {
            await _context.FormedDrainages.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(FormedDrainage entity)
        {
            _context.FormedDrainages.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(FormedDrainage entity)
        {
            _context.FormedDrainages.Remove(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<int> DeleteByIdAsync(int id)
        {
            var entity = await _context.FormedDrainages.FindAsync(id);
            if (entity == null) return 0;
            _context.FormedDrainages.Remove(entity);
            await _context.SaveChangesAsync();
            return 1;
        }
        public async Task<IEnumerable<FormedDrainage>> GetAllWithPurposesAsync()
        {
            return await _context.FormedDrainages
                .Include(fd => fd.Purposes)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<FormedDrainage> GetByIdWithPurposesAsync(int id)
        {
            return await _context.FormedDrainages
                .Include(fd => fd.Purposes)
                .FirstOrDefaultAsync(fd => fd.Id == id);
        }
    }
}
