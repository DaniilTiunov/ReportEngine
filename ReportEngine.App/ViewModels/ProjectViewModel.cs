using ReportEngine.App.AppHelpers;
using ReportEngine.App.Commands;
using ReportEngine.App.Model;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.ModelWrappers;
using ReportEngine.App.Services;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Armautre;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.ElectricSockets;
using ReportEngine.Domain.Entities.Pipes;
using ReportEngine.Domain.Repositories;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.IniHeleprs;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Data;

namespace ReportEngine.App.ViewModels;

public class ProjectViewModel : BaseViewModel
{
    private readonly ICalculationService _calculationService;
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
        ICalculationService calculationService)
    {
        _projectRepository = projectRepository;
        _dialogService = dialogService;
        _notificationService = notificationService;
        _standService = standService;
        _projectService = projectService;
        _projectDataLoaderService = projectDataLoaderService;
        _reportService = reportService;
        _calculationService = calculationService;

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

    public void InitializeTime()
    {
        CurrentProjectModel.CreationDate = DateTime.Now.Date;
        CurrentProjectModel.StartDate = DateTime.Now.Date;
        CurrentProjectModel.OutOfProduction = DateTime.Now.Date;
        CurrentProjectModel.EndDate = DateTime.Now.Date;
    }

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

    public void OnSelectObvCommandExecuted(object p)
    {
        ExceptionHelper.SafeExecute(() => { SelectedObvyazka = _dialogService.ShowObvyazkaDialog(); });
    }

    public async void OnCalculateProjectCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(CalculateProjectAsync);
    }

    public async void OnCreateSummaryReportCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await CreateReportAsync(ReportType.ComponentsListReport, "комплектующих");
        });
    }

    public async void OnCreateMarksReportCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await CreateReportAsync(ReportType.MarksReport, "маркировки");
        });
    }

    public async void OnCreateContainerReportCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await CreateReportAsync(ReportType.ContainerReport, "тара");
        });
    }

    private async void OnSaveChangesInStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(SaveChangesInStandAsync);
    }

    private async void OnDeleteElectricalComponentFromStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(DeleteElectricalComponentFromStandAsync);
    }

    private async void OnUpdateElectricalComponentInStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(UpdateElectricalPurposeAsync);
    }

    private async void OnDeleteAdditionalComponentFromStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(DeleteAdditionalComponentFromStandAsync);
    }

    private async void OnUpdateAdditionalComponentInStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(UpdateAdditionalPurposeAsync);
    }

    private async void OnDeleteDrainageComponentFromStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(DeleteDrainageComponentFromStandAsync);
    }

    private async void OnUpdateDrainageComponentInStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(UpdateDrainagePurposeAsync);
    }

    public void ResetProject()
    {
        CurrentProjectModel = new ProjectModel();
        CurrentStandModel = new StandModel();
        InitializeTime();
        OnPropertyChanged(nameof(CurrentProjectModel));
        OnPropertyChanged(nameof(CurrentStandModel));
    }

    // TODO: Сделать тут рефакторинг (дженерик метод или фабрику)

    #region Инициализация команд

    public void InitializeCommands()
    {
        ProjectCommandProvider.CreateNewCardCommand =
            new RelayCommand(OnCreateNewCardCommandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.AddNewStandCommand =
            new RelayCommand(OnAddNewStandCommandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.SaveChangesCommand =
            new RelayCommand(OnSaveChangesCommandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.AddFrameToStandCommand =
            new RelayCommand(OnAddFrameToStandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.AddDrainageToStandCommand =
            new RelayCommand(OnAddDrainageToStandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.AddCustomDrainageToStandCommand =
            new RelayCommand(OnAddCustomDrainageToStandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.AddCustomElectricalComponentToStandCommand =
            new RelayCommand(OnAddCustomElectricalComponentToStandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.AddCustomAdditionalEquipToStandCommand =
            new RelayCommand(OnAddCustomAdditionalEquipToStandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.SelectObvFromDialogCommand =
            new RelayCommand(OnSelectObvCommandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.CalculateProjectCommand =
            new RelayCommand(OnCalculateProjectCommandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.CreateSummaryReportCommand =
            new RelayCommand(OnCreateSummaryReportCommandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.OpenAllSortamentsDialogCommand =
            new RelayCommand(OnOpenAllSortamentsDialogExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.CreateMarkReportCommand =
            new RelayCommand(OnCreateMarksReportCommandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.DeleteSelectedStandCommand =
            new RelayCommand(OnDeleteSelectedStandFromProjectExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.RemoveObvFromStandCommand =
            new RelayCommand(OnRemoveObvCommandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.CreateContainerReportCommand =
            new RelayCommand(OnCreateContainerReportCommandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.SaveChangesInStandCommand =
            new RelayCommand(OnSaveChangesInStandCommandExecuted, CanAllCommandsExecute);

        ProjectCommandProvider.DeleteElectricalComponentFromStandCommand =
            new RelayCommand(OnDeleteElectricalComponentFromStandCommandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.UpdateElectricalComponentInStandCommand =
            new RelayCommand(OnUpdateElectricalComponentInStandCommandExecuted, CanAllCommandsExecute);

        ProjectCommandProvider.DeleteAdditionalComponentFromStandCommand =
            new RelayCommand(OnDeleteAdditionalComponentFromStandCommandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.UpdateAdditionalComponentInStandCommand =
            new RelayCommand(OnUpdateAdditionalComponentInStandCommandExecuted, CanAllCommandsExecute);

        ProjectCommandProvider.DeleteDrainageComponentFromStandCommand =
            new RelayCommand(OnDeleteDrainageComponentFromStandCommandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.UpdateDrainageComponentInStandCommand =
            new RelayCommand(OnUpdateDrainageComponentInStandCommandExecuted, CanAllCommandsExecute);

    }

    public void InitializeGenericCommands()
    {
        ProjectCommandProvider.SelectMaterialLineDialogCommand =
            new RelayCommand(OnSelectMaterialFromDialogCommandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.SelectArmatureDialogCommand =
            new RelayCommand(OnSelectArmatureFromDialogCommandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.SelectKMCHDialogCommand =
            new RelayCommand(OnSelectKMCHFromDialogCommandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.SelectTreeSocketDialogCommand =
            new RelayCommand(OnSelectTreeSocketFromDialogCommandExecuted, CanAllCommandsExecute);
        ProjectCommandProvider.SaveObvCommand =
            new RelayCommand(OnSaveObvCommandExecuted, CanAllCommandsExecute);
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
            OnPropertyChanged(nameof(CurrentStandModel));
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
        CurrentStandModel.InitializeDefaultPurposes();
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
        CurrentStandModel.InitializeDefaultPurposes();
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
        CurrentStandModel.InitializeDefaultPurposes();
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
                    CollectionViewSource.GetDefaultView(CurrentStandModel.NewDrainage.Purposes).Refresh();
                    return;

                case AdditionalEquipPurpose ap:
                    ap.Material = selected.Name;
                    ap.CostPerUnit = selected.Cost;
                    ap.Measure = selected.Measure;
                    CollectionViewSource.GetDefaultView(CurrentStandModel.NewAdditionalEquip.Purposes).Refresh();
                    return;

                case ElectricalPurpose ep:
                    ep.Material = selected.Name;
                    ep.CostPerUnit = selected.Cost;
                    ep.Measure = selected.Measure;
                    CollectionViewSource.GetDefaultView(CurrentStandModel.NewElectricalComponent.Purposes).Refresh();
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

    public async Task UpdateElectricalPurposeAsync()
    {
        var purpose = CurrentProjectModel.SelectedStand.SelectedElectricalComponent;

        await _standService.UpdateElectricalPurposeAsync(purpose);
        _notificationService.ShowInfo("Электрические компоненты сохранены");
    }

    private async Task DeleteElectricalComponentFromStandAsync()
    {
        var toRemove = CurrentProjectModel.SelectedStand.SelectedElectricalComponent;
        await _standService.DeleteElectricalPurposeAsync(CurrentProjectModel.SelectedStand.SelectedElectricalComponent.Id);
        CurrentProjectModel.SelectedStand.AllElectricalPurposesInStand.Remove(toRemove);

        _notificationService.ShowInfo("Электрический компонент удалён");
    }

    public async Task UpdateAdditionalPurposeAsync()
    {
        var purpose = CurrentProjectModel.SelectedStand.SelectedAdditionalEquip;

        await _standService.UpdateAdditionalPurposeAsync(purpose);
        _notificationService.ShowInfo("Доп. комплктующие сохранены");
    }

    private async Task DeleteAdditionalComponentFromStandAsync()
    {
        var toRemove = CurrentProjectModel.SelectedStand.SelectedAdditionalEquip;
        await _standService.DeleteAdditionalPurposeAsync(CurrentProjectModel.SelectedStand.SelectedElectricalComponent.Id);
        CurrentProjectModel.SelectedStand.AllAdditionalEquipPurposesInStand.Remove(toRemove);

        _notificationService.ShowInfo("Доп. комплктующее удалено");
    }

    public async Task UpdateDrainagePurposeAsync()
    {
        var purpose = CurrentProjectModel.SelectedStand.SelectedDrainagePurpose;

        await _standService.UpdateDrainagePurposeAsync(purpose);
        _notificationService.ShowInfo("Дренажные комплектующие сохранены");
    }

    private async Task DeleteDrainageComponentFromStandAsync()
    {
        var toRemove = CurrentProjectModel.SelectedStand.SelectedDrainagePurpose;
        await _standService.DeleteDrainagePurposeAsync(CurrentProjectModel.SelectedStand.SelectedDrainage.Id);
        CurrentProjectModel.SelectedStand.AllDrainagePurposesInStand.Remove(toRemove);

        _notificationService.ShowInfo("Дренажное комплектующее удалён");
    }

    #endregion

    #region Методы расчёта и создания отчётности

    private async Task CalculateProjectAsync()
    {
        await _calculationService.CalculateProjectAsync(CurrentProjectModel);
        OnPropertyChanged(nameof(CurrentProjectModel.Stands));
        OnPropertyChanged(nameof(CurrentProjectModel.Cost));
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