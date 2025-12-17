using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Config.Logger;
using Serilog;

namespace ReportEngine.App.Services
{
    public class EquipChangesListener : BackgroundService
    {
        private readonly Dictionary<Type, IEnumerable<IBaseEquip>> _cache = new();
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public EquipChangesListener(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = LoggerConfig.InitializeLogger();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await LoadCurrentDataAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task LoadCurrentDataAsync() 
        {
            var equipTypes = GetEquipTypes();

            using var scope = _serviceProvider.CreateScope();

            foreach (var type in equipTypes)
            {
                var repositoryType = typeof(IGenericBaseRepository<,>).MakeGenericType(type, type);
                dynamic repository = scope.ServiceProvider.GetRequiredService(repositoryType);

                IEnumerable<IBaseEquip> items = await repository.GetAllAsync();

                _cache[type] = items;

                _logger.Information($"Загружено {items.Count()} записей для типа {type.Name}");
            }
        }

        private IGenericBaseRepository<T, T> GetCurrentRepository<T>(IServiceProvider serviceProvider)
            where T : class, IBaseEquip
        {
            return serviceProvider.GetRequiredService<IGenericBaseRepository<T, T>>();
        }

        private List<Type> GetEquipTypes()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ReAppContext>();

            return context.Model
                .GetEntityTypes()
                .Select(e => e.ClrType)
                .Where(t => typeof(IBaseEquip).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToList();
        }

        public IEnumerable<IBaseEquip> GetCachedEquip(Type type)
        {
            return _cache.TryGetValue(type, out var items) ? items : Enumerable.Empty<IBaseEquip>();
        }
    }
}

