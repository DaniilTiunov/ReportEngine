using System.Collections.Concurrent;
using DocumentFormat.OpenXml.InkML;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Config.Logger;
using Serilog;

namespace ReportEngine.App.Services
{
    public class EquipChangesListener : BackgroundService
    {
        private readonly ConcurrentDictionary<Type, IEnumerable<IBaseEquip>> _cache = new();
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ReAppContext _dbContext;
        private readonly IProjectService projectService;

        public EquipChangesListener(IServiceProvider serviceProvider, ReAppContext dbContext)
        {
            _serviceProvider = serviceProvider;
            _logger = LoggerConfig.InitializeLogger();
            _dbContext = dbContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                try
                {
                    await LoadCurrentDataAsync();
                }
                catch(Exception ex)
                {
                    _logger.Error(ex, "Фоновый процесс прервался");
                }
                
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        // Загрузка данных
        private async Task LoadCurrentDataAsync()
        {

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ReAppContext>();

            var equipTypes = GetEquipTypes(context);


            foreach (var type in equipTypes)
            {
                dynamic repository = CreateRepository(scope, type);

                IEnumerable<IBaseEquip> items = await repository.GetAllAsync();

                _cache[type] = items;

                //_logger.Information($"Загружено {items.Count()} записей для типа {type.Name}");
            }
        }

        /// <summary>
        /// Создаёт экземпляр обобщённого репозитория для указанного типа сущности.
        /// </summary>
        /// <param name="scope">DI scope для разрешения зависимостей.</param>
        /// <param name="entityType">Тип сущности, для которой требуется репозиторий.</param>
        /// <returns>Экземпляр репозитория.</returns>
        private static object CreateRepository(IServiceScope scope, Type entityType)
        {
            var repositoryType =
                    typeof(IGenericBaseRepository<,>).MakeGenericType(entityType, entityType);

            return scope.ServiceProvider.GetRequiredService(repositoryType);
        }

        // Получение типов всех IBaseEquip
        private List<Type> GetEquipTypes(ReAppContext context)
        {
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
