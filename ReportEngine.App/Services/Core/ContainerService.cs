using ReportEngine.App.AppHelpers;
using ReportEngine.App.Model;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ReportEngine.App.ModelWrappers;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.Services.Core
{
    public class ContainerService
    {
        private readonly IContainerRepository _containerRepository ;
        private readonly INotificationService _notificationService;

        public ContainerService(IContainerRepository containerRepository, INotificationService notificationService)
        {
            _containerRepository = containerRepository;
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
                var existing = projectModel.ContainerBatchesInProject ?? new ObservableCollection<ContainerBatch>();
                var nextOrder = existing.Any() ? existing.Max(b => b.BatchOrder) + 1 : 1;

                var batch = new ContainerBatch
                {
                    ProjectInfoId = projectModel.CurrentProjectId,
                    Name = $"Партия {nextOrder}",
                    BatchOrder = nextOrder,
                    ContainersCount = 0,
                    StandsCount = 0
                };

                await _containerRepository.AddAsync(batch);

                if (projectModel.ContainerBatchesInProject == null)
                    projectModel.ContainerBatchesInProject = new ObservableCollection<ContainerBatch>();

                if (!projectModel.ContainerBatchesInProject.Any(b => b.Id == batch.Id))
                    projectModel.ContainerBatchesInProject.Add(batch);

                projectModel.SelectedContainerBatch = batch;
                
                _notificationService.ShowInfo("Очередь добавлена");
            });
        }

        public async Task DeleteBanchAsync(ProjectModel projectModel)
        {
            if (projectModel == null || projectModel.SelectedContainerBatch == null)
            {
                _notificationService.ShowInfo("Упаковку проекта!");
                return;
            }

            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var batchToDelete = projectModel.SelectedContainerBatch;

                await _containerRepository.DeleteByIdAsync(batchToDelete.Id);

                projectModel.ContainerBatchesInProject.Remove(batchToDelete);
                projectModel.SelectedContainerBatch = null;

                _notificationService.ShowInfo("Партия удалена");
            });
        }

        // Загрузить партии проекта
        public async Task LoadBatchesAsync(ProjectModel projectModel)
        {
            if (projectModel == null)
            {
                _notificationService.ShowInfo("Сначала создайте проект!");
                return; 
            }

            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var bathces = await _containerRepository.GetAllByProjectIdAsync(projectModel.CurrentProjectId);
            });
        }

        // Добавить упаковку (ContainerStand) в выбранную партию
        public async Task AddContainerToBatchAsync(ProjectModel projectModel)
        {
            if (projectModel == null || projectModel.CurrentProjectId == 0 || projectModel.SelectedContainerBatch == null)
            {
                _notificationService.ShowInfo("Сначала выберите\nупаковку проекта");
                return;
            }

            if (projectModel.SelectedContainerStand == null)
            {
                _notificationService.ShowInfo("Выберите или создайте упаковку для добавления");
                return;
            }

            if (projectModel.SelectedStand == null)
            {
                _notificationService.ShowInfo("Выберите стенд для добавления");
                return;
            }

            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                // Выполняем привязку в БД
                await _containerRepository.AddStandToContainerAsync(projectModel.SelectedContainerStand.Id, projectModel.SelectedStand.Id);
                
                // Добавляем выбранный UI-объект (StandModel) в коллекцию, если его там ещё нет
                if (!projectModel.StandsInContainer.Contains(projectModel.SelectedStand))
                    projectModel.StandsInContainer.Add(projectModel.SelectedStand);

                // Перезагрузим данные проекта из репозитория — это обновит счётчики и связанные объекты в UI
                await LoadAllData(projectModel);

                // Сброс выбора стенда для ввода следующего
                projectModel.SelectedStand = null;

                _notificationService.ShowInfo("Стенд добавлен в упаковку");
            });
        }

        // Удалить упаковку из партии (используется контекстное меню)
        public async Task RemoveContainerFromBatchAsync(ProjectModel projectModel)
        {
            if (projectModel == null || projectModel.CurrentProjectId == 0 || projectModel.SelectedContainerBatch == null)
            {
                _notificationService.ShowInfo("Сначала выберите\nупаковку проекта");
                return;
            }

            if (projectModel.SelectedContainerStand == null)
            {
                _notificationService.ShowInfo("Выберите или создайте\nупаковку для удаления");
                return;
            }

            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var containerToRemove = projectModel.SelectedContainerStand;

                if (containerToRemove.ProjectInfoId == 0)
                    containerToRemove.ProjectInfoId = projectModel.CurrentProjectId;
        
                await _containerRepository.RemoveContainerFromBatchAsync(projectModel.SelectedContainerBatch.Id, containerToRemove.Id);

                if (projectModel.ContainerStandsInProject != null)
                {
                    var existing = projectModel.ContainerStandsInProject.FirstOrDefault(c => c.Id == containerToRemove.Id);
                    if (existing != null)
                        projectModel.ContainerStandsInProject.Remove(existing);
                }

                projectModel.SelectedContainerStand = new ContainerStand
                {
                    ProjectInfoId = projectModel.CurrentProjectId
                };
                
                await LoadAllData(projectModel);

                _notificationService.ShowInfo("Упаковка удалена");
            });
        }

        public async Task AddStandToContainerAsync(ProjectModel projectModel)
        {
            if (projectModel == null || projectModel.CurrentProjectId == 0 || projectModel.SelectedContainerBatch == null)
            {
                _notificationService.ShowInfo("Сначала выберите\nупаковку проекта");
                return;
            }

            if (projectModel.SelectedContainerStand == null)
            {
                _notificationService.ShowInfo("Выберите или создайте\nупаковку для стенда");
                return;
            }
            
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                await _containerRepository.AddStandToContainerAsync(projectModel.SelectedContainerStand.Id, projectModel.SelectedStand.Id);
                
                projectModel.StandsInContainer.Add(projectModel.SelectedStand);
                
                await LoadAllData(projectModel);
                
                _notificationService.ShowInfo("Стенд добавлен в упаковку");
            });
        }

        public async Task RemoveStandFromContainerAsync(ProjectModel projectModel)
        {

        }

        public async Task LoadAllData(ProjectModel projectModel)
        { 
            projectModel.ContainerBatchesInProject.Clear();
            projectModel.ContainerStandsInProject.Clear();

            
            var batchesInProject = await _containerRepository.GetAllByProjectIdAsync(projectModel.CurrentProjectId);
            var containerStandsInProject = batchesInProject.SelectMany(b => b.Containers).ToList();
            var standsInContainer = containerStandsInProject
                .Where(c => c.Id == projectModel.SelectedContainerStand?.Id)
                .SelectMany(c => c.Stands)
                .ToList();

            foreach (var batch in batchesInProject)
            {
                projectModel.ContainerBatchesInProject.Add(batch);
            }

            foreach (var containerStand in containerStandsInProject)
            {
                projectModel.ContainerStandsInProject.Add(containerStand);
            }
        }
    }
}
