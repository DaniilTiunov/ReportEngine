using ReportEngine.App.AppHelpers;
using ReportEngine.App.Model;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.Services.Core
{
    public class ContainerService
    {
        private readonly IProjectService _projectService;
        private readonly INotificationService _notificationService;

        public ContainerService(IProjectService projectService, INotificationService notificationService)
        {
            _projectService = projectService;
            _notificationService = notificationService;
        }

        public async Task CreateBatchAsync(ProjectModel projectModel)
        {
            if (projectModel == null || projectModel.CurrentProjectId == 0)
            {
                _notificationService.ShowInfo("Сначала выберите проект!");
                return;
            }

            await ExceptionHelper.SafeExecuteAsync(async() =>
            {
                var newBatch = new ContainerBatch
                {
                    ProjectInfoId = projectModel.CurrentProjectId,
                    Name = $"Очередь {projectModel.CurrentProjectId}",
                    BatchOrder = 0,
                    StandsCount = 0,
                };

                var addedBatch = await _projectService.CreateBatchAsync(newBatch);
                projectModel.ContainerBatchesInProject.Add(addedBatch);
            });
        }

        public async Task DeleteBanchAsync(ProjectModel projectModel)
        {

        }

        // Загрузить партии проекта
        public async Task LoadBatchesAsync(ProjectModel projectModel)
        {

        }

        // Добавить упаковку (ContainerStand) в выбранную партию
        public async Task AddContainerToBatchAsync(ProjectModel projectModel)
        {

        }

        // Удалить упаковку из партии (используется контекстное меню)
        public async Task RemoveContainerFromBatchAsync(ProjectModel projectModel)
        {

        }

        public async Task AddStandToContainerAsync(ProjectModel projectModel)
        {


        }

        public async Task RemoveStandFromContainerAsync(ProjectModel projectModel)
        {

        }
    }
}
