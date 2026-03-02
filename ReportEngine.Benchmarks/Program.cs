using BenchmarkDotNet.Running;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Repositories;

namespace ReportEngine.Benchmarks
{
    public class Program
    {
        static async Task Main(string[] args)
        {

            var optionsBuilder = new DbContextOptionsBuilder<ReAppContext>();
            optionsBuilder.UseNpgsql(
                "Host=172.16.0.210;Port=5432;Database=reportengine;Username=postgres;Password=postgres");

            ReAppContext reAppContext = new ReAppContext(optionsBuilder.Options);

            using (reAppContext)
            {
                var zhopa = new GenericRepository(reAppContext);
                
                Console.WriteLine("АГААА");
            }
           
        }
    }

}
