using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ReportEngine.AtomicDomain.Database.Context
{
    public class AtomicAppContextFactory : IDesignTimeDbContextFactory<AtomicAppContext>
    {
        public AtomicAppContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AtomicAppContext>();
            optionsBuilder.UseNpgsql(
                "Host=172.16.0.210;Port=5432;Database=atomicreportengine;Username=postgres;Password=postgres");
            return new AtomicAppContext(optionsBuilder.Options);
        }
    }
}
