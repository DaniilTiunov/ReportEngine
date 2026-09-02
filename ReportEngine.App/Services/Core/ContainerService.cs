using System.Collections.ObjectModel;
using ReportEngine.App.Model;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.Services.Notification;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.Services.Core;

public class ContainerService
{
    private readonly IContainerRepository _containerRepository;
    private readonly ExceptionService _exceptionService;
    private readonly INotificationService _notificationService;

    public ContainerService(
        IContainerRepository containerRepository,
        INotificationService notificationService,
        ExceptionService exceptionService)
    {
        _containerRepository = containerRepository;
        _notificationService = notificationService;
        _exceptionService = exceptionService;
    }

    public async Task<ContainerBatch> CreateBatchAsync(int projectId)
    {
        if (projectId == 0)
        {
            _notificationService.ShowInfo("Сначала выберите проект!");
            return null;
        }

        var newBatch = new ContainerBatch();
        
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            var existing = await _containerRepository
                .GetAllByProjectIdAsync(projectId);

            var nextOrder = existing.Any()
                ? existing.Max(b => b.BatchOrder) + 1
                : 1;

            var newBatch = new ContainerBatch
            {
                ProjectInfoId = projectId,
                Name = $"Партия {nextOrder}",
                BatchOrder = nextOrder
            };

            await _containerRepository.AddAsync(newBatch);

            _notificationService.ShowInfo($"Партия создана {newBatch.Name}");

        });
        
        return newBatch;
    }

    public async Task DeleteBatchAsync(int batchId)
    {
        if (batchId == null)
        {
            _notificationService.ShowInfo("Выберите партию!");
            return;
        }

        await _exceptionService.SafeExecuteAsync(async () =>
        {
            await _containerRepository.DeleteByIdAsync(batchId);
            
            await RecalculateAndUpdateAllBatches(batchId);
            
            _notificationService.ShowInfo("Партия удалена");
        });
    }

    public async Task AddContainerToBatchAsync(int projectId, int batchId, ContainerStand container)
    {
        if (projectId == null)
        {
            _notificationService.ShowInfo("Выберите партию!");
            return;
        }

        await _exceptionService.SafeExecuteAsync(async () =>
        {
            await _containerRepository.AddContainerToBatchAsync(batchId, container);

            await RecalculateAndUpdateAllBatches(projectId);
            
            _notificationService.ShowInfo("Тара добавлена");
        });
    }

    public async Task RemoveContainerFromBatchAsync(int projectId, 
        int batchId,
        int containerId)
    {
        if (projectId == null || batchId == null || containerId == null)
        {
            _notificationService.ShowInfo("Выберите партию!");
            return;
        }
        
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            await _containerRepository.RemoveContainerFromBatchAsync(batchId, containerId);

            await RecalculateAndUpdateAllBatches(projectId);
            
            _notificationService.ShowInfo("Тара удалена");
        });
    }

    public async Task UpdateSelectedContainerAsync(ProjectModel projectModel)
    {
        var container = projectModel.SelectedContainerBatch;

        if (container == null)
        {
            _notificationService.ShowError("Не выбрана упаковка");
            return;
        }

        RecalculateBatch(container);

        await _containerRepository.UpdateAsync(container);
    }

    public async Task RecalculateAndUpdateAllBatches(int projectId)
    {
        var allBatches = await _containerRepository.GetAllProjectBatchesInfoAsync(projectId);

        foreach (var batch in allBatches)
        {
            RecalculateBatch(batch);
            await _containerRepository.UpdateAsync(batch);
        }
    }

    public async Task AddStandToContainerAsync(int projectId, 
        int containerId, 
        int standId)
    {
        if (projectId == null)
        {
            _notificationService.ShowInfo("Выберите тару!");
            return;
        }

        await _exceptionService.SafeExecuteAsync(async () =>
        {
            await _containerRepository.AddStandToContainerAsync(containerId, standId);

            await RecalculateAndUpdateAllBatches(projectId);

            _notificationService.ShowInfo("Стенд добавлен");
        });
    }

    public async Task RemoveStandFromContainerAsync(int projectId, 
        int containerId, 
        int standId)
    {
        if (projectId == null)
        {
            _notificationService.ShowInfo("Выберите упаковку!");
            return;
        }

        await _exceptionService.SafeExecuteAsync(async () =>
        {
            await _containerRepository.RemoveStandFromContainerAsync(containerId, standId);

            await RecalculateAndUpdateAllBatches(projectId);

            _notificationService.ShowInfo("Стенд удалён");
        });
    }


    private void RecalculateBatch(ContainerBatch batch)
    {
        foreach (var container in batch.Containers) RecalculateContainer(container);

        batch.ContainersCount = batch.Containers.Count;
        batch.StandsCount = batch.Containers.Sum(c => c.StandsCount);
    }

    private void RecalculateContainer(ContainerStand container)
    {
        container.StandsCount = container.Stands?.Count ?? 0;
        container.StandsWeight = container.Stands?.Sum(s => s.Weight) ?? 0;
    }

    public async Task LoadBatchesAsync(ProjectModel projectModel)
    {
        if (projectModel == null)
        {
            _notificationService.ShowInfo("Сначала создайте проект!");
            return;
        }

        await _exceptionService.SafeExecuteAsync(async () =>
        {
            var batches = await _containerRepository.GetAllByProjectIdAsync(projectModel.CurrentProjectId);

            // Обновляем партии без сброса выбранной партии, если возможно
            if (projectModel.ContainerBatchesInProject == null)
                projectModel.ContainerBatchesInProject = new ObservableCollection<ContainerBatch>();
            else
                projectModel.ContainerBatchesInProject.Clear();

            foreach (var b in batches)
                projectModel.ContainerBatchesInProject.Add(b);

            await LoadAllData(projectModel);
        });
    }

    public async Task LoadAllData(ProjectModel projectModel)
    {
        if (projectModel == null) return;

        var selectedBatchId = projectModel.SelectedContainerBatch?.Id;
        var selectedContainerId = projectModel.SelectedContainerStand?.Id;

        var batches = await _containerRepository
            .GetAllByProjectIdAsync(projectModel.CurrentProjectId);

        projectModel.ContainerBatchesInProject.Clear();

        foreach (var batch in batches)
            projectModel.ContainerBatchesInProject.Add(batch);

        // Восстановление выбора партии
        projectModel.SelectedContainerBatch =
            projectModel.ContainerBatchesInProject
                .FirstOrDefault(b => b.Id == selectedBatchId);

        // Собираем контейнеры
        var containers = batches
            .SelectMany(b => b.Containers ?? Enumerable.Empty<ContainerStand>())
            .ToList();

        projectModel.ContainerStandsInProject.Clear();

        foreach (var container in containers)
            projectModel.ContainerStandsInProject.Add(container);

        // Восстановление выбора контейнера
        projectModel.SelectedContainerStand =
            projectModel.ContainerStandsInProject
                .FirstOrDefault(c => c.Id == selectedContainerId);

        projectModel.OnPropertyChanged(nameof(projectModel.ContainerStandsInSelectedBatch));
        projectModel.OnPropertyChanged(nameof(projectModel.StandsInSelectedContainer));
    }
}
