using System.Collections.ObjectModel;
using ReportEngine.App.Commands;
using ReportEngine.App.Model;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.ModelWrappers;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Armautre;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.ElectricSockets;
using ReportEngine.Domain.Entities.Pipes;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;

namespace ReportEngine.App.ViewModels;

public class ProjectViewModel : BaseViewModel
{
    private readonly IDialogService _dialogService;
    private readonly INotificationService _notificationService;
    private readonly IProjectDataLoaderService _projectDataLoaderService;
    private readonly IProjectInfoRepository _projectRepository;
    private readonly IProjectService _projectService;
    private readonly IStandService _standService;

    public ProjectViewModel(IProjectInfoRepository projectRepository,
        IDialogService dialogService,
        INotificationService notificationService,
        IStandService standService,
        IProjectService projectService,
        IProjectDataLoaderService projectDataLoaderService)
    {
        _projectRepository = projectRepository;
        _dialogService = dialogService;
        _notificationService = notificationService;
        _standService = standService;
        _projectService = projectService;
        _projectDataLoaderService = projectDataLoaderService;

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
    }

    public void InitializeGenericCommands()
    {
        ProjectCommandProvider.SelectMaterialLineDialogCommand =
            new RelayCommand(OnSelectMaterialFromDialogCommandExecuted<HeaterPipe>, CanAllCommandsExecute);
        ProjectCommandProvider.SelectArmatureDialogCommand =
            new RelayCommand(OnSelectArmatureFromDialogCommandExecuted<HeaterArmature>, CanAllCommandsExecute);
        ProjectCommandProvider.SelectKMCHDialogCommand =
            new RelayCommand(OnSelectTreeSocketFromDialogCommandExecuted<HeaterSocket>, CanAllCommandsExecute);
        ProjectCommandProvider.SaveObvCommand = new RelayCommand(OnSaveObvCommandExecuted, CanAllCommandsExecute);
    }

    #endregion

    #region Команды

    public bool CanAllCommandsExecute(object e)
    {
        return true;
    }

    public void OnSelectMaterialFromDialogCommandExecuted<T>(object e)
        where T : class, IBaseEquip, new()
    {
        SelectEquipment<T>(name => CurrentProjectModel.SelectedStand.MaterialLine = name);
    }

    public void OnSelectArmatureFromDialogCommandExecuted<T>(object e)
        where T : class, IBaseEquip, new()
    {
        SelectEquipment<T>(name => CurrentProjectModel.SelectedStand.Armature = name);
    }

    public void OnSelectTreeSocketFromDialogCommandExecuted<T>(object e)
        where T : class, IBaseEquip, new()
    {
        SelectEquipment<T>(name => CurrentProjectModel.SelectedStand.TreeSocket = name);
    }

    public async void OnCreateNewCardCommandExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(CreateNewProjectCardAsync);
    }

    public async void OnAddNewStandCommandExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(AddNewStandToProjectAsync);
    }

    public async void OnSaveChangesCommandExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(SaveProjectChangesAsync);
    }

    public async void OnSaveObvCommandExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(AddObvToStandAsync);
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

    #endregion

    #region Методы

    public void ResetProject()
    {
        CurrentProjectModel = new ProjectModel();
        CurrentStandModel = new StandModel();
        InitializeTime();
        OnPropertyChanged(nameof(CurrentProjectModel));
        OnPropertyChanged(nameof(CurrentStandModel));
    }

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
            OnPropertyChanged(nameof(CurrentStandModel));
        });
    }


    private async Task AddObvToStandAsync()
    {
        if (CurrentProjectModel.SelectedStand == null)
            return;

        var entity = await CreateObvyazkaAsync(CurrentProjectModel, SelectedObvyazka);

        await _standService.AddObvyazkaToStandAsync(CurrentProjectModel.SelectedStand.Id, entity);
        CurrentProjectModel.SelectedStand.ObvyazkiInStand.Add(entity);
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

        CurrentProjectModel.Stands.Add(newStandModel);
        CurrentProjectModel.SelectedStand = newStandModel;

        _notificationService.ShowInfo($"Стенд успешно добавлен! {addedStandEntity.Id}");
    }

    private async Task CreateNewProjectCardAsync()
    {
        await _projectService.CreateProjectAsync(CurrentProjectModel);

        CurrentProjectModel.Stands.Clear();
        CurrentStandModel = new StandModel();
    }

    private void SelectEquipment<T>(Action<string> setProperty)
        where T : class, IBaseEquip, new()
    {
        ExceptionHelper.SafeExecute(() =>
        {
            var equipment = _dialogService.ShowEquipDialog<T>();
            if (equipment != null && CurrentProjectModel.SelectedStand != null) setProperty(equipment.Name);
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
    }

    private async Task<ObvyazkaInStand> CreateObvyazkaAsync(ProjectModel project, Obvyazka selectedObvyazka)
    {
        return new ObvyazkaInStand
        {
            LineLength = selectedObvyazka.LineLength,
            ZraCount = selectedObvyazka.ZraCount,
            Sensor = selectedObvyazka.Sensor,
            SensorType = selectedObvyazka.SensorType,
            Clamp = selectedObvyazka.Clamp,
            WidthOnFrame = selectedObvyazka.WidthOnFrame,
            OtherLineCount = selectedObvyazka.OtherLineCount,
            Weight = selectedObvyazka.Weight,
            TreeSocketCount = selectedObvyazka.TreeSocket,
            HumanCost = selectedObvyazka.HumanCost,
            ImageName = selectedObvyazka.ImageName,
            ObvyazkaId = selectedObvyazka.Id,

            ObvyazkaName = CurrentProjectModel.SelectedStand.ObvyazkaName,
            StandId = CurrentProjectModel.SelectedStand.Id,
            NN = CurrentProjectModel.SelectedStand.NN,
            MaterialLine = CurrentProjectModel.SelectedStand.MaterialLine,
            Armature = CurrentProjectModel.SelectedStand.Armature,
            TreeSocket = CurrentProjectModel.SelectedStand.TreeSocket,
            KMCH = CurrentProjectModel.SelectedStand.KMCH,
            FirstSensorType = CurrentProjectModel.SelectedStand.FirstSensorType,
            FirstSensorKKS = CurrentProjectModel.SelectedStand.FirstSensorKKSCounter,
            FirstSensorMarkPlus = CurrentProjectModel.SelectedStand.FirstSensorMarkPlus,
            FirstSensorMarkMinus = CurrentProjectModel.SelectedStand.FirstSensorMarkMinus
        };
    }

    #endregion
}