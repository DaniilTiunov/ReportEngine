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
using ReportEngine.Domain.Enums;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;

namespace ReportEngine.App.ViewModels;

public class ProjectViewModel : BaseViewModel
{
    private readonly IDialogService _dialogService;
    private readonly INotificationService _notificationService;
    private readonly IProjectInfoRepository _projectRepository;
    private readonly IStandService _standService;

    public ProjectViewModel(IProjectInfoRepository projectRepository,
        IDialogService dialogService,
        INotificationService notificationService,
        IFrameRepository formedFrameRepository,
        IFormedDrainagesRepository formedDrainagesRepository,
        IStandService standService)
    {
        _projectRepository = projectRepository;
        _dialogService = dialogService;
        _notificationService = notificationService;
        _standService = standService;

        InitializeCommands();
        InitializeTime();
        InitializeGenericCommands();
    }

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

    public async Task LoadDataAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _standService.LoadStandDataAsync(CurrentStandModel);
        });
    }

    public async Task LoadObvyazkiAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _standService.LoadObvyazkiInStandAsync(CurrentStandModel);
        });
    }

    public async Task LoadProjectInfoAsync(int projectId)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var projectInfo = await _projectRepository.GetStandsByIdAsync(projectId);
            if (projectInfo == null)
                return;

            CurrentProjectModel.CurrentProjectId = projectInfo.Id;
            CurrentProjectModel.Number = projectInfo.Number;
            CurrentProjectModel.Description = projectInfo.Description;
            CurrentProjectModel.CreationDate = projectInfo.CreationDate.ToDateTime(TimeOnly.MinValue);
            CurrentProjectModel.Company = projectInfo.Company;
            CurrentProjectModel.Object = projectInfo.Object;
            CurrentProjectModel.StandCount = projectInfo.StandCount;
            CurrentProjectModel.Cost = projectInfo.Cost;
            CurrentProjectModel.Status = projectInfo.Status.ToString();
            CurrentProjectModel.StartDate = projectInfo.StartDate.ToDateTime(TimeOnly.MinValue);
            CurrentProjectModel.OutOfProduction = projectInfo.OutOfProduction.ToDateTime(TimeOnly.MinValue);
            CurrentProjectModel.EndDate = projectInfo.EndDate.ToDateTime(TimeOnly.MinValue);
            CurrentProjectModel.OrderCustomer = projectInfo.OrderCustomer;
            CurrentProjectModel.RequestProduction = projectInfo.RequestProduction;
            CurrentProjectModel.MarkMinus = projectInfo.MarkMinus;
            CurrentProjectModel.MarkPlus = projectInfo.MarkPlus;
            CurrentProjectModel.IsGalvanized = projectInfo.IsGalvanized;

            CurrentProjectModel.Stands.Clear();
            if (projectInfo.Stands != null)
                foreach (var stand in projectInfo.Stands)
                {
                    var standModel = StandDataConverter.ConvertToStandModel(stand);

                    // Получаем рамы через StandFrame
                    if (stand.StandFrames != null)
                        standModel.FramesInStand = new ObservableCollection<FormedFrame>(
                            stand.StandFrames.Select(sf => sf.Frame)
                        );
                    else
                        standModel.FramesInStand = new ObservableCollection<FormedFrame>();
                    CurrentProjectModel.Stands.Add(standModel);
                }

            CurrentProjectModel.SelectedStand = CurrentProjectModel.Stands.FirstOrDefault();

            if (CurrentProjectModel.SelectedStand != null)
                CurrentStandModel = CurrentProjectModel.SelectedStand;
            else
                CurrentStandModel = new StandModel();

            OnPropertyChanged(nameof(CurrentStandModel));
        });
    }

    private async Task AddObvToStandAsync()
    {
        if (CurrentProjectModel.SelectedStand == null)
            return;

        var entity = new ObvyazkaInStand
        {
            StandId = CurrentProjectModel.SelectedStand.Id,
            ObvyazkaId = CurrentProjectModel.SelectedStand.ObvyazkaType,
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

        await _standService.AddObvyazkaToStandAsync(CurrentProjectModel.SelectedStand.Id, entity);

        OnPropertyChanged(nameof(CurrentProjectModel.SelectedStand.ObvyazkiInStand));
    }

    private async Task SaveProjectChangesAsync()
    {
        if (CurrentProjectModel.CurrentProjectId == 0)
        {
            _notificationService.ShowInfo("Сначала создайте проект");
            return;
        }

        var projectInfo = new ProjectInfo
        {
            Id = CurrentProjectModel.CurrentProjectId,
            Number = CurrentProjectModel.Number,
            Description = CurrentProjectModel.Description,
            CreationDate = DateOnly.FromDateTime(CurrentProjectModel.CreationDate),
            Company = CurrentProjectModel.Company,
            Object = CurrentProjectModel.Object,
            StandCount = CurrentProjectModel.StandCount,
            Cost = CurrentProjectModel.Cost,
            HumanCost = CurrentProjectModel.HumanCost,
            Manager = CurrentProjectModel.Manager,
            Status = ComboBoxHelper.ComboBoxChangedValue<ProjectStatus>(CurrentProjectModel.Status),
            StartDate = DateOnly.FromDateTime(CurrentProjectModel.StartDate),
            OutOfProduction = DateOnly.FromDateTime(CurrentProjectModel.OutOfProduction),
            EndDate = DateOnly.FromDateTime(CurrentProjectModel.EndDate),
            OrderCustomer = CurrentProjectModel.OrderCustomer,
            RequestProduction = CurrentProjectModel.RequestProduction,
            MarkMinus = CurrentProjectModel.MarkMinus,
            MarkPlus = CurrentProjectModel.MarkPlus,
            IsGalvanized = CurrentProjectModel.IsGalvanized
        };
        await _projectRepository.UpdateAsync(projectInfo);
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
            ObvyazkaType = CurrentStandModel.ObvyazkaType,
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
        var newProjectCard = CurrentProjectModel.CreateNewProjectCard();

        await _projectRepository.AddAsync(newProjectCard);

        CurrentProjectModel.CurrentProjectId = newProjectCard.Id;
        CurrentProjectModel.Stands.Clear();
        CurrentStandModel = new StandModel();

        _notificationService.ShowInfo(
            $"Новая карточка проекта успешно создана!\nId Проекта: {CurrentProjectModel.CurrentProjectId}");
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
                CurrentStandModel.SelectedFrame.Id // теперь передаём только Id рамы
            );

            CurrentStandModel.FramesInStand.Add(CurrentStandModel.SelectedFrame);
            OnPropertyChanged(nameof(CurrentStandModel.FramesInStand));
        }
    }

    private async Task AddDrainageToStandAsync()
    {
        if (CurrentStandModel.SelectedDrainage != null)
        {
            await _standService.AddDrainageToStandAsync(
                CurrentProjectModel.SelectedStand.Id,
                CurrentStandModel.SelectedDrainage.Id);

            CurrentStandModel.DrainagesInStand.Add(CurrentStandModel.SelectedDrainage);
        }
    }

    private async Task AddCustomDrainageToStandAsync()
    {
        if (!string.IsNullOrWhiteSpace(CurrentStandModel.NewDrainage.Name))
        {
            await _standService.AddCustomDrainageAsync(
                CurrentProjectModel.SelectedStand.Id,
                CurrentStandModel.NewDrainage);

            CurrentStandModel.AllAvailableDrainages.Add(CurrentStandModel.NewDrainage);
            CurrentStandModel.NewDrainage = new FormedDrainage();
            OnPropertyChanged(nameof(CurrentStandModel.AllAvailableDrainages));
        }
    }

    #endregion
}