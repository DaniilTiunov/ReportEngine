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
                var toRemove = projectModel.ContainerStandsInProject
                                                                .Where(c => c.ContainerBatchId == batchToDelete.Id)
                                                                .ToList();
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

            var batch = projectModel.SelectedContainerBatch;
            var containerToAdd = projectModel.SelectedContainerStand;

            // Привязываем к партии
            containerToAdd.ContainerBatchId = batch.Id;

            await _containerRepository.AddContainerToBatchAsync(batch.Id, containerToAdd);

            // Инициализация коллекций, если нужно
            if (batch.Containers == null) batch.Containers = new List<ContainerStand>();
            batch.Containers.Add(containerToAdd);

            if (projectModel.ContainerStandsInProject == null)
                projectModel.ContainerStandsInProject = new ObservableCollection<ContainerStand>();
            if (!projectModel.ContainerStandsInProject.Any(c => c.Id == containerToAdd.Id))
                projectModel.ContainerStandsInProject.Add(containerToAdd);

            batch.ContainersCount = batch.Containers.Count;
            batch.StandsCount = batch.Containers.Sum(c => c.Stands?.Count ?? 0);

            // Оповещаем UI что список контейнеров для выбранной партии изменился
            projectModel.OnPropertyChanged(nameof(projectModel.ContainerStandsInSelectedBatch));
            projectModel.OnPropertyChanged(nameof(projectModel.ContainerBatchesInProject));

            _notificationService.ShowInfo("Добавлено!");
        }

        public async Task RemoveContainerFromBatchAsync(ProjectModel projectModel)
        {
            if (projectModel == null || projectModel.SelectedContainerBatch == null || projectModel.SelectedContainerStand == null)
            {
                _notificationService.ShowInfo("Выберите партию и упаковку для удаления!");
                return;
            }

            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var batch = projectModel.SelectedContainerBatch;
                var containerToRemove = projectModel.SelectedContainerStand;

                await _containerRepository.RemoveContainerFromBatchAsync(batch.Id, containerToRemove.Id);

                // Удаляем из коллекций
                batch.Containers?.Remove(containerToRemove);

                var inProject = projectModel.ContainerStandsInProject?.FirstOrDefault(c => c.Id == containerToRemove.Id);
                if (inProject != null)
                    projectModel.ContainerStandsInProject.Remove(inProject);

                // Обновляем счётчики
                batch.ContainersCount = batch.Containers?.Count ?? 0;
                batch.StandsCount = batch.Containers?.Sum(c => c.Stands?.Count ?? 0) ?? 0;

                // Если удаляли выбранную упаковку — сбросим её
                if (projectModel.SelectedContainerStand?.Id == containerToRemove.Id)
                {
                    projectModel.SelectedContainerStand = null;
                    projectModel.StandsInContainer?.Clear();
                }

                projectModel.OnPropertyChanged(nameof(projectModel.ContainerStandsInSelectedBatch));
                projectModel.OnPropertyChanged(nameof(projectModel.ContainerBatchesInProject));

                _notificationService.ShowInfo("Упаковка удалена");
            });
        }

        public async Task AddStandToContainerAsync(ProjectModel projectModel)
        {
            if (projectModel.SelectedContainerStand == null)
            {
                _notificationService.ShowInfo("Выберите упаковку");
            }

            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var batch = projectModel.SelectedContainerBatch;
                var container = projectModel.SelectedContainerStand;
                var standToAdd = projectModel.SelectedStand;
                
                await _containerRepository.AddStandToContainerAsync(container.Id, standToAdd.Id);
                
                projectModel.OnPropertyChanged(nameof(projectModel.ContainerStandsInSelectedBatch));
                projectModel.OnPropertyChanged(nameof(projectModel.StandsInSelectedContainer));
                
                _notificationService.ShowInfo("Стенд добавлен!");
            });
        }

        public async Task RemoveStandFromContainerAsync(ProjectModel projectModel)
        {
            if (projectModel.SelectedContainerStand == null)
            {
                _notificationService.ShowInfo("Выберите упаковку");
            }

            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var container = projectModel.SelectedContainerStand;
                var standToRemove = projectModel.SelectedStand;
                
                await _containerRepository.RemoveStandFromContainerAsync(container.Id, standToRemove.Id);

                standToRemove = null;
                
                projectModel.OnPropertyChanged(nameof(projectModel.ContainerStandsInSelectedBatch));
                projectModel.OnPropertyChanged(nameof(projectModel.StandsInSelectedContainer));
                
                _notificationService.ShowInfo("Стенд удалён!");
            });
        }

        public async Task LoadAllData(ProjectModel projectModel)
        {
            if (projectModel == null) return;

            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                projectModel.ContainerBatchesInProject.Clear();

                var batchesInProject = await _containerRepository.GetAllByProjectIdAsync(projectModel.CurrentProjectId);

                // наполняем партии
                foreach (var batch in batchesInProject)
                    projectModel.ContainerBatchesInProject.Add(batch);

                // собираем все контейнеры в общую коллекцию
                var allContainers = batchesInProject
                    .SelectMany(b => b.Containers ?? Enumerable.Empty<ContainerStand>())
                    .ToList();

                var allStands = allContainers
                    .SelectMany(s => s.Stands ?? Enumerable.Empty<Stand>())
                    .ToList();

                projectModel.ContainerStandsInProject = new ObservableCollection<ContainerStand>(allContainers);

                // если ранее была выбрана партия — попробуем восстановить ссылку на неё из загруженных данных
                if (projectModel.SelectedContainerBatch != null)
                {
                    var selected = projectModel.ContainerBatchesInProject.FirstOrDefault(b => b.Id == projectModel.SelectedContainerBatch.Id);
                    projectModel.SelectedContainerBatch = selected;
                }
                
                if (projectModel.SelectedContainerStand != null)
                {
                    var selectedContainer = projectModel.ContainerStandsInProject.FirstOrDefault(c => c.Id == projectModel.SelectedContainerStand.Id);
                    projectModel.SelectedContainerStand = selectedContainer;
                }

                projectModel.OnPropertyChanged(nameof(projectModel.ContainerStandsInSelectedBatch));
                projectModel.OnPropertyChanged(nameof(projectModel.StandsInSelectedContainer));
            });
        }
    }
}