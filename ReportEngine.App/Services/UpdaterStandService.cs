using Microsoft.EntityFrameworkCore;
using ReportEngine.App.Model;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Background;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.Services
{
    public class UpdaterStandService(
        ReAppContext context,
        IProjectService projectService,
        IStandService standService,
        IProjectInfoRepository projectRepository,
        INotificationService notificationService)
    {
        private readonly ReAppContext _context = context;
        private readonly IProjectService _projectService = projectService;
        private readonly IStandService _standService = standService;
        private readonly IProjectInfoRepository _projectRepository = projectRepository;
        private readonly INotificationService _notificationService = notificationService;

        public async Task ApplyChangesAndSaveAsync(ProjectModel project)
        {
            var changes = await GetUnprocessedChangesAsync(project);

            foreach (var stand in project.Stands)
            {
                foreach (var change in changes)
                {
                    ApplyChangesToObject(stand, change);

                    ApplyChangesToCollection(stand.AllAdditionalEquipPurposesInStand, change);
                    ApplyChangesToCollection(stand.ObvyazkaAdditionalComponents, change);
                    ApplyChangesToCollection(stand.AllElectricalPurposesInStand, change);
                    ApplyChangesToCollection(stand.AllDrainagePurposesInStand, change);
                    ApplyChangesToCollection(stand.ObvyazkiInStand, change);

                    change.Processed = true;
                }

                await SyncStandPropertiesToObvyazkiAsync(stand);
            }

            await _projectService.UpdateStandEntity(project);
            await _standService.LoadObvyazkiInStandsAsync(project.Stands);
            await _standService.LoadStandsDataAsync(project.Stands);
            await _standService.LoadPurposesInStands(project.Stands);

            await _context.SaveChangesAsync();

            _notificationService.ShowInfo("Данные комплектующих обновлены!");
        }

        private void ApplyChangesToCollection<T>(IEnumerable<T> collection, TablesChanges change)
        {
            if (collection == null)
                return;

            foreach (var item in collection)
            {
                ApplyChangesToObject(item!, change);
            }
        }

        private void ApplyChangesToObject(object target, TablesChanges change)
        {
            var properties = target.GetType()
                .GetProperties()
                .Where(p => p.CanRead && p.CanWrite);

            foreach (var prop in properties)
            {
                // 🔹 Обработка string
                if (prop.PropertyType == typeof(string))
                {
                    var value = (string?)prop.GetValue(target);

                    if (!string.IsNullOrEmpty(value) &&
                        value.Contains(change.OldName))
                    {
                        prop.SetValue(target,
                            value.Replace(change.OldName!, change.NewName));
                    }
                }

                // 🔹 Обработка стоимости (float)
                if (prop.PropertyType == typeof(float) ||
                    prop.PropertyType == typeof(float?))
                {
                    var value = prop.GetValue(target);

                    if (value is float floatValue &&
                        change.OldCost.HasValue &&
                        floatValue == change.OldCost.Value)
                    {
                        prop.SetValue(target, change.NewCost);
                    }
                }
            }
        }

        public async Task SyncStandPropertiesToObvyazkiAsync(StandModel stand)
        {
            foreach (var obvyazka in stand.ObvyazkiInStand)
            {
                obvyazka.MaterialLine = stand.MaterialLine;
                obvyazka.Armature = stand.Armature;
                obvyazka.TreeSocket = stand.TreeSocket;
                obvyazka.KMCH = stand.KMCH;

                await _projectRepository.UpdateObvInStandAsync(stand.Id, obvyazka);
            }
        }

        public async Task<List<TablesChanges>> GetUnprocessedChangesAsync(ProjectModel project)
        {
            return await _context.TablesChanges
                .Where(c => c.Processed == false)
                .ToListAsync();
        }
    }
}
