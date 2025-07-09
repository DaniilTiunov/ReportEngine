using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.Domain.Repositories
{
    public class UserRepository : IBaseRepository<User>
    {
        private readonly ReAppContext _context;

        public UserRepository(ReAppContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Set<User>().AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteByIdAsync(int id)
        {
           
            var entity = await _context.Set<User>().FindAsync(id);

            if (entity == null)
                return 0;

                _context.Set<User>().Remove(entity);

            await _context.SaveChangesAsync();

            return 1;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Set<User>()
                                    .AsNoTracking()
                                    .ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Set<User>().FindAsync(id);
        }

        public Task UpdateAsync(User entity)
        {
            throw new NotImplementedException();
        }
    }
}
