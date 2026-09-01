using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportEngine.App.Services.Core;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Extensions.Extensions;
using ReportEngine.Domain.Entities.Other;

namespace ReportEngine.App.ViewModels;

public partial class ContainersViewModel : ObservableObject
{
    private readonly IContainerRepository _containerRepository;
    private readonly ContainerService _containerService;
    private readonly INotificationService _notificationService;
    private readonly ProjectViewModel _projectViewModel;
    private readonly IDialogService _dialogService;

    [ObservableProperty] private ObservableCollection<ContainerBatch> _allProjectBatches = new();
    [ObservableProperty] private ContainerBatch _selectedBatch;
    [ObservableProperty] private ObservableCollection<ContainerStand> _containersInBatch = new();
    [ObservableProperty] private ContainerStand _selectedStandContainer = new();
    

    public ContainersViewModel(
        ProjectViewModel projectViewModel,
        ContainerService containerService,
        INotificationService notificationService,
        IContainerRepository containerRepository, 
        IDialogService dialogService)
    {
        _projectViewModel = projectViewModel;
        _containerService = containerService;
        _notificationService = notificationService;
        _containerRepository = containerRepository;
        _dialogService = dialogService;

        _ = RefreshBatchesDataAsync();
        
        InitCommands();
    }

    public ICommand CreateBatchCommand { get; set; }
    public ICommand RefreshBatchesCommand { get; set; }
    public ICommand RemoveSelectedBatchCommand { get; set; }
    public ICommand AddContainerToBatchCommand { get; set; }
    public ICommand RemoveContainerFromBatchCommand { get; set; }
    
    private void InitCommands()
    {
        CreateBatchCommand = new AsyncRelayCommand(CreateBatchAsync);
        RefreshBatchesCommand = new AsyncRelayCommand(RefreshBatchesDataAsync);
        RemoveSelectedBatchCommand = new AsyncRelayCommand(RemoveSelectedBatchAsync);
        AddContainerToBatchCommand = new AsyncRelayCommand(AddContainerToBatchAsync);
        RemoveContainerFromBatchCommand = new AsyncRelayCommand(RemoveContainerFromBatchAsync);
    }

    private async Task CreateBatchAsync()
    {
        await _containerService.CreateBatchAsync(
            _projectViewModel.CurrentProjectModel.CurrentProjectId);
        
        await RefreshBatchesDataAsync();
        
        SelectedBatch = AllProjectBatches.LastOrDefault();
    }

    private async Task RefreshBatchesDataAsync()
    {
        var selectedBatchId = SelectedBatch?.Id;
        var selectedContainerId = SelectedStandContainer?.Id;
        
        AllProjectBatches.Clear();
        
        var batches = await _containerRepository.GetAllProjectBatchesInfoAsync(
            _projectViewModel.CurrentProjectModel.CurrentProjectId);

        foreach (var batch in batches)
        {
            AllProjectBatches.Add(batch);
        }

        if (selectedBatchId.HasValue)
        {
            var restoredBatch = AllProjectBatches.FirstOrDefault(b => b.Id == selectedBatchId.Value);
            if (restoredBatch != null)
            {
                SelectedBatch = restoredBatch;
                
                // Восстанавливаем выбранный контейнер
                if (selectedContainerId.HasValue && selectedContainerId.Value > 0)
                {
                    SelectedStandContainer = SelectedBatch.Containers
                        .FirstOrDefault(c => c.Id == selectedContainerId.Value);
                }
            }
            else
            {
                SelectedBatch = null;
                ContainersInBatch.Clear();
            }
        }
    }

    private async Task RemoveSelectedBatchAsync()
    {
        if (SelectedBatch == null) return;
        
        var batchId = SelectedBatch.Id;
        await _containerService.DeleteBatchAsync(batchId);
        
        await RefreshBatchesDataAsync();

        if (SelectedBatch?.Id == batchId)
        {
            SelectedBatch = null;
        }
    }

    private async Task AddContainerToBatchAsync()
    {
        if (SelectedBatch == null)
        {
            _notificationService.ShowInfo("Сначала выберите партию!");
            return;
        }

        var selectedContainer = _dialogService.ShowEquipDialog<Container>();
        
        if (selectedContainer == null) return;

        var container = new ContainerStand
        {
            ProjectInfoId = _projectViewModel.CurrentProjectModel.CurrentProjectId,
            Name = selectedContainer.Name,
            ContainerWeight = selectedContainer.Weight,
            ContainerCost = selectedContainer.Cost,
            ContainerBatchId = SelectedBatch.Id,
        };

        await _containerService.AddContainerToBatchAsync(
            _projectViewModel.CurrentProjectModel.CurrentProjectId,
            SelectedBatch.Id,
            container
        );
        
        var batchId = SelectedBatch.Id;
        
        await RefreshBatchesDataAsync();
        
        SelectedBatch = AllProjectBatches.FirstOrDefault(b => b.Id == batchId);
        
        if (SelectedBatch?.Containers != null)
        {
            SelectedStandContainer = SelectedBatch.Containers.LastOrDefault();
        }
    }

    private async Task RemoveContainerFromBatchAsync()
    {
        if (SelectedBatch == null)
        {
            _notificationService.ShowInfo("Сначала выберите партию!");
            return;
        }

        if (SelectedStandContainer == null || SelectedStandContainer.Id == 0)
        {
            _notificationService.ShowInfo("Выберите контейнер для удаления!");
            return;
        }

        var batchId = SelectedBatch.Id;
        var containerId = SelectedStandContainer.Id;

        await _containerService.RemoveContainerFromBatchAsync(
            _projectViewModel.CurrentProjectModel.CurrentProjectId,
            SelectedBatch.Id,
            containerId
        );
        
        await RefreshBatchesDataAsync();
        
        SelectedBatch = AllProjectBatches.FirstOrDefault(b => b.Id == batchId);
        
        SelectedStandContainer = null;
    }
}