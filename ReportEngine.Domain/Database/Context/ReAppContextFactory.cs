using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ReportEngine.Domain.Database.Context
{
    public class ReAppContextFactory : IDesignTimeDbContextFactory<ReAppContext>
    {
        public ReAppContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ReAppContext>();
            optionsBuilder.UseNpgsql("Host=172.16.10.58;Port=5432;Database=reportengine;Username=postgres;Password=postgres");
            return new ReAppContext(optionsBuilder.Options);
        }
    }
}
