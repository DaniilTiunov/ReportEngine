using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;

namespace ReportEngine.App.Services
{
    public class EquipChangesListener : BackgroundService
    {
        private readonly static string _jsFilePath = DirectoryHelper.GetConfigPath();
        private readonly string _connectionString = JsonHandler.GetConnectionString(_jsFilePath);

        private readonly IServiceProvider _serviceProvider;

        public EquipChangesListener(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var dbConnect = _serviceProvider.GetRequiredService<ReAppContext>();

            return Task.CompletedTask;
        }
    }
}
