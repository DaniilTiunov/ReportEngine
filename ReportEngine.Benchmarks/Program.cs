using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;

namespace ReportEngine.Benchmarks;

public class Program
{
    private static async Task Main(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ReAppContext>();
        optionsBuilder.UseNpgsql(
            "Host=172.16.0.210;Port=5432;Database=reportengine;Username=postgres;Password=postgres");

        var reAppContext = new ReAppContext(optionsBuilder.Options);

        using (reAppContext)
        {
        }

        ;
    }
}
