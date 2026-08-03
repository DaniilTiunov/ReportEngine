using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Repositories;

public class Program
{
    private static async Task Main(string[] args)
    {
        var conString = "Host=172.16.0.210;Port=5432;Database=reportengine;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<ReAppContext>()
            .UseNpgsql(conString)
            .Options;

        using (var context = new ReAppContext(options))
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var projectRepository = new ProjectInfoRepository(context);

            var projectInfo = await projectRepository.GetFullProjectAsync(214);
            var stands = projectInfo.Stands;

            Console.WriteLine("Всё супер кол-во стендов:");
            Console.WriteLine(projectInfo.Stands.Count());

            Console.WriteLine("Время запроса:");
            Console.WriteLine(sw.ElapsedMilliseconds / 1000);
            sw.Stop();

            Console.ReadLine();
        }
    }
}
