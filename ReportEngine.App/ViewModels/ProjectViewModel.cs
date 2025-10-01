using System.Collections.ObjectModel;
using System.Diagnostics;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.Commands.Initializers;
using ReportEngine.App.Commands.Providers;
using ReportEngine.App.Model;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.ModelWrappers;
using ReportEngine.App.Services.Core;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Armautre;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.ElectricSockets;
using ReportEngine.Domain.Entities.Pipes;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.IniHeleprs;

namespace ReportEngine.App.ViewModels;

public class ProjectViewModel : BaseViewModel
{
    private readonly ICalculationService _calculationService;
    private readonly IContainerRepository _containerRepository;
    private readonly ContainerService _containerService;
    private readonly IDialogService _dialogService;
    private readonly INotificationService _notificationService;
    private readonly IProjectDataLoaderService _projectDataLoaderService;
    private readonly IProjectInfoRepository _projectRepository;
    private readonly IProjectService _projectService;
    private readonly IReportService _reportService;
    private readonly IStandService _standService;

    public ProjectViewModel(IProjectInfoRepository projectRepository,
        IDialogService dialogService,
        INotificationService notificationService,
        IStandService standService,
        IProjectService projectService,
        IProjectDataLoaderService projectDataLoaderService,
        IReportService reportService,
        ICalculationService calculationService,
        ContainerService containerService)
    {
        _projectRepository = projectRepository;
        _dialogService = dialogService;
        _notificationService = notificationService;
        _standService = standService;
        _projectService = projectService;
        _projectDataLoaderService = projectDataLoaderService;
        _reportService = reportService;
        _calculationService = calculationService;
        _containerService = containerService;

        InitializeCommands();
        InitializeTime();
        InitializeGenericCommands();
    }

    public ObservableCollection<FormedFrame> AllAvailableFrames { get; set; } = new();
    public ObservableCollection<FormedDrainage> AllAvailableDrainages { get; set; } = new();
    public ObservableCollection<FormedElectricalComponent> AllAvailableElectricalComponents { get; set; } = new();
    public ObservableCollection<FormedAdditionalEquip> AllAvailableAdditionalEquips { get; set; } = new();
    public Obvyazka SelectedObvyazka { get; set; } = new();
    public StandModel CurrentStandModel { get; set; } = new();
    public ProjectModel CurrentProjectModel { get; set; } = new();
    public ProjectCommandProvider ProjectCommandProvider { get; set; } = new();
    public MaterialLinesModel CurrentMaterials { get; set; } = new();

    public bool CanAllCommandsExecute(object? e)
    {
        return true;
    }

    public void OnOpenAllSortamentsDialogExecuted(object e)
    {
        // e приходит из XAML CommandParameter — это объект назначения (DrainagePurpose / AdditionalEquipPurpose / ElectricalPurpose)
        var selected = _dialogService.ShowAllSortamentsDialog();
        if (selected == null) return;

        ApplySelectedEquipToPurpose(e, selected);
    }

    // TODO: Сделать тут рефакторинг команд
    public void OnSelectMaterialFromDialogCommandExecuted(object e)
    {
        switch (CurrentMaterials.SelectedMaterialLine)
        {
            case "Жаропрочные":
                SelectEquipment<HeaterPipe>(
                    name => CurrentProjectModel.SelectedStand.MaterialLine = name,
                    measure => CurrentProjectModel.SelectedStand.MaterialLineMeasure = measure);
                break;
            case "Нержавеющие":
                SelectEquipment<StainlessPipe>(
                    name => CurrentProjectModel.SelectedStand.MaterialLine = name,
                    measure => CurrentProjectModel.SelectedStand.MaterialLineMeasure = measure);
                break;
            case "Углеродистые":
                SelectEquipment<CarbonPipe>(
                    name => CurrentProjectModel.SelectedStand.MaterialLine = name,
                    measure => CurrentProjectModel.SelectedStand.MaterialLineMeasure = measure);
                break;
        }
    }

    public void OnSelectArmatureFromDialogCommandExecuted(object e)
    {
        switch (CurrentMaterials.SelectedAramuteres)
        {
            case "Жаропрочные":
                SelectEquipment<HeaterArmature>(
                    name => CurrentProjectModel.SelectedStand.Armature = name,
                    measure => CurrentProjectModel.SelectedStand.ArmatureMeasure = measure);
                break;
            case "Нержавеющие":
                SelectEquipment<StainlessArmature>(
                    name => CurrentProjectModel.SelectedStand.Armature = name,
                    measure => CurrentProjectModel.SelectedStand.ArmatureMeasure = measure);
                break;
            case "Углеродистые":
                SelectEquipment<CarbonArmature>(
                    name => CurrentProjectModel.SelectedStand.Armature = name,
                    measure => CurrentProjectModel.SelectedStand.ArmatureMeasure = measure);
                break;
        }
    }

    public void OnSelectTreeSocketFromDialogCommandExecuted(object e)
    {
        switch (CurrentMaterials.SelectedSocketTypes)
        {
            case "Жаропрочные":
                SelectEquipment<HeaterSocket>(
                    name => CurrentProjectModel.SelectedStand.TreeSocket = name,
                    measure => CurrentProjectModel.SelectedStand.TreeSocketMaterialMeasure = measure);
                break;
            case "Нержавеющие":
                SelectEquipment<StainlessSocket>(
                    name => CurrentProjectModel.SelectedStand.TreeSocket = name,
                    measure => CurrentProjectModel.SelectedStand.TreeSocketMaterialMeasure = measure);
                break;
            case "Углеродистые":
                SelectEquipment<CarbonSocket>(
                    name => CurrentProjectModel.SelectedStand.TreeSocket = name,
                    measure => CurrentProjectModel.SelectedStand.TreeSocketMaterialMeasure = measure);
                break;
        }
    }

    public void OnSelectKMCHFromDialogCommandExecuted(object e)
    {
        switch (CurrentMaterials.SelectedKMCHType)
        {
            case "Жаропрочные":
                SelectEquipment<HeaterSocket>(
                    name => CurrentProjectModel.SelectedStand.KMCH = name,
                    measure => CurrentProjectModel.SelectedStand.KMCHMeasure = measure);
                break;
            case "Нержавеющие":
                SelectEquipment<StainlessSocket>(
                    name => CurrentProjectModel.SelectedStand.KMCH = name,
                    measure => CurrentProjectModel.SelectedStand.KMCHMeasure = measure);
                break;
            case "Углеродистые":
                SelectEquipment<CarbonSocket>(
                    name => CurrentProjectModel.SelectedStand.KMCH = name,
                    measure => CurrentProjectModel.SelectedStand.KMCHMeasure = measure);
                break;
        }
    }

    public async void OnCreateNewCardCommandExecuted(object? e)
    {
        await ExceptionHelper.SafeExecuteAsync(CreateNewProjectCardAsync);
    }

    public async void OnAddNewStandCommandExecuted(object? e)
    {
        await ExceptionHelper.SafeExecuteAsync(AddNewStandToProjectAsync);
    }

    // TODO: Сделать красивое окно
    public async void OnCopyStandsCommandExecuted(object? e)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _projectService.CopyStandsAsync(CurrentProjectModel);
            
            await LoadPurposesInStandsAsync();
            await LoadObvyazkiAsync();
        });
    }

    public async void OnDeleteSelectedStandFromProjectExecuted(object? e)
    {
        await ExceptionHelper.SafeExecuteAsync(DeleteStandFromProject);
    }

    public async void OnSaveChangesCommandExecuted(object? e)
    {
        await ExceptionHelper.SafeExecuteAsync(SaveProjectChangesAsync);
    }

    public async void OnSaveObvCommandExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(AddObvToStandAsync);
    }

    public async void OnRemoveObvCommandExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(DeleteObvFromStandAsync);
    }

    public async void OnRemoveFrameFromStandCommandExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _projectService.DeleteFrameFromStandAsync(CurrentProjectModel);

            _notificationService.ShowInfo("Рама удалена из стенда");
        });
    }

    public async void OnAddCustomDrainageToStandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(AddCustomDrainageToStandAsync);
    }

    public async void OnAddDrainageToStandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(AddDrainageToStandAsync);
    }

    public async void OnAddFrameToStandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(AddFrameToStandAsync);
    }

    public async void OnAddCustomElectricalComponentToStandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(AddCustomElectricalComponentToStandAsync);
    }

    public async void OnAddCustomAdditionalEquipToStandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(AddCustomAdditionalEquipToStandAsync);
    }

    public async void OnCopyObvyazkaToStandsCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var sourceObv = CurrentProjectModel.SelectedObvyazkaToCopy;
            var standId = CurrentProjectModel.SelectedStand.Id;

            var newObvyazka = ObvyzkaModelWrapper.CloneForStand(sourceObv, standId);

            await _standService.AddObvyazkaToStandAsync(standId, newObvyazka);

            OnPropertyChanged(nameof(CurrentProjectModel.SelectedStand.ObvyazkiInStand));
            OnPropertyChanged(nameof(CurrentProjectModel.ObvyazkiInProject));

            await LoadObvyazkiAsync();

            _notificationService.ShowInfo("Обвязка скопирована в стенд");
        });
    }

    // TODO: Вынести в отдельный класс
    public void OnSelectObvCommandExecuted(object p)
    {
        ExceptionHelper.SafeExecute(() =>
        {
            SelectedObvyazka = _dialogService.ShowObvyazkaDialog();

            var stand = CurrentProjectModel.SelectedStand;

            var tmp = stand.SelectedObvyazkaInStand;

            tmp.ImageName = SelectedObvyazka.ImageName;

            stand.SelectedObvyazkaInStand = null;
            stand.SelectedObvyazkaInStand = tmp;
        });
    }

    public async void OnCalculateProjectCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(CalculateProjectAsync);
    }

    public async void OnComponentsListReportCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await CreateReportAsync(ReportType.ComponentsListReport, "комплектующих");
        });
    }
    
    public async void OnCreateSummaryReportCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await CreateReportAsync(ReportType.SummaryReport, "сводная");
        });
    }

    public async void OnCreateMarksReportCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await CreateReportAsync(ReportType.MarksReport, "маркировки");
        });
    }
    
    
    public async Task OnCreateMarksReportAsyncCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await CreateReportAsync(ReportType.MarksReport, "маркировки");
        });
    }

    public async void OnCreateNameplatesReportCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await CreateReportAsync(ReportType.NameplatesReport, "шильдики");
        });
    }

    public async void OnCreateContainerReportCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await CreateReportAsync(ReportType.ContainerReport, "тара");
        });
    }

    public async void OnCreateProductionReportCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await CreateReportAsync(ReportType.ProductionReport, "производство");
        });
    }

    public async void OnCreateFinplanReportCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await CreateReportAsync(ReportType.FinPlanReport, "финплан");
        });
    }

    public async void OnCreatePassportReportCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await CreateReportAsync(ReportType.PassportsReport, "паспорт");
        });
    }

    public async void OnSaveChangesInStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(SaveChangesInStandAsync);
    }

    public async void OnDeleteElectricalComponentFromStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await DeletePurposeAsync(CurrentProjectModel.SelectedStand.SelectedElectricalComponent,
                _standService.DeleteElectricalPurposeAsync,
                CurrentProjectModel.SelectedStand.AllElectricalPurposesInStand,
                "Электрический компонент удалён");
        });
    }

    public async void OnUpdateElectricalComponentInStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var selectedPurpose = CurrentProjectModel.SelectedStand.SelectedElectricalComponent;

            if (selectedPurpose.Id == 0)
            {
                var selectedComponent = CurrentProjectModel.SelectedStand.ElectricalComponentsInStand.FirstOrDefault();
                if (selectedComponent != null)
                {
                    selectedPurpose.FormedElectricalComponentId = selectedComponent.Id;
                }
                else
                {
                    _notificationService.ShowError("Нет электрического компонента для назначения.");
                    return;
                }
            }

            await UpdatePurposeAsync(selectedPurpose,
                _standService.UpdateElectricalPurposeAsync,
                "Электрические компоненты сохранены");
        });
    }

    public async void OnDeleteAdditionalComponentFromStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await DeletePurposeAsync(CurrentProjectModel.SelectedStand.SelectedAdditionalEquip,
                _standService.DeleteAdditionalPurposeAsync,
                CurrentProjectModel.SelectedStand.AllAdditionalEquipPurposesInStand,
                "Доп. комплектующее удалено");
        });
    }

    public async void OnUpdateAdditionalComponentInStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var selectedPurpose = CurrentProjectModel.SelectedStand.SelectedAdditionalEquip;

            if (selectedPurpose.Id == 0)
            {
                var selectedComponent = CurrentProjectModel.SelectedStand.AdditionalEquipsInStand.FirstOrDefault();
                if (selectedComponent != null)
                {
                    selectedPurpose.FormedAdditionalEquipId = selectedComponent.Id;
                }
                else
                {
                    _notificationService.ShowError("Нет дополнительного компонента для назначения.");
                    return;
                }
            }

            await UpdatePurposeAsync(CurrentProjectModel.SelectedStand.SelectedAdditionalEquip,
                _standService.UpdateAdditionalPurposeAsync,
                "Доп. комплектующие сохранены");
        });
    }

    public async void OnDeleteDrainageComponentFromStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await DeletePurposeAsync(CurrentProjectModel.SelectedStand.SelectedDrainagePurpose,
                _standService.DeleteDrainagePurposeAsync,
                CurrentProjectModel.SelectedStand.AllDrainagePurposesInStand,
                "Дренажное комплектующее удалено");
        });
    }

    public async void OnUpdateDrainageComponentInStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var selectedPurpose = CurrentProjectModel.SelectedStand.SelectedDrainagePurpose;

            if (selectedPurpose.Id == 0)
            {
                var selectedComponent = CurrentProjectModel.SelectedStand.DrainagesInStand.FirstOrDefault();
                if (selectedComponent != null)
                {
                    selectedPurpose.FormedDrainageId = selectedComponent.Id;
                }
                else
                {
                    _notificationService.ShowError("Нет дренажа для назначения.");
                    return;
                }
            }

            await UpdatePurposeAsync(CurrentProjectModel.SelectedStand.SelectedDrainagePurpose,
                _standService.UpdateDrainagePurposeAsync,
                "Дренажное комплектующее сохранено");
        });
    }

    public async void OnFillStandFieldsFromObvyazkaCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            _standService.FillStandFieldsFromObvyazka(CurrentProjectModel.SelectedStand,
                CurrentProjectModel.SelectedStand.SelectedObvyazkaInStand);
        });
    }

    public async void OnUpdateObvInStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _projectService.UpdateObvInStandAsync(CurrentProjectModel, SelectedObvyazka);
        });
    }

    public async void OnCreateContainerStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
            await _containerService.CreateBatchAsync(CurrentProjectModel));
    }

    public async void OnDeleteBatchCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () => _containerService.DeleteBanchAsync(CurrentProjectModel));
    }

    public async void OnRefreshBatchesCommandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () => _containerService.LoadBatchesAsync(CurrentProjectModel));
    }

    public async void OnAddContainerToBatchCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
            _containerService.AddContainerToBatchAsync(CurrentProjectModel));
    }

    public async void OnDeleteContainerCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
            _containerService.RemoveContainerFromBatchAsync(CurrentProjectModel));
    }

    public async void OnAddStandToContainerCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
            _containerService.AddStandToContainerAsync(CurrentProjectModel));
    }

    public async void OnRemoveStandFromContainerCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
            _containerService.RemoveStandFromContainerAsync(CurrentProjectModel));
    }

    public void ResetProject()
    {
        CurrentProjectModel = new ProjectModel();
        CurrentStandModel = new StandModel();
        InitializeTime();
        OnPropertyChanged(nameof(CurrentProjectModel));
        OnPropertyChanged(nameof(CurrentStandModel));
    }

    #region Инициализация

    public void InitializeTime()
    {
        CurrentProjectModel.CreationDate = DateTime.Now.Date;
        CurrentProjectModel.StartDate = DateTime.Now.Date;
        CurrentProjectModel.OutOfProduction = DateTime.Now.Date;
        CurrentProjectModel.EndDate = DateTime.Now.Date;
    }

    public void InitializeCommands()
    {
        ProjectCommandsInitializer.InitializeCommands(this);
    }

    public void InitializeGenericCommands()
    {
        ProjectCommandsInitializer.InitializeGenericCommands(this);
    }

    #endregion

    #region Методы загрузки данных на view

    public async Task LoadStandsDataAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _standService.LoadStandsDataAsync(CurrentProjectModel.Stands);
        });
    }

    public async Task LoadPurposesInStandsAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(() =>
        {
            _standService.LoadPurposesInStands(CurrentProjectModel.Stands);
            return Task.CompletedTask;
        });
    }

    public async Task LoadObvyazkiAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _standService.LoadObvyazkiInStandsAsync(CurrentProjectModel.Stands);
            await _projectService.LoadAllObvyazkiInProject(CurrentProjectModel);
        });
    }

    public async Task LoadAllAvaileDataAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _projectDataLoaderService.LoadAllAvailDataToViewModel(this);
        });
    }

    public async Task LoadProjectInfoAsync(int projectId)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var loadedModel = await _projectService.LoadProjectInfoAsync(projectId);
            if (loadedModel == null)
                return;

            CurrentProjectModel = loadedModel;
            CurrentStandModel = loadedModel.SelectedStand ?? new StandModel();
            CurrentStandModel.InitializeDefaultPurposes();

            await LoadObvyazkiAsync();
            await LoadStandsDataAsync();

            OnPropertyChanged(nameof(CurrentStandModel));
        });
    }

    public async Task LoadContainersInfoAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _containerService.LoadAllData(CurrentProjectModel);
        });
    }

    #endregion

    #region Методы для CRUD с проектами и стендами

    private async Task AddObvToStandAsync()
    {
        if (CurrentProjectModel.SelectedStand == null)
            return;

        var entity = await _standService.CreateObvyazkaAsync(CurrentProjectModel.SelectedStand, SelectedObvyazka);

        await _standService.AddObvyazkaToStandAsync(CurrentProjectModel.SelectedStand.Id, entity);

        CurrentProjectModel.SelectedStand.ObvyazkiInStand.Add(entity);
    }

    private async Task DeleteObvFromStandAsync()
    {
        var stand = CurrentProjectModel?.SelectedStand;
        var selectedObv = stand?.SelectedObvyazkaInStand;

        if (stand == null || selectedObv == null)
        {
            _notificationService.ShowInfo("Обвязка или стенд не выбран");
            return;
        }

        var standId = stand.Id;
        var obvId = selectedObv.Id;

        try
        {
            await _projectService.DeleteObvFromStandAsync(standId, obvId);
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Не удалось удалить обвязку: {ex.Message}");
            return;
        }

        var toRemove = stand.ObvyazkiInStand?.FirstOrDefault(o => o.Id == obvId);
        if (toRemove != null)
            stand.ObvyazkiInStand.Remove(toRemove);

        stand.SelectedObvyazkaInStand = null;

        _notificationService.ShowInfo("Обвязка удалена из стенда");
    }

    private async Task SaveProjectChangesAsync()
    {
        if (CurrentProjectModel.CurrentProjectId == 0)
        {
            _notificationService.ShowInfo("Сначала создайте проект");
            return;
        }

        await _projectService.UpdateProjectAsync(CurrentProjectModel);

        _notificationService.ShowInfo("Изменения успешно сохранены!");
    }

    private async Task AddNewStandToProjectAsync()
    {
        if (CurrentProjectModel.CurrentProjectId == 0)
        {
            _notificationService.ShowInfo("Сначала создайте проект");
            return;
        }

        var newStandModel = new StandModel
        {
            KKSCode = CurrentStandModel.KKSCode,
            Design = CurrentStandModel.Design,
            BraceType = CurrentStandModel.BraceType,
            Devices = CurrentStandModel.Devices,
            Width = CurrentStandModel.Width,
            SerialNumber = CurrentStandModel.SerialNumber,
            Weight = CurrentStandModel.Weight,
            StandSummCost = CurrentStandModel.StandSummCost,
            NN = CurrentStandModel.NN,
            MaterialLine = CurrentStandModel.MaterialLine,
            Armature = CurrentStandModel.Armature,
            TreeSocket = CurrentStandModel.TreeSocket,
            KMCH = CurrentStandModel.KMCH,
            FirstSensorType = CurrentStandModel.FirstSensorType,
            ProjectId = CurrentProjectModel.CurrentProjectId
        };

        var newStandEntity = StandDataConverter.ConvertToStandEntity(newStandModel);
        var addedStandEntity =
            await _projectRepository.AddStandAsync(CurrentProjectModel.CurrentProjectId, newStandEntity);

        newStandModel.Id = addedStandEntity.Id;
        newStandModel.ProjectId = addedStandEntity.ProjectInfoId;

        newStandModel.InitializeDefaultPurposes();

        CurrentProjectModel.Stands.Add(newStandModel);
        CurrentProjectModel.SelectedStand = newStandModel;

        _notificationService.ShowInfo($"Стенд успешно добавлен! {addedStandEntity.Id}");
    }

    private async Task SaveChangesInStandAsync()
    {
        if (CurrentProjectModel.CurrentProjectId == 0)
        {
            _notificationService.ShowInfo("Сначала создайте проект");
            return;
        }

        var stand = CurrentProjectModel.SelectedStand;
        if (stand == null)
        {
            _notificationService.ShowInfo("Стенд не выбран");
            return;
        }

        var standEntity = StandDataConverter.ConvertToStandEntity(stand);
        await _projectRepository.UpdateStandAsync(standEntity);

        _notificationService.ShowInfo("Изменения стенда сохранены");
    }

    private async Task DeleteStandFromProject()
    {
        await _projectService.DeleteStandAsync(CurrentProjectModel.CurrentProjectId,
            CurrentProjectModel.SelectedStand.Id);
        CurrentProjectModel.Stands.Remove(CurrentProjectModel.SelectedStand);
    }

    private async Task CreateNewProjectCardAsync()
    {
        await _projectService.CreateProjectAsync(CurrentProjectModel);

        CurrentProjectModel.Stands.Clear();
        CurrentStandModel = new StandModel();
    }

    private void SelectEquipment<T>(Action<string> setProperty, Action<string> setMeasure)
        where T : class, IBaseEquip, new()
    {
        ExceptionHelper.SafeExecute(() =>
        {
            var equipment = _dialogService.ShowEquipDialog<T>();
            if (equipment != null && CurrentProjectModel.SelectedStand != null)
            {
                setProperty(equipment.Name);
                setMeasure(equipment.Measure);
            }
        });
    }

    private async Task AddFrameToStandAsync()
    {
        if (CurrentStandModel.SelectedFrame != null)
        {
            await _standService.AddFrameToStandAsync(
                CurrentProjectModel.SelectedStand.Id,
                CurrentStandModel.SelectedFrame.Id
            );

            CurrentProjectModel.SelectedStand.FramesInStand.Add(CurrentStandModel.SelectedFrame);
        }
    }

    private async Task AddDrainageToStandAsync()
    {
        if (CurrentStandModel.SelectedDrainage != null)
        {
            await _standService.AddDrainageToStandAsync(
                CurrentProjectModel.SelectedStand.Id,
                CurrentStandModel.SelectedDrainage.Id);

            CurrentProjectModel.SelectedStand.DrainagesInStand.Add(CurrentStandModel.SelectedDrainage);
        }
    }

    private async Task AddCustomDrainageToStandAsync()
    {
        await _standService.AddCustomDrainageAsync(
            CurrentProjectModel.SelectedStand.Id,
            CurrentStandModel.NewDrainage);

        AllAvailableDrainages.Add(CurrentStandModel.NewDrainage);
        CurrentProjectModel.SelectedStand.DrainagesInStand.Add(CurrentStandModel.NewDrainage);

        OnPropertyChanged(nameof(AllAvailableDrainages));

        CurrentStandModel.NewDrainage = new FormedDrainage();
        CurrentStandModel.InitializeDrainagePurposes();

        await LoadPurposesInStandsAsync();
    }

    private async Task AddCustomElectricalComponentToStandAsync()
    {
        await _standService.AddCustomElectricalComponentAsync(
            CurrentProjectModel.SelectedStand.Id,
            CurrentStandModel.NewElectricalComponent);

        AllAvailableElectricalComponents.Add(CurrentStandModel.NewElectricalComponent);
        CurrentProjectModel.SelectedStand.ElectricalComponentsInStand.Add(CurrentStandModel.NewElectricalComponent);

        OnPropertyChanged(nameof(AllAvailableElectricalComponents));
        CurrentStandModel.NewElectricalComponent = new FormedElectricalComponent();
        CurrentStandModel.InitializeElectricalComponent();

        await LoadPurposesInStandsAsync();
    }

    private async Task AddCustomAdditionalEquipToStandAsync()
    {
        await _standService.AddCustomAdditionalEquipAsync(
            CurrentProjectModel.SelectedStand.Id,
            CurrentStandModel.NewAdditionalEquip);

        AllAvailableAdditionalEquips.Add(CurrentStandModel.NewAdditionalEquip);
        CurrentProjectModel.SelectedStand.AdditionalEquipsInStand.Add(CurrentStandModel.NewAdditionalEquip);

        OnPropertyChanged(nameof(AllAvailableAdditionalEquips));
        CurrentStandModel.NewAdditionalEquip = new FormedAdditionalEquip();
        CurrentStandModel.InitializeAdditionalEquip();

        await LoadPurposesInStandsAsync();
    }

    public async Task UpdateStandBlueprintAsync(byte[] imageData, string imageType)
    {
        if (CurrentProjectModel.SelectedStand == null || CurrentProjectModel == null)
            return;

        CurrentProjectModel.SelectedStand.ImageData = imageData;
        CurrentProjectModel.SelectedStand.ImageType = imageType;

        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _projectService.UpdateStandEntity(CurrentProjectModel);
            _notificationService.ShowInfo("Чертёж стенда сохранён");
        });
    }

    private void ApplySelectedEquipToPurpose(object target, IBaseEquip selected)
    {
        ExceptionHelper.SafeExecute(() =>
        {
            if (target == null || selected == null) return;

            switch (target)
            {
                case DrainagePurpose dp:
                    dp.Material = selected.Name;
                    dp.CostPerUnit = selected.Cost;
                    dp.Measure = selected.Measure;
                    CollectionRefreshHelper.SafeRefreshCollection(CurrentStandModel.NewDrainage.Purposes);
                    CollectionRefreshHelper.SafeRefreshCollection(CurrentProjectModel.SelectedStand
                        .AllDrainagePurposesInStand);
                    CollectionRefreshHelper.SafeRefreshCollection(CurrentProjectModel.SelectedStand
                        .DrainagesInStand);
                    return;

                case AdditionalEquipPurpose ap:
                    ap.Material = selected.Name;
                    ap.CostPerUnit = selected.Cost;
                    ap.Measure = selected.Measure;
                    CollectionRefreshHelper.SafeRefreshCollection(CurrentStandModel.NewAdditionalEquip.Purposes);
                    CollectionRefreshHelper.SafeRefreshCollection(CurrentProjectModel.SelectedStand
                        .AllAdditionalEquipPurposesInStand);
                    CollectionRefreshHelper.SafeRefreshCollection(CurrentProjectModel.SelectedStand
                        .AdditionalEquipsInStand);
                    return;

                case ElectricalPurpose ep:
                    ep.Material = selected.Name;
                    ep.CostPerUnit = selected.Cost;
                    ep.Measure = selected.Measure;
                    CollectionRefreshHelper.SafeRefreshCollection(CurrentStandModel.NewElectricalComponent.Purposes);
                    CollectionRefreshHelper.SafeRefreshCollection(CurrentProjectModel.SelectedStand
                        .AllElectricalPurposesInStand);
                    CollectionRefreshHelper.SafeRefreshCollection(CurrentProjectModel.SelectedStand
                        .ElectricalComponentsInStand);
                    return;

                case ContainerStand cs:
                    cs.Name = selected.Name;
                    CollectionRefreshHelper.SafeRefreshCollection(CurrentProjectModel.ContainerStandsInSelectedBatch);
                    return;
            }

            var t = target.GetType();
            var matProp = t.GetProperty("Material");
            var costProp = t.GetProperty("CostPerUnit");
            var measureProp = t.GetProperty("Measure");
            if (matProp != null && matProp.CanWrite) matProp.SetValue(target, selected.Name);
            if (costProp != null && costProp.CanWrite) costProp.SetValue(target, selected.Cost);
        });
    }

    private async Task UpdatePurposeAsync<T>(T? purpose,
        Func<T, Task> updateFunc,
        string successMessage)
        where T : class, IPurposeEntity
    {
        if (purpose is null)
            return;

        await updateFunc(purpose);
        _notificationService.ShowInfo(successMessage);
    }

    private async Task DeletePurposeAsync<T>(T? purpose,
        Func<int, Task> deleteFunc,
        ICollection<T> collection,
        string successMessage)
        where T : class, IPurposeEntity
    {
        if (purpose is null)
            return;

        await deleteFunc(purpose.Id);
        collection.Remove(purpose);
        _notificationService.ShowInfo(successMessage);
    }

    #endregion

    #region Методы расчёта и создания отчётности

    private async Task CalculateProjectAsync()
    {
        await _calculationService.CalculateProjectAsync(CurrentProjectModel);
        OnPropertyChanged(nameof(CurrentProjectModel.Stands));
        OnPropertyChanged(nameof(CurrentProjectModel.Cost));

        _notificationService.ShowInfo("Расчёт завершён");
    }

    private async Task CreateReportAsync(ReportType typeGenerator, string reportName)
    {
        await _reportService.GenerateReportAsync(typeGenerator, CurrentProjectModel.CurrentProjectId);

        if (_notificationService.ShowConfirmation($"Ведомость {reportName} создана!\nОткрыть папку с отчётами?"))
        {
            var reportDir = SettingsManager.GetReportDirectory();
            Process.Start("explorer.exe", reportDir);
        }
    }

    #endregion
}