using Microsoft.EntityFrameworkCore;
using ReportEngine.App.Model;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Background;
using ReportEngine.Domain.Database.Context;

namespace ReportEngine.App.Services
{
    public class UpdaterStandService
    {
        private readonly EquipChangesListener _changesListener;
        private readonly ReAppContext _context;
        private readonly IProjectService _projectService;

        public UpdaterStandService(EquipChangesListener changesListener,
            ReAppContext context,
            IProjectService projectService)
        {
            _changesListener = changesListener;
            _context = context;
            _projectService = projectService;
        }

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
                        if (!string.IsNullOrEmpty(value) && value.Contains(change.OldName))
                        {
                            prop.SetValue(stand, value.Replace(change.OldName, change.NewName));
                        }
                    }

                    // Помечаем изменение как обработанное
                    change.Processed = true;
                    change.ChangedAt = DateTime.UtcNow;
                }
            }

            await _projectService.UpdateStandEntity(project);

            await _context.SaveChangesAsync();
        }

        public async Task<List<TablesChanges>> GetUnprocessedChangesAsync(ProjectModel project)
        {
            return await _context.TablesChanges
                .Where(c => c.Processed == false)
                .ToListAsync();
        }
    }
}
