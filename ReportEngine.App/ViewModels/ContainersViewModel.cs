using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Enums;
using ReportEngine.App.Services.Core;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.Services.Notification;
using ReportEngine.App.Views.Windows.Dialog;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Extensions.Extensions;
using ReportEngine.Domain.Entities.Other;
using ReportEngine.Export.DTO;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;

namespace ReportEngine.App.ViewModels;

public partial class ContainersViewModel : ObservableObject
{
    private readonly IContainerRepository _containerRepository;
    private readonly ContainerService _containerService;
    private readonly INotificationService _notificationService;
    private readonly ProjectViewModel _projectViewModel;
    private readonly IDialogService _dialogService;
    private readonly IProjectInfoRepository _projectInfoRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly IReportService _reportService;
    
    [ObservableProperty] private ObservableCollection<ContainerBatch> _allProjectBatches = new();
    [ObservableProperty] private ObservableCollection<Stand> _standsInProject = new();
    [ObservableProperty] private ObservableCollection<ContainerStand> _containersInBatch = new();
    [ObservableProperty] private ObservableCollection<Stand> _standsInContainer = new();
    [ObservableProperty] private ObservableCollection<Stand> _availableStands  = new();
    [ObservableProperty] private ContainerBatch _selectedBatch;
    [ObservableProperty] private ContainerStand _selectedStandContainer = new();
    [ObservableProperty] private Stand _selectedStand;
    [ObservableProperty] private ReportType _selectedReportType;
    
    private readonly Dictionary<ReportType, string> _reportDisplayNames = new()
    {
        { ReportType.SummaryReport, "📊 Сводная ведомость" },
        { ReportType.ComponentsListReport, "🔧 Ведомость комплектующих" },
        { ReportType.NameplatesReport, "🏷 Шильдики и таблички" },
        { ReportType.MarksReport, "✏️ Маркировка" },
        { ReportType.ContainerReport, "📦 Тара" },
        { ReportType.ProductionReport, "🏭 Производство" },
        { ReportType.FinPlanReport, "💰 Фин. план" },
        { ReportType.PassportsReport, "📋 Паспорт" },
        { ReportType.TechnologicalCards, "📑 Тех. карты" },
        { ReportType.FlatSummaryReport, "📊 Сводная ведомость 1С" }
    };

    public ContainersViewModel(
        ProjectViewModel projectViewModel,
        ContainerService containerService,
        INotificationService notificationService,
        IContainerRepository containerRepository, 
        IDialogService dialogService, 
        IProjectInfoRepository projectInfoRepository, 
        IServiceProvider serviceProvider, 
        IReportService reportService)
    {
        _projectViewModel = projectViewModel;
        _containerService = containerService;
        _notificationService = notificationService;
        _containerRepository = containerRepository;
        _dialogService = dialogService;
        _projectInfoRepository = projectInfoRepository;
        _serviceProvider = serviceProvider;
        _reportService = reportService;
        
        _ = InitializeAsync();
        
        InitCommands();
    }

    public ICommand CreateBatchCommand { get; set; }
    public ICommand RefreshBatchesCommand { get; set; }
    public ICommand RemoveSelectedBatchCommand { get; set; }
    public ICommand AddContainerToBatchCommand { get; set; }
    public ICommand RemoveContainerFromBatchCommand { get; set; }
    public ICommand AddStandToContainerCommand {get; set;}
    public ICommand RemoveStandFromContainerCommand { get; set; }
    public ICommand GenerateSelectedReportCommand { get; set; }
    
    private void InitCommands()
    {
        CreateBatchCommand = new AsyncRelayCommand(CreateBatchAsync);
        RefreshBatchesCommand = new AsyncRelayCommand(RefreshBatchesDataAsync);
        RemoveSelectedBatchCommand = new AsyncRelayCommand(RemoveSelectedBatchAsync);
        AddContainerToBatchCommand = new AsyncRelayCommand(AddContainerToBatchAsync);
        RemoveContainerFromBatchCommand = new AsyncRelayCommand(RemoveContainerFromBatchAsync);
        AddStandToContainerCommand = new AsyncRelayCommand(AddStandToContainerAsync);
        RemoveStandFromContainerCommand = new AsyncRelayCommand(RemoveStandFromContainerAsync);
        GenerateSelectedReportCommand = new AsyncRelayCommand(GenerateSelectedReportAsync);
    }

    private async Task InitializeAsync()
    {
        await LoadStandsAsync();
        await RefreshBatchesDataAsync();

        InitSelectedItems();
        UpdateAvailableStands();
    }
    
    private void InitSelectedItems()
    {
        SelectedBatch = AllProjectBatches.FirstOrDefault();

        if (SelectedBatch == null)
            return;

        SelectedStandContainer = SelectedBatch.Containers?.FirstOrDefault();

        UpdateAvailableStands();
    }

    private async Task LoadStandsAsync()
    {
        StandsInProject.Clear();
        var stands = await _projectInfoRepository.GetProjectWithStandsAsync(
            _projectViewModel.CurrentProjectModel.CurrentProjectId);
        
        StandsInProject = stands.ToObservable();
        
        UpdateAvailableStands();
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

                if (selectedContainerId.HasValue && selectedContainerId.Value > 0)
                {
                    SelectedStandContainer = SelectedBatch.Containers
                        .FirstOrDefault(c => c.Id == selectedContainerId.Value);
                    
                    UpdateAvailableStands();
                }
            }
            else
            {
                SelectedBatch = null;
                ContainersInBatch.Clear();
            }
        }
        
        UpdateAvailableStands();
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

    private async Task AddStandToContainerAsync()
    {
        await _containerService.AddStandToContainerAsync(
            _projectViewModel.CurrentProjectModel.CurrentProjectId,
            SelectedStandContainer.Id,
            SelectedStand.Id
        );

        StandsInProject.Remove(SelectedStand);
        
        SelectedStandContainer.Stands.Add(SelectedStand);

        UpdateAvailableStands();
        
        await RefreshBatchesDataAsync();
        
        SelectedStand = null;
    }

    private async Task RemoveStandFromContainerAsync()
    {
        await _containerService.RemoveStandFromContainerAsync(
            _projectViewModel.CurrentProjectModel.CurrentProjectId, 
            SelectedStandContainer.Id, 
            SelectedStand.Id);

        SelectedStandContainer.Stands.Remove(SelectedStand);
        
        await RefreshBatchesDataAsync();

        UpdateAvailableStands();
        
        SelectedStand = null;
    }

    private void UpdateAvailableStands()
    {
        var containerStandIds = SelectedStandContainer?.Stands?
                                    .Select(s => s.Id)
                                    .ToHashSet() 
                                ?? [];

        AvailableStands = StandsInProject
            .Where(s => !containerStandIds.Contains(s.Id))
            .ToObservable();
    }
    
    private async Task CreateReportAsync(
        ReportType typeGenerator,
        string reportName,
        List<Stand> selectedStands)
    {

        if (selectedStands == null || selectedStands.Count == 0)
        {
            _notificationService.ShowConfirmation("Стенды не выбраны!");
            return;
        }

        var kksDuplicates = selectedStands
            .GroupBy(stand => stand.KKSCode)
            .Where(group => group.Count() > 1)
            .ToList();

        if (kksDuplicates.Count > 0)
        {
            var warningMessage = "Обнаружены дублирования KKS-кодов стендов:\n\n" +
                                 string.Join("\n", kksDuplicates.Select(g => $"- {g.Key} ({g.Count()} шт.)")) +
                                 "\n\nПродолжить генерацию отчета?";

            var confirmationResult = _notificationService.ShowConfirmation(warningMessage);

            if (!confirmationResult)
            {
                _notificationService.ShowInfo("Генерация отчета отменена");
                return;
            }
        }

        if (typeGenerator == ReportType.TechnologicalCards)
        {
            var reportTypeWindow = new TechCardElecrticDialog
            {
                Owner = Application.Current.MainWindow
            };
            var dialogResult = reportTypeWindow.ShowDialog();

            if (dialogResult == true && reportTypeWindow.SelectedOption != TechCardElecticDialogResult.Cancel)
            {
                var includeElectric = reportTypeWindow.SelectedOption == TechCardElecticDialogResult.WithElectric;
                var reportSettings = _serviceProvider.GetRequiredService<ReportSettings>();
                reportSettings.TechCardIncludeElectric = includeElectric;
            }
            else
            {
                _notificationService.ShowInfo("Генерация отчета отменена");
                return;
            }
        }

        await _dialogService.RunWithProgressDialogAsync(() =>
            _reportService.GenerateReportAsync(
                typeGenerator, 
                _projectViewModel.CurrentProjectModel.CurrentProjectId, 
                selectedStands));

        if (_notificationService.ShowConfirmation(
                $"Отчёт \"{reportName}\" по выбранной партии создана!\nОткрыть папку с отчётами?"))
        {
            var reportDir = JsonHandler.GetSaveReportDirectory(DirectoryHelper.GetConfigPath());
            Process.Start("explorer.exe", reportDir);
        }
    }
    
    private async Task GenerateSelectedReportAsync()
    {
        if (SelectedBatch == null)
        {
            _notificationService.ShowInfo("Сначала выберите партию!");
            return;
        }
        
        var batchStands = SelectedBatch.Containers
            .SelectMany(container => container.Stands)
            .ToList();

        if (batchStands.Count == 0)
        {
            _notificationService.ShowInfo("Выбранная партия не содержит стендов!");
            return;
        }

        var reportName = _reportDisplayNames.TryGetValue(SelectedReportType, out var name) 
            ? name 
            : SelectedReportType.ToString();

        await CreateReportAsync(SelectedReportType, reportName, batchStands);
    }
}