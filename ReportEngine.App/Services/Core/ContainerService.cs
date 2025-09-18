using ReportEngine.App.AppHelpers;
using ReportEngine.App.Model;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using System.Collections.ObjectModel;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.Services.Core
{
    public class ContainerService
    {
        private readonly IContainerRepository _containerRepository;
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

            await ExceptionHelper.SafeExecuteAsync(async () =>
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
                
                if (!projectModel.ContainerBatchesInProject.Any(b => b.Id == batch.Id))
                    projectModel.ContainerBatchesInProject.Add(batch);
                
                projectModel.SelectedContainerBatch = batch;

                _notificationService.ShowInfo("Упаковка проекта добавлена");
            });
        }

        public async Task DeleteBanchAsync(ProjectModel projectModel)
        {
            if (projectModel == null || projectModel.SelectedContainerBatch == null)
            {
                _notificationService.ShowInfo("Выберите упаковку проекта!");
                return;
            }

            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var batchToDelete = projectModel.SelectedContainerBatch;

                await _containerRepository.DeleteByIdAsync(batchToDelete.Id);

                projectModel.ContainerBatchesInProject.Remove(batchToDelete);

                // Если удаляли выбранную партию — очищаем список упаковок для выбранной партии
                if (projectModel.SelectedContainerBatch?.Id == batchToDelete.Id)
                {
                    projectModel.SelectedContainerBatch = null;
                    projectModel.ContainerStandsInSelectedBatch?.Clear();
                }

                // Также удаляем все упаковки этой партии из общей коллекции
                var toRemove = projectModel.ContainerStandsInProject.Where(c => c.ContainerBatchId == batchToDelete.Id).ToList();
                foreach (var c in toRemove)
                    projectModel.ContainerStandsInProject.Remove(c);

                _notificationService.ShowInfo("Партия удалена");
            });
        }

        public async Task LoadBatchesAsync(ProjectModel projectModel)
        {
            if (projectModel == null)
            {
                _notificationService.ShowInfo("Сначала создайте проект!");
                return;
            }

            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var batches = await _containerRepository.GetAllByProjectIdAsync(projectModel.CurrentProjectId);

                // Обновляем партии без сброса выбранной партии, если возможно
                if (projectModel.ContainerBatchesInProject == null)
                    projectModel.ContainerBatchesInProject = new ObservableCollection<ContainerBatch>();
                else
                    projectModel.ContainerBatchesInProject.Clear();

                foreach (var b in batches)
                    projectModel.ContainerBatchesInProject.Add(b);

                // Обновим упаковки тоже (полная загрузка)
                await LoadAllData(projectModel);
            });
        }

        public async Task AddContainerToBatchAsync(ProjectModel projectModel)
        {
            if (projectModel == null || projectModel.SelectedContainerBatch == null)
            {
                _notificationService.ShowInfo("Выберите упаковку проекта!");
                return;
            }

            var containerToAdd = projectModel.SelectedContainerStand;
            
            await _containerRepository.AddContainerToBatchAsync(projectModel.SelectedContainerBatch.Id, projectModel.SelectedContainerStand);
            
            projectModel.ContainerStandsInSelectedBatch.Add(containerToAdd);
            
            _notificationService.ShowInfo("Добавлено!");
        }

        public async Task RemoveContainerFromBatchAsync(ProjectModel projectModel)
        {

        }

        public async Task AddStandToContainerAsync(ProjectModel projectModel)
        {
 
        }

        public async Task RemoveStandFromContainerAsync(ProjectModel projectModel)
        {
            
        }

        public async Task LoadAllData(ProjectModel projectModel)
        {
            projectModel.ContainerBatchesInProject.Clear();
            projectModel.ContainerStandsInSelectedBatch.Clear();
            
            var batchesInProject = await _containerRepository.GetAllByProjectIdAsync(projectModel.CurrentProjectId);
            
            foreach (var batch in batchesInProject)
            {
                projectModel.ContainerBatchesInProject.Add(batch);
            }
        }
    }
}