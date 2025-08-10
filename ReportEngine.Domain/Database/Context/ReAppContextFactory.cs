using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ReportEngine.Domain.Database.Context
{
    public class ReAppContextFactory : IDesignTimeDbContextFactory<ReAppContext>
    {
        public ReAppContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ReAppContext>();
            optionsBuilder.UseNpgsql("Host=127.0.0.1;Port=5433;Database=reportengine;Username=postgres;Password=postgres");
            return new ReAppContext(optionsBuilder.Options);
        }
    }
}
