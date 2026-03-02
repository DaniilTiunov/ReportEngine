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

    public async Task CreateBatchAsync(ProjectModel model)
    {
        if (model?.CurrentProjectId == 0)
        {
            _notificationService.ShowInfo("Сначала выберите проект!");
            return;
        }

        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var existing = await _containerRepository
                .GetAllByProjectIdAsync(model.CurrentProjectId);

            var nextOrder = existing.Any()
                ? existing.Max(b => b.BatchOrder) + 1
                : 1;

            var batch = new ContainerBatch
            {
                ProjectInfoId = model.CurrentProjectId,
                Name = $"Партия {nextOrder}",
                BatchOrder = nextOrder
            };

            await _containerRepository.AddAsync(batch);

            await LoadAllData(model);

            _notificationService.ShowInfo("Партия создана");
        });
    }

    public async Task DeleteBatchAsync(ProjectModel model)
    {
        if (model?.SelectedContainerBatch == null)
        {
            _notificationService.ShowInfo("Выберите партию!");
            return;
        }

        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _containerRepository
                .DeleteByIdAsync(model.SelectedContainerBatch.Id);

            await LoadAllData(model);

            model.SelectedContainerBatch = null;

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

    public async Task AddContainerToBatchAsync(ProjectModel model)
    {
        if (model?.SelectedContainerBatch == null ||
            model.SelectedContainerStand == null)
        {
            _notificationService.ShowInfo("Выберите партию и тару!");
            return;
        }

        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _containerRepository.AddContainerToBatchAsync(
                model.SelectedContainerBatch.Id,
                model.SelectedContainerStand);

            await LoadAllData(model);

            _notificationService.ShowInfo("Тара добавлена");
        });
    }

    public async Task RemoveContainerFromBatchAsync(ProjectModel model)
    {
        if (model?.SelectedContainerBatch == null ||
            model.SelectedContainerStand == null)
        {
            _notificationService.ShowInfo("Выберите тару!");
            return;
        }

        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _containerRepository.RemoveContainerFromBatchAsync(
                model.SelectedContainerBatch.Id,
                model.SelectedContainerStand.Id);

            await LoadAllData(model);

            _notificationService.ShowInfo("Тара удалена");
        });
    }

    public async Task UpdateSelectedContainerAsync(ProjectModel model)
    {
        var container = model.SelectedContainerStand;

        if (container == null)
        {
            _notificationService.ShowError("Не выбран ящик");
            return;
        }

        await _containerRepository.UpdateContainerAsync(container);
    }

    public async Task AddStandToContainerAsync(ProjectModel model)
    {
        if (model?.SelectedContainerStand == null ||
            model.SelectedStandInProject == null)
        {
            _notificationService.ShowInfo("Выберите тару и стенд!");
            return;
        }

        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _containerRepository.AddStandToContainerAsync(
                model.SelectedContainerStand.Id,
                model.SelectedStandInProject.Id);

            await LoadAllData(model);

            _notificationService.ShowInfo("Стенд добавлен");
        });
    }

    public async Task RemoveStandFromContainerAsync(ProjectModel model)
    {
        if (model?.SelectedContainerStand == null ||
            model.SelectedStandInContainer == null)
        {
            _notificationService.ShowInfo("Выберите стенд!");
            return;
        }

        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _containerRepository.RemoveStandFromContainerAsync(
                model.SelectedContainerStand.Id,
                model.SelectedStandInContainer.Id);

            await LoadAllData(model);

            _notificationService.ShowInfo("Стенд удалён");
        });
    }

    public async Task LoadAllData(ProjectModel model)
    {
        if (model == null) return;

        var selectedBatchId = model.SelectedContainerBatch?.Id;
        var selectedContainerId = model.SelectedContainerStand?.Id;

        var batches = await _containerRepository
            .GetAllByProjectIdAsync(model.CurrentProjectId);

        model.ContainerBatchesInProject.Clear();

        foreach (var batch in batches)
            model.ContainerBatchesInProject.Add(batch);

        // Восстановление выбора партии
        model.SelectedContainerBatch =
            model.ContainerBatchesInProject
                .FirstOrDefault(b => b.Id == selectedBatchId);

        // Собираем контейнеры
        var containers = batches
            .SelectMany(b => b.Containers ?? Enumerable.Empty<ContainerStand>())
            .ToList();

        model.ContainerStandsInProject.Clear();

        foreach (var container in containers)
            model.ContainerStandsInProject.Add(container);

        // Восстановление выбора контейнера
        model.SelectedContainerStand =
            model.ContainerStandsInProject
                .FirstOrDefault(c => c.Id == selectedContainerId);

        model.OnPropertyChanged(nameof(model.ContainerStandsInSelectedBatch));
        model.OnPropertyChanged(nameof(model.StandsInSelectedContainer));
    }
}
