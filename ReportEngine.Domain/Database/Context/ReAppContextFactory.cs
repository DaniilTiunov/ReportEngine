using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using ReportEngine.Domain.Database.DbSettings;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.Models;

namespace ReportEngine.Domain.Database.Context;

public class ReAppContextFactory : IDesignTimeDbContextFactory<ReAppContext>
{
    public ReAppContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ReAppContext>();

        // Загружаем конфигурацию напрямую из файла
        var configPath = DirectoryHelper.GetConfigPath();
        var json = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<ReportEngineConfig>(json) 
                     ?? throw new InvalidOperationException("Failed to load configuration");

        // Создаем IOptions из загруженной конфигурации
        var options = Options.Create(config);

        // Используем новый метод
        DbContextOptionsFactory.Configure(optionsBuilder, options);

        return new ReAppContext(optionsBuilder.Options);
    }
}
