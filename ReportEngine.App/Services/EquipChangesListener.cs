using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;
using ReportEngine.Shared.Config.Logger;
using Serilog;

namespace ReportEngine.App.Services
{
    public class EquipChangesListener : BackgroundService
    {
        private readonly static string _jsFilePath = DirectoryHelper.GetConfigPath();
        private readonly string _connectionString = JsonHandler.GetConnectionString(_jsFilePath);
        
        private ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public EquipChangesListener(IServiceProvider serviceProvider)
        {
            _logger = LoggerConfig.InitializeLogger().ForContext<EquipChangesListener>();
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            _logger.Information("Подписка на изменения стартовала");

            var dbConnect = _serviceProvider.GetRequiredService<ReAppContext>();

            return Task.CompletedTask;
        }
    }
}
