using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ReportEngine.Domain.Database.DbSettings;

namespace ReportEngine.Domain.Database.Context;

public class ReAppContextFactory : IDesignTimeDbContextFactory<ReAppContext>
{
    public ReAppContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ReAppContext>();

        DbContextOptionsFactory.Configure(
            optionsBuilder,
            "Online");

        return new ReAppContext(optionsBuilder.Options);
    }
}
