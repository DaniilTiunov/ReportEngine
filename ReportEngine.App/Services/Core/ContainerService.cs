using System.Collections.ObjectModel;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.Model;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.Services.Core;

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
        if (projectModel?.CurrentProjectId == 0)
        {
            _notificationService.ShowInfo("Сначала выберите проект!");
            return;
        }

        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var existing = await _containerRepository
                .GetAllByProjectIdAsync(projectModel.CurrentProjectId);

            var nextOrder = existing.Any()
                ? existing.Max(b => b.BatchOrder) + 1
                : 1;

            var batch = new ContainerBatch
            {
                ProjectInfoId = projectModel.CurrentProjectId,
                Name = $"Партия {nextOrder}",
                BatchOrder = nextOrder
            };

            await _containerRepository.AddAsync(batch);

            await LoadAllData(projectModel);

            _notificationService.ShowInfo("Партия создана");
        });
    }

    public async Task DeleteBatchAsync(ProjectModel projectModel)
    {
        if (projectModel?.SelectedContainerBatch == null)
        {
            _notificationService.ShowInfo("Выберите партию!");
            return;
        }

        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _containerRepository
                .DeleteByIdAsync(projectModel.SelectedContainerBatch.Id);

            await LoadAllData(projectModel);

            projectModel.SelectedContainerBatch = null;

            _notificationService.ShowInfo("Партия удалена");
        });
    }

    public async Task AddContainerToBatchAsync(ProjectModel projectModel)
    {
        if (projectModel?.SelectedContainerBatch == null ||
            projectModel.SelectedContainerStand == null)
        {
            _notificationService.ShowInfo("Выберите партию и тару!");
            return;
        }

        await ExceptionHelper.SafeExecuteAsync(async () =>
        {

            await _containerRepository.AddContainerToBatchAsync(
                projectModel.SelectedContainerBatch.Id,
                projectModel.SelectedContainerStand);

            await RecalculateBatchAsync(projectModel.SelectedContainerBatch);

            await LoadAllData(projectModel);

            _notificationService.ShowInfo("Тара добавлена");
        });
    }

    public async Task RemoveContainerFromBatchAsync(ProjectModel projectModel)
    {
        if (projectModel?.SelectedContainerBatch == null ||
            projectModel.SelectedContainerStand == null)
        {
            _notificationService.ShowInfo("Выберите тару!");
            return;
        }

        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _containerRepository.RemoveContainerFromBatchAsync(
                projectModel.SelectedContainerBatch.Id,
                projectModel.SelectedContainerStand.Id);

            await RecalculateBatchAsync(projectModel.SelectedContainerBatch);

            await LoadAllData(projectModel);

            _notificationService.ShowInfo("Тара удалена");
        });
    }

    public async Task UpdateSelectedContainerAsync(ProjectModel projectModel)
    {
        var container = projectModel.SelectedContainerBatch;

        if (container == null)
        {
            _notificationService.ShowError("Не выбран ящик");
            return;
        }

        await _containerRepository.UpdateAsync(container);
    }

    public async Task AddStandToContainerAsync(ProjectModel projectModel)
    {
        if (projectModel?.SelectedContainerStand == null ||
            projectModel.SelectedStandInProject == null)
        {
            _notificationService.ShowInfo("Выберите тару и стенд!");
            return;
        }

        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _containerRepository.AddStandToContainerAsync(
                projectModel.SelectedContainerStand.Id,
                projectModel.SelectedStandInProject.Id);

            await RecalculateBatchAsync(projectModel.SelectedContainerBatch);

            await LoadAllData(projectModel);

            _notificationService.ShowInfo("Стенд добавлен");
        });
    }

    public async Task RemoveStandFromContainerAsync(ProjectModel projectModel)
    {
        if (projectModel?.SelectedContainerStand == null ||
            projectModel.SelectedStandInContainer == null)
        {
            _notificationService.ShowInfo("Выберите стенд!");
            return;
        }

        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _containerRepository.RemoveStandFromContainerAsync(
                projectModel.SelectedContainerStand.Id,
                projectModel.SelectedStandInContainer.Id);

            await RecalculateBatchAsync(projectModel.SelectedContainerBatch);

            await LoadAllData(projectModel);

            _notificationService.ShowInfo("Стенд удалён");
        });
    }

    private async Task RecalculateBatchAsync(ContainerBatch batch)
    {
        foreach (var container in batch.Containers)
        {
            RecalculateContainer(container);
        }

        batch.ContainersCount = batch.Containers.Count;
        batch.StandsCount = batch.Containers.Sum(c => c.StandsCount);

        await _containerRepository.UpdateAsync(batch);
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
