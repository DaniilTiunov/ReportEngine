using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;

namespace ReportEngine.Domain.Repositories
{
    public class GenericRepository
    {
        private readonly ReAppContext _context;

        public GenericRepository(ReAppContext context)
        {
            _context = context;
        }

        public async Task<T?> GetAsync<T>(
            Expression<Func<T, bool>> predicate)
            where T : class
        {
            return await _context.Set<T>()
                .FirstOrDefaultAsync(predicate);
        }
    }
}
