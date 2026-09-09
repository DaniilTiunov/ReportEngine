using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ReportEngine.Shared.Config.Models;

namespace ReportEngine.Domain.Database.DbSettings;

public static class DbContextOptionsFactory
{
    public static void Configure(
        DbContextOptionsBuilder options,
        IOptions<ReportEngineConfig> appSettings)  // Теперь принимает IOptions
    {
        var settings = appSettings.Value;
        
        if (settings.DatabaseSettings.DatabaseMode == "Online")
            options.UseNpgsql(settings.ConnectionStrings.DefaultConnection);
        else
            options.UseSqlite(settings.ConnectionStrings.SqliteConnectionString);
    }
}
