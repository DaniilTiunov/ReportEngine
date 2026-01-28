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
    public class EquipChangesListener
    {
        private readonly ConcurrentDictionary<Type, IEnumerable<IBaseEquip>> _cache = new();
        public IReadOnlyDictionary<Type, IEnumerable<IBaseEquip>> Cache => _cache;

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

        // Теперь этот метод можно вызывать извне
        public async Task LoadCurrentDataAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ReAppContext>();


            foreach (var type in GetEquipTypes(context))
            {
                dynamic repository = CreateRepository(scope, type);

                _cache[type] = (IEnumerable<IBaseEquip>)await repository.GetAllAsync();

                _logger.Information($"Загружено {_cache[type].Count()} записей для типа {type.Name}");
            }

            ;
        }

        private static object CreateRepository(IServiceScope scope, Type entityType)
        {
            var repositoryType =
                typeof(IGenericBaseRepository<,>).MakeGenericType(entityType, entityType);

            return scope.ServiceProvider.GetRequiredService(repositoryType);
        }

        private List<Type> GetEquipTypes(ReAppContext context)
        {
            return context.Model
                .GetEntityTypes()
                .Select(e => e.ClrType)
                .Where(t => typeof(IBaseEquip).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToList();
        }
    }
}
