using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.Pipes;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Config.Logger;
using Serilog;

namespace ReportEngine.App.Services
{
    public class EquipChangesListener : BackgroundService
    {
        private readonly Dictionary<Type, IBaseEquip> _cache = new();
        private List<Type> _equipTypes = new();
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public EquipChangesListener(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = LoggerConfig.InitializeLogger();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Information("EquipChangesListener запущен");

            _equipTypes = GetEquipType();

            while (!stoppingToken.IsCancellationRequested)
            {
                await LoadEquipDataAsync<HeaterPipe>();

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private List<Type> GetEquipType()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ReAppContext>();

            return context.Model
                    .GetEntityTypes()
                    .Select(e => e.ClrType)
                    .Where(t =>
                        typeof(IBaseEquip).IsAssignableFrom(t) &&
                        t.IsClass &&
                        !t.IsAbstract)
                    .ToList();
        }

        private async Task LoadEquipDataAsync<TEntity>()
            where TEntity : class, IBaseEquip
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IGenericBaseRepository<TEntity, TEntity>>();

                var items = await repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private void LogError(Exception ex)
        {
            _logger.Error(ex, "Ошибка в EquipChangesListener");
        }
    }
}
