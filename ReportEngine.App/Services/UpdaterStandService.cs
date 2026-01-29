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
                var stringProps = stand.GetType().GetProperties()
                    .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

                foreach (var change in changes)
                {
                    foreach (var prop in stringProps)
                    {
                        var value = (string?)prop.GetValue(stand);

                        if (!string.IsNullOrEmpty(value) &&
                            value.Contains(change.OldName))
                        {
                            prop.SetValue(
                                stand,
                                value.Replace(change.OldName, change.NewName)
                            );
                        }
                    }

                    change.Processed = true;
                }

                await SyncStandPropertiesToObvyazkiAsync(stand);
            }

            await _projectService.UpdateStandEntity(project);
            await _standService.LoadObvyazkiInStandsAsync(project.Stands);

            await _context.SaveChangesAsync();

            _notificationService.ShowInfo("Данные комплектующих обновлены!");
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
                .Where(c => !c.Processed == false)
                .ToListAsync();
        }
    }
}
