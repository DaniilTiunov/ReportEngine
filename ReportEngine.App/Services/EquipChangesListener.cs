using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReportEngine.App.Display;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;

namespace ReportEngine.App.Services
{
    public class EquipChangesListener : BackgroundService
    {
        private static readonly string _jsFilePath = DirectoryHelper.GetConfigPath();
        private readonly string _connectionString = JsonHandler.GetConnectionString(_jsFilePath);

        private readonly ILogger<EquipChangesListener> _logger;
        private readonly IServiceProvider _serviceProvider;

        public EquipChangesListener(IServiceProvider serviceProvider, ILogger<EquipChangesListener> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Подписка на изменения стартовала");

            MessageBoxHelper.ShowInfo("Служба прослушивания изменений оборудования запущена.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ReAppContext>();

                    // TODO: ваша логика прослушивания изменений с использованием dbContext

                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // ожидаем завершения
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка в EquipChangesListener");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            _logger.LogInformation("Подписка на изменения остановлена");
        }
    }
}
