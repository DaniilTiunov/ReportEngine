using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities.Other;

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
            var sw = new Stopwatch();
            sw.Start();


            var containersSizes = new Dictionary<string, (float Width, float Height, float Depth)>
            {
                { "ЭП-П.041.1100.000", (1150.00f, 1700.00f, 1400.00f) },
                { "ЭП-П.041.1200.000", (1350.00f, 1700.00f, 2050.00f) },
                { "ЭП-П.043.0100.000", (1400.00f, 1700.00f, 1600.00f) },
                { "ЭП-П.049.0100.000", (1450.00f, 1700.00f, 2200.00f) },
                { "ЭП-П.049.0200.000", (1100.00f, 1700.00f, 2200.00f) },
                { "ЭП-П.049.0300.000", (1100.00f, 1700.00f, 2520.00f) },
                { "ЭП-П.049.0400.000", (1100.00f, 1700.00f, 1800.00f) },
                { "ЭП-П.049.0500.000", (600.00f, 1700.00f, 2100.00f) },
                { "ЭП-П.052.0100.000", (1300.00f, 1700.00f, 1900.00f) },
                { "ЭП-П.059.0100.000", (1400.00f, 1700.00f, 1900.00f) },
                { "ЭП-П.068.0100.000", (1000.00f, 1700.00f, 1900.00f) },
                { "ЭП-П.084.0100.000", (1200.00f, 1700.00f, 1200.00f) },
                { "ЭП-П.084.0200.000", (1000.00f, 1700.00f, 1000.00f) },
                { "ЭП-П.093.0100.000", (1900.00f, 1700.00f, 1600.00f) },
                { "ЭП-П.093.0200.000", (1900.00f, 1700.00f, 2400.00f) },
                { "ЭП-П.094.0100.000", (800.00f, 1700.00f, 1100.00f) },
                { "ЭП-С.000.0001.000", (800.00f, 1700.00f, 600.00f) },
                { "ЭП-С.000.0002.000", (1400.00f, 1700.00f, 900.00f) },
                { "ЭП-С.000.0003.000", (2200.00f, 1700.00f, 1500.00f) },
                { "ЭП-С.000.0004.000", (2600.00f, 1700.00f, 1900.00f) },
                { "ЭП-С.000.0005.000", (1900.00f, 1700.00f, 3000.00f) },
                { "ЭП-С.000.0007.000", (2800.00f, 1700.00f, 1900.00f) },
                { "ЭП-С.000.0008.000", (1700.00f, 600.00f, 900.00f) },
                { "ЭП-С.000.0009.000", (1700.00f, 2000.00f, 1400.00f) },
                { "ЭП-С.000.0010.000", (900.00f, 1700.00f, 600.00f) },
                { "ЭП-С.000.0011.000", (1600.00f, 1700.00f, 600.00f) },
                { "ЭП-С.000.0012.000", (2600.00f, 2100.00f, 1200.00f) },
                { "ЭП-С.007.0100.000", (1400.00f, 1700.00f, 1900.00f) },
                { "ЭП-С.007.0100.000-01", (1700.00f, 1700.00f, 2300.00f) },
                { "ЭП-С.007.0100.000-02", (1900.00f, 1700.00f, 3600.00f) },
                { "ЭП-С.58.0001.000", (1800.00f, 2000.00f, 1000.00f) },
                { "ЭП-С.58.0002.000", (1400.00f, 2000.00f, 900.00f) },
                { "ЭП-С.58.42.100", (1800.00f, 2000.00f, 1200.00f) }
            };


            var allContainers = await context.Set<Container>().ToListAsync();

            foreach (var container in allContainers)
                if (containersSizes.TryGetValue(container.Name, out var size))
                {
                    container.Width = size.Width;
                    container.Height = size.Height;
                    container.Depth = size.Depth;
                    await context.SaveChangesAsync();
                }
        }
    }
}
