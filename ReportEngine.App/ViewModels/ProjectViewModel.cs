using System.Collections.ObjectModel;
using System.Diagnostics;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.Commands.Initializers;
using ReportEngine.App.Commands.Providers;
using ReportEngine.App.Model;
using ReportEngine.App.Model.CalculationModels;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.ModelWrappers;
using ReportEngine.App.Services;
using ReportEngine.App.Services.Core;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.ViewModels.Utils;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Armautre;
using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.ElectricSockets;
using ReportEngine.Domain.Entities.Other;
using ReportEngine.Domain.Entities.Pipes;
using ReportEngine.Domain.Repositories;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.IniHeleprs;
using ReportEngine.Shared.Config.IniHelpers;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettings;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;

namespace ReportEngine.App.ViewModels;

public class ProjectViewModel : BaseViewModel
{
    private readonly ICalculationService _calculationService;
    private readonly IBaseRepository<Company> _companyRepository;
    private readonly UpdaterStandService _updaterStandService;
    private readonly IContainerRepository _containerRepository;
    private readonly ContainerService _containerService;
    private readonly IDialogService _dialogService;
    private readonly INotificationService _notificationService;
    private readonly IProjectDataLoaderService _projectDataLoaderService;
    private readonly IProjectInfoRepository _projectRepository;
    private readonly IProjectService _projectService;
    private readonly IReportService _reportService;
    private readonly IStandService _standService;
    private readonly AdditionalEquipService _additionalEquipService;
    private readonly SemaphoreSlim _updateUiLock = new(1, 1);
    private readonly UIValidatorService _uiValidatorService;
    private readonly GenericRepository _genericRepository;
    private readonly InitializeService _initializeService;

    public ProjectViewModel(
        IProjectInfoRepository projectRepository,
        IDialogService dialogService,
        INotificationService notificationService,
        IStandService standService,
        IProjectService projectService,
        IProjectDataLoaderService projectDataLoaderService,
        IReportService reportService,
        ICalculationService calculationService,
        ContainerService containerService,
        UpdaterStandService updaterStandService,
        AdditionalEquipService additionalEquipService,
        UIValidatorService uiValidatorService,
        GenericRepository genericRepository,
        InitializeService initializeService)
    {
        _genericRepository = genericRepository;
        _projectRepository = projectRepository;
        _dialogService = dialogService;
        _notificationService = notificationService;
        _standService = standService;
        _projectService = projectService;
        _projectDataLoaderService = projectDataLoaderService;
        _reportService = reportService;
        _calculationService = calculationService;
        _containerService = containerService;
        _updaterStandService = updaterStandService;
        _additionalEquipService = additionalEquipService;
        _uiValidatorService = uiValidatorService;
        _initializeService = initializeService;

        NewStand = new StandModel { Number = 1 };

        InitializeCommands();
        InitializeTime();
        InitializeGenericCommands();
        InitializeStandsData();
        
    }
    public FrameSettingsModel FrameSettings { get; set; } = new();
    public ObservableCollection<FormedFrame> AllAvailableFrames { get; set; } = new();
    public ObservableCollection<FormedDrainage> AllAvailableDrainages { get; set; } = new();
    public ObservableCollection<FormedElectricalComponent> AllAvailableElectricalComponents { get; set; } = new();
    public ObservableCollection<FormedAdditionalEquip> AllAvailableAdditionalEquips { get; set; } = new();
    public Obvyazka SelectedObvyazka { get; set; } = new();
    public StandModel CurrentStandModel { get; set; } = new();
    public StandModel NewStand { get; set; } = new();
    public ProjectModel CurrentProjectModel { get; set; } = new();
    public ProjectCommandProvider ProjectCommandProvider { get; set; } = new();
    public MaterialLinesModel CurrentMaterials { get; set; } = new();
    public StandSettingsData StandSettings { get; set; } = new();

    #region Инициализация
    public void InitializeStandsData()
    {
        StandSettings = CalculationSettingsManager.Load<StandSettings, StandSettingsData>();
    }
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

    #endregion Инициализация
    public int MaxObvNN
    {
        get => CurrentProjectModel?.SelectedStand?.ObvyazkiInStand.Max(obv => obv.NN) ?? 0;
    }

    public int MaxStandNN
    {
        get => CurrentProjectModel.Stands.Count > 0 ? CurrentProjectModel.Stands.Max(stand => stand.Number) : 0;
    }

    public bool CanAllCommandsExecute(object? e)
    {
        return true;
    }
    public void OnOpenAllSortamentsDialogExecuted(object e)
    {
        var selected = _dialogService.ShowAllSortamentsDialog();

        if (selected == null)
            return;

        ApplySelectedEquipToPurpose(e, selected);
    }

    public async void OnShowCompanyDialogExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            CurrentProjectModel.Company = _dialogService.ShowCompanyDialog();
        });
    }

    public async void OnShowSubjectDialogExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
                CurrentProjectModel.Object = _dialogService.ShowSubjectDialog());
    }

    public async void OnOpenObvSettingsWindowCommandExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            CurrentProjectModel.SelectedStand.ObvyazkaAdditionalComponents.Clear();

            _dialogService.ShowObvSettingsWindow(this);
        });
    }

    public async void OnShowFrameDialogExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            if (Guard.ExitIfNull("Сначала создайте стенд!", _notificationService, CurrentProjectModel.SelectedStand))
                return;

            var totalWidth = _projectService.GetSummWidthObvyzaka(CurrentProjectModel);
            _notificationService.ShowInfo("Рекомендуемая рама: Рама с длиной " + totalWidth);

            var selectedFrame = _dialogService.ShowFrameDialog();

            if (Guard.ExitIfNull("Рама или стенд не выбраны!",
                           _notificationService,
                           selectedFrame,
                           CurrentProjectModel.SelectedStand))
                return;

            await _standService.AddFrameToStandAsync(CurrentProjectModel.SelectedStand.Id, selectedFrame.Id);

            if (selectedFrame.Disassembled == true)
            {
                await DisambledFrameUpdateAsync();
            }

            CurrentProjectModel.SelectedStand.FramesInStand.Add(selectedFrame);

            OnFramesInStandChanged();
        });
    }
    public async void OnAdditionalTestCommandExecuted(object e)
    {
        CurrentProjectModel.SelectedStand.AdditionalPurposesChanges = true;

        await _additionalEquipService.CreateEquipsFromObvyzkaAsync(CurrentProjectModel);
    }

    public async Task DisambledFrameUpdateAsync()
    {
        await FrameSettings.LoadFrameDataFromIniAsync();

        var materialOne = await _genericRepository.GetAsync<Other>(x => x.Name == FrameSettings.MaterialOne);
        var materialTwo = await _genericRepository.GetAsync<Other>(x => x.Name == FrameSettings.MaterialTwo);


        var items = new List<AdditionalEquipPurpose>
        {
            new()
            {
                Material = FrameSettings.MaterialOne,
                Quantity = (float)FrameSettings.CountMaterialOne,
                CostPerUnit = materialOne.Cost,
                Measure = "шт",
                FormedAdditionalEquipId = CurrentProjectModel.SelectedStand.AdditionalEquipsInStand.FirstOrDefault().Id
            },
            new()
            {
                Material = FrameSettings.MaterialTwo,
                Quantity = (float)FrameSettings.CountMaterialTwo,
                CostPerUnit = materialTwo.Cost,
                Measure = "шт",
                FormedAdditionalEquipId = CurrentProjectModel.SelectedStand.AdditionalEquipsInStand.FirstOrDefault().Id
            }
        };

        foreach (var item in items)
        {
            await _standService.UpdateAdditionalPurposeAsync(item);
            CurrentProjectModel.SelectedStand.AllAdditionalEquipPurposesInStand.Add(item);
        }
    }

    // TODO: Сделать тут рефакторинг команд
    public void OnSelectMaterialFromDialogCommandExecuted(object e)
    {
        if (Guard.ExitIfNull("Стенд не выбран!",
                        _notificationService,
                        CurrentProjectModel.SelectedStand))
            return;

        switch (CurrentMaterials.SelectedMaterialLine)
        {
            case "Жаропрочные":
                SelectEquipment<HeaterPipe>(
                    name => CurrentProjectModel.SelectedStand.MaterialLine = name,
                    measure => CurrentProjectModel.SelectedStand.MaterialLineMeasure = measure,
                    cost => CurrentProjectModel.SelectedStand.MaterialLineCostPerUnit = cost,
                    exportDays => CurrentProjectModel.SelectedStand.MaterialLineExportDays = exportDays,
                    weight => CurrentProjectModel.SelectedStand.Weight += weight);
                break;

            case "Нержавеющие":
                SelectEquipment<StainlessPipe>(
                    name => CurrentProjectModel.SelectedStand.MaterialLine = name,
                    measure => CurrentProjectModel.SelectedStand.MaterialLineMeasure = measure,
                    cost => CurrentProjectModel.SelectedStand.MaterialLineCostPerUnit = cost,
                    exportDays => CurrentProjectModel.SelectedStand.MaterialLineExportDays = exportDays,
                    weight => CurrentProjectModel.SelectedStand.Weight += weight);
                break;

            case "Углеродистые":
                SelectEquipment<CarbonPipe>(
                    name => CurrentProjectModel.SelectedStand.MaterialLine = name,
                    measure => CurrentProjectModel.SelectedStand.MaterialLineMeasure = measure,
                    cost => CurrentProjectModel.SelectedStand.MaterialLineCostPerUnit = cost,
                    exportDays => CurrentProjectModel.SelectedStand.MaterialLineExportDays = exportDays,
                    weight => CurrentProjectModel.SelectedStand.Weight += weight);
                break;
        }

        _standService.UpdateStandWeight(CurrentProjectModel.SelectedStand);
    }

    public void OnSelectArmatureFromDialogCommandExecuted(object e)
    {
        if (Guard.ExitIfNull("Стенд не выбран!",
                _notificationService,
                CurrentProjectModel.SelectedStand))
            return;

        switch (CurrentMaterials.SelectedAramuteres)
        {
            case "Жаропрочные":
                SelectEquipment<HeaterArmature>(
                    name => CurrentProjectModel.SelectedStand.Armature = name,
                    measure => CurrentProjectModel.SelectedStand.ArmatureMeasure = measure,
                    cost => CurrentProjectModel.SelectedStand.ArmatureCostPerUnit = cost,
                    exportDays => CurrentProjectModel.SelectedStand.ArmatureExportDays = exportDays,
                    weight => CurrentProjectModel.SelectedStand.Weight += weight);
                break;

            case "Нержавеющие":
                SelectEquipment<StainlessArmature>(
                    name => CurrentProjectModel.SelectedStand.Armature = name,
                    measure => CurrentProjectModel.SelectedStand.ArmatureMeasure = measure,
                    cost => CurrentProjectModel.SelectedStand.ArmatureCostPerUnit = cost,
                    exportDays => CurrentProjectModel.SelectedStand.ArmatureExportDays = exportDays,
                    weight => CurrentProjectModel.SelectedStand.Weight += weight);
                break;

            case "Углеродистые":
                SelectEquipment<CarbonArmature>(
                    name => CurrentProjectModel.SelectedStand.Armature = name,
                    measure => CurrentProjectModel.SelectedStand.ArmatureMeasure = measure,
                    cost => CurrentProjectModel.SelectedStand.ArmatureCostPerUnit = cost,
                    exportDays => CurrentProjectModel.SelectedStand.ArmatureExportDays = exportDays,
                    weight => CurrentProjectModel.SelectedStand.Weight += weight);
                break;
        }

        _standService.UpdateStandWeight(CurrentProjectModel.SelectedStand);
    }

    public void OnSelectTreeSocketFromDialogCommandExecuted(object e)
    {
        if (Guard.ExitIfNull("Стенд не выбран!",
                        _notificationService,
                        CurrentProjectModel.SelectedStand))
            return;

        switch (CurrentMaterials.SelectedSocketTypes)
        {
            case "Жаропрочные":
                SelectEquipment<HeaterSocket>(
                    name => CurrentProjectModel.SelectedStand.TreeSocket = name,
                    measure => CurrentProjectModel.SelectedStand.TreeSocketMaterialMeasure = measure,
                    cost => CurrentProjectModel.SelectedStand.TreeSocketMaterialCostPerUnit = cost,
                    exportDays => CurrentProjectModel.SelectedStand.TreeSocketExportDays = exportDays,
                    weight => CurrentProjectModel.SelectedStand.Weight += weight);
                break;

            case "Нержавеющие":
                SelectEquipment<StainlessSocket>(
                    name => CurrentProjectModel.SelectedStand.TreeSocket = name,
                    measure => CurrentProjectModel.SelectedStand.TreeSocketMaterialMeasure = measure,
                    cost => CurrentProjectModel.SelectedStand.TreeSocketMaterialCostPerUnit = cost,
                    exportDays => CurrentProjectModel.SelectedStand.TreeSocketExportDays = exportDays,
                    weight => CurrentProjectModel.SelectedStand.Weight += weight);
                break;

            case "Углеродистые":
                SelectEquipment<CarbonSocket>(
                    name => CurrentProjectModel.SelectedStand.TreeSocket = name,
                    measure => CurrentProjectModel.SelectedStand.TreeSocketMaterialMeasure = measure,
                    cost => CurrentProjectModel.SelectedStand.TreeSocketMaterialCostPerUnit = cost,
                    exportDays => CurrentProjectModel.SelectedStand.TreeSocketExportDays = exportDays,
                    weight => CurrentProjectModel.SelectedStand.Weight += weight);
                break;
        }

        _standService.UpdateStandWeight(CurrentProjectModel.SelectedStand);
    }

    public void OnSelectKMCHFromDialogCommandExecuted(object e)
    {
        if (Guard.ExitIfNull("Стенд не выбран!",
                        _notificationService,
                        CurrentProjectModel.SelectedStand))
            return;

        switch (CurrentMaterials.SelectedKMCHType)
        {
            case "Жаропрочные":
                SelectEquipment<HeaterSocket>(
                    name => CurrentProjectModel.SelectedStand.KMCH = name,
                    measure => CurrentProjectModel.SelectedStand.KMCHMeasure = measure,
                    cost => CurrentProjectModel.SelectedStand.KMCHCostPerUnit = cost,
                    exportDays => CurrentProjectModel.SelectedStand.KMCHExportDays = exportDays,
                    weight => CurrentProjectModel.SelectedStand.Weight += weight);
                break;

            case "Нержавеющие":
                SelectEquipment<StainlessSocket>(
                    name => CurrentProjectModel.SelectedStand.KMCH = name,
                    measure => CurrentProjectModel.SelectedStand.KMCHMeasure = measure,
                    cost => CurrentProjectModel.SelectedStand.KMCHCostPerUnit = cost,
                    exportDays => CurrentProjectModel.SelectedStand.KMCHExportDays = exportDays,
                    weight => CurrentProjectModel.SelectedStand.Weight += weight);
                break;

            case "Углеродистые":
                SelectEquipment<CarbonSocket>(
                    name => CurrentProjectModel.SelectedStand.KMCH = name,
                    measure => CurrentProjectModel.SelectedStand.KMCHMeasure = measure,
                    cost => CurrentProjectModel.SelectedStand.KMCHCostPerUnit = cost,
                    exportDays => CurrentProjectModel.SelectedStand.KMCHExportDays = exportDays,
                    weight => CurrentProjectModel.SelectedStand.Weight += weight);
                break;
        }

        _standService.UpdateStandWeight(CurrentProjectModel.SelectedStand);
    }

    public async void OnCreateNewCardCommandExecuted(object? e)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var isCorrectProjNumber = _uiValidatorService.ValidateCorrectProjNN(CurrentProjectModel.Number);

            if (!isCorrectProjNumber)
                return;

            var isFreeProjNumber = await _uiValidatorService.ValidateFreeProjNN(this, CurrentProjectModel.Number, false);

            if (!isFreeProjNumber)
                return;

            var isCorrectStatus = _uiValidatorService.ValidateProjectStatus(this);

            if (!isCorrectStatus)
                return;

            await CreateNewProjectCardAsync();
            await _projectService.GetOrAddCompanyAsync(CurrentProjectModel.Company);
            await _projectService.GetOrAddSubjectAsync(CurrentProjectModel.Object, CurrentProjectModel.Company);
        });
    }

    public async void OnOpenCreateNewStandCommandExecuted(object? e)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            _dialogService.ShowStandsSettingsWindow(this, false);
        });
    }

    public async void OnOpenEditStandCommandExecuted(object? e)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            _dialogService.ShowEditStandsObvSettingsWindow(this, CurrentProjectModel.SelectedStand, true);
        });
    }

    public async void OnAddNewStandCommandExecuted(object? e)
    {
        await ExceptionHelper.SafeExecuteAsync(AddNewStandToProjectAsync);
    }

    public async void OnCopyStandsCommandExecuted(object? e)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            if (CurrentProjectModel.SelectedStand == null)
            {
                _notificationService.ShowError("Стенды для копирования не выбраны!");
                return;
            }

            await _projectService.CopyStandsAsync(CurrentProjectModel);

            await LoadPurposesInStandsAsync();
            await LoadObvyazkiAsync();

            var lastStand = CurrentProjectModel.Stands.LastOrDefault();

            if (lastStand == null)
                return;

            CurrentProjectModel.SelectedStand = lastStand;
        });
    }

    public async void OnDeleteSelectedStandFromProjectExecuted(object? e)
    {
        await ExceptionHelper.SafeExecuteAsync(DeleteStandFromProject);
    }

    public async void OnSaveChangesCommandExecuted(object? e)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var isCorrectProjNumber = _uiValidatorService.ValidateCorrectProjNN(CurrentProjectModel.Number);

            if (!isCorrectProjNumber)
                return;

            var isFreeProjNumber = await _uiValidatorService.ValidateFreeProjNN(this, CurrentProjectModel.Number, true);

            if (!isFreeProjNumber)
                return;

            await SaveProjectChangesAsync();
        });
    }

    public async void OnAddObvCommandExecuted(object e)
    {
        var selectedStand = CurrentProjectModel?.SelectedStand;

        if (Guard.ExitIfNull("Не был выбран стенд", _notificationService, selectedStand))
            return;

        if (Guard.ExitIfNull("Не был выбран тип обвязки", _notificationService, SelectedObvyazka))
            return;



        var correctNN = _uiValidatorService.ValidateCorrectObvNN(selectedStand.NN);

        if (!correctNN)
            return;

        var freeNN = _uiValidatorService.ValidateFreeObvNN(this,selectedStand.NN, false);

        if (!freeNN)
            return;


        //var isCorrectSensorsData = _uiValidatorService.ValidateSensorsQuantityInNewObv(this);

        //if (!isCorrectSensorsData)
         //   return;

        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await AddObvToStandAsync();
            await LoadObvyazkiAsync(); // Перезагрузить данные из БД

            UpdateNewObvNN();
            OnObvyazkiInStandChanged();
        });

        _notificationService.ShowInfo("Обвязка добавлена в стенд");
    }

    public async void OnDeleteAdditionalEquipFromObvCommandExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        await _standService.DeleteAdditionalPurposeFromObvAsync(
            CurrentProjectModel.SelectedStand.SelectedObvyazkaAdditionalEquipPurpose,
            CurrentProjectModel.SelectedStand));
    }

    public async void OnUpdateAdditionalEquipFromObvCommandExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var stand = CurrentProjectModel.SelectedStand;

            if (Guard.ExitIfNull("Стенд не выбран!", _notificationService, stand))
                return;

            var obvyazki = stand.ObvyazkaAdditionalComponents.ToList();

            foreach (var obv in obvyazki)
            {
                if (obv.Id == 0)
                {
                    obv.ObvyazkaInStandId = stand.SelectedObvyazkaInStand?.Id;
                }

                await _standService.UpdateAdditionalPurposeFromObvAsync(obv, obv.ObvyazkaInStandId ?? 0);
            }

            _notificationService.ShowInfo("Все комплектующие обвязок сохранены");
        });
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

            OnFramesInStandChanged();

            _notificationService.ShowInfo("Рама удалена из стенда");
        });
    }

    public async void OnUpdateStandsAfterEquipsCommandExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _updaterStandService.ApplyChangesAndSaveAsync(CurrentProjectModel);
        });
    }

    public async void OnAddDrainageToStandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(AddDrainageToStandAsync);
    }

    public async void OnAddFrameToStandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(AddFrameToStandAsync);
    }

    public async void OnCopyObvyazkaToStandsCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var sourceObv = CurrentProjectModel.SelectedObvyazkaToCopy;

            if (Guard.ExitIfNull("Не выбран стенд!", _notificationService, CurrentProjectModel.SelectedStand))
                return;

            var standId = CurrentProjectModel.SelectedStand.Id;

            var newObvyazka = ObvyzkaModelWrapper.CloneForStand(sourceObv, standId);
            newObvyazka.NN = MaxObvNN + 1;


            await _standService.AddObvyazkaToStandAsync(standId, newObvyazka);

            OnPropertyChanged(nameof(CurrentProjectModel.SelectedStand.ObvyazkiInStand));
            OnPropertyChanged(nameof(CurrentProjectModel.ObvyazkiInProject));

            await LoadObvyazkiAsync();

            OnObvyazkiInStandChanged();

            _notificationService.ShowInfo("Обвязка успешно добавлена в стенд!");
        });
    }

    public void OnSelectObvCommandExecuted(object p)
    {
        ExceptionHelper.SafeExecute(() =>
        {
            SelectedObvyazka = _dialogService.ShowObvyazkaDialog(true);

            //если не выбрали - просто выходим
            if (SelectedObvyazka == null)
                return;

            var stand = CurrentProjectModel.SelectedStand;

            var tmp = new ObvyazkaInStand
            {
                ImageName = SelectedObvyazka.ImageName
            };

            stand.MaterialLineCount = SelectedObvyazka.LineLength;
            stand.ArmatureCount = SelectedObvyazka.ZraCount;
            stand.TreeSocketMaterialCount = SelectedObvyazka.TreeSocket;
            stand.KMCHCount = SelectedObvyazka.KMCHCount;

            stand.SelectedObvyazkaInStand = tmp;
        });
    }

    public async void OnCalculateProjectCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(CalculateProjectAsync);
    }

    public async void OnComponentsListReportCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(() => CreateReportAsync(ReportType.ComponentsListReport, "комплектующих"));
    }

    public async void OnCreateSummaryReportCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(() => CreateReportAsync(ReportType.SummaryReport, "сводная"));
    }

    public async void OnCreateMarksReportCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(() => CreateReportAsync(ReportType.MarksReport, "маркировки"));
    }

    public async void OnCreateNameplatesReportCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(() => CreateReportAsync(ReportType.NameplatesReport, "шильдики и таблички"));
    }

    public async void OnCreateContainerReportCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(() => CreateReportAsync(ReportType.ContainerReport, "тара"));
    }

    public async void OnCreateProductionReportCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(() => CreateReportAsync(ReportType.ProductionReport, "производство"));
    }

    public async void OnCreateFinplanReportCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(() => CreateReportAsync(ReportType.FinPlanReport, "финплан"));
    }

    public async void OnCreatePassportReportCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(() => CreateReportAsync(ReportType.PassportsReport, "паспорта"));
    }

    public async void OnCreateTechnologicalCardsCommandExecute(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(async () => await CreateReportAsync(ReportType.TechnologicalCards, "технологические карты"));
    }

    public async void OnSaveChangesInStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(SaveChangesInStandAsync);
    }

    public async void OnSaveAllChangesInComponentsCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            if (CurrentProjectModel.SelectedStand == null)
            {
                _notificationService.ShowError("Стенд не выбран");
                return;
            }

            await _standService.SaveAllPurposesInStandAsync(CurrentProjectModel.SelectedStand);

            CurrentProjectModel.SelectedStand.DrainagePurposesChanges = false;
            CurrentProjectModel.SelectedStand.ElectricalPurposesChanges = false;
            CurrentProjectModel.SelectedStand.AdditionalPurposesChanges = false;

            _notificationService.ShowInfo("Все изменения сохранены");
        });
    }

    public async Task OnDeleteElectricalComponentFromStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await DeletePurposeAsync(CurrentProjectModel.SelectedStand.SelectedElectricalComponent,
                _standService.DeleteElectricalPurposeAsync,
                CurrentProjectModel.SelectedStand.AllElectricalPurposesInStand,
                "Электрический компонент удалён");
        });
    }

    public async Task OnUpdateElectricalComponentInStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var stand = CurrentProjectModel.SelectedStand;

            if (Guard.ExitIfNull("Стенд не выбран!", _notificationService, stand))
                return;

            var purposes = stand.AllElectricalPurposesInStand.ToList();

            foreach (var purpose in purposes)
            {
                if (purpose.Id == 0)
                {
                    var firstComponent = stand.ElectricalComponentsInStand.FirstOrDefault();
                    if (firstComponent != null)
                        purpose.FormedElectricalComponentId = firstComponent.Id;
                }

                await _standService.UpdateElectricalPurposeAsync(purpose);
            }

            CurrentProjectModel.SelectedStand.ElectricalPurposesChanges = false;
            OnPropertyChanged(nameof(CurrentProjectModel.SelectedStand.ElectricalPurposesChanges));

            _notificationService.ShowInfo("Все электрические компоненты сохранены");
        });
    }

    public async Task OnDeleteAdditionalComponentFromStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await DeletePurposeAsync(CurrentProjectModel.SelectedStand.SelectedAdditionalEquip,
                _standService.DeleteAdditionalPurposeAsync,
                CurrentProjectModel.SelectedStand.AllAdditionalEquipPurposesInStand,
                "Доп. комплектующее удалено возврат");
        });
    }

    public async Task OnUpdateAdditionalComponentInStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var stand = CurrentProjectModel.SelectedStand;

            if (Guard.ExitIfNull("Стенд не выбран!", _notificationService, stand))
                return;

            foreach (var purpose in stand.AllAdditionalEquipPurposesInStand.ToList())
            {
                if (purpose.Id == 0)
                {
                    var firstComponent = stand.AdditionalEquipsInStand.FirstOrDefault();
                    if (firstComponent != null)
                        purpose.FormedAdditionalEquipId = firstComponent.Id;
                }

                await _standService.UpdateAdditionalPurposeAsync(purpose);
            }

            stand.AdditionalPurposesChanges = false;
            OnPropertyChanged(nameof(stand.AdditionalPurposesChanges));

            _notificationService.ShowInfo("Все доп. комплектующие сохранены");
        });
    }

    public async Task OnDeleteDrainageComponentFromStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await DeletePurposeAsync(CurrentProjectModel.SelectedStand.SelectedDrainagePurpose,
                _standService.DeleteDrainagePurposeAsync,
                CurrentProjectModel.SelectedStand.AllDrainagePurposesInStand,
                "Дренажное комплектующее удалено");
        });
    }

    public async Task OnUpdateDrainageComponentInStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var stand = CurrentProjectModel.SelectedStand;

            if (Guard.ExitIfNull("Стенд не выбран!", _notificationService, stand))
                return;

            var purposes = stand.AllDrainagePurposesInStand.ToList();

            foreach (var purpose in purposes)
            {
                if (purpose.Id == 0)
                {
                    var firstDrainage = stand.DrainagesInStand.FirstOrDefault();
                    if (firstDrainage != null)
                        purpose.FormedDrainageId = firstDrainage.Id;
                }

                await _standService.UpdateDrainagePurposeAsync(purpose);
            }

            CurrentProjectModel.SelectedStand.DrainagePurposesChanges = false;
            OnPropertyChanged(nameof(CurrentProjectModel.SelectedStand.DrainagePurposesChanges));

            _notificationService.ShowInfo("Все дренажные компоненты сохранены");
        });
    }

    public async void OnEditObvSettingsCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            _dialogService.ShowEditObvSettingsWindow(this,
                CurrentProjectModel.SelectedStand,
                CurrentProjectModel.SelectedStand.SelectedObvyazkaInStand);
        });
    }

    public async void OnFillObvFieldsCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            _standService.FillStandFieldsFromObvyazka(CurrentProjectModel.SelectedStand,
                CurrentProjectModel.SelectedObvyazkaToCopy);
        });
    }

    //TODO: вынести в standService
    public async void OnFillStandFieldsFromSelectedStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var selectedStand = CurrentProjectModel.SelectedStand;

            if (selectedStand == null)
            {
                Debug.WriteLine("Стенд для перезаполнения пуст");
                return;
            }

            NewStand.Number = selectedStand.Number;
            NewStand.KKSCode = selectedStand.KKSCode;
            NewStand.Design = selectedStand.Design;
            NewStand.BraceType = selectedStand.BraceType;
            NewStand.Devices = selectedStand.Devices;
            NewStand.Width = selectedStand.Width;
            NewStand.SerialNumber = selectedStand.SerialNumber;
            NewStand.Weight = selectedStand.Weight;
            NewStand.StandSummCost = selectedStand.StandSummCost;
            NewStand.Number = selectedStand.Number;
            NewStand.ProjectId = selectedStand.ProjectId;
            NewStand.Comments = selectedStand.Comments;
            NewStand.DesignStand = selectedStand.DesignStand;

            Debug.WriteLine("Поля перезаполнены");
        });
    }

    public async void OnRenumerateStandsCommandExecuted(object obj)
    {
        var renumInfo = _dialogService.ShowRenumerateDialog();

        if (CurrentProjectModel.Stands == null)
        {
            _notificationService.ShowError("Список стендов пуст");
            return;
        }

        if (!renumInfo.StartValue.HasValue || !renumInfo.Step.HasValue)
        {
            _notificationService.ShowError("Неверно введены данные. Операция отменена.");
            return;
        }

        var renumeratedStand = CurrentProjectModel.Stands
            .Where(stand => stand.Number >= renumInfo.FromNumber && stand.Number <= renumInfo.ToNumber)
            .OrderBy(stand => stand.Number)
            .ToList();

        if (renumeratedStand == null || renumeratedStand.Count < 1)
        {
            _notificationService.ShowError("Не найдены подходящие стенды");
            return;
        }

        var standEntities = new List<Stand>();

        int iteration = 1;

        foreach (var stand in renumeratedStand)
        {
            var iterPart = renumInfo.StartValue.Value + (iteration - 1) * renumInfo.Step.Value;
            string formattedIterPart = iterPart.ToString().PadLeft(renumInfo.StartValueLength, '0');


            stand.SerialNumber = $"{renumInfo.Prefix}{formattedIterPart}{renumInfo.Postfix}";

            var newStandEntity = StandDataConverter.ConvertToStandEntity(stand);
            standEntities.Add(newStandEntity);

            iteration++;
        }

        await _projectRepository.UpdateStandsGroupAsync(standEntities);

        _notificationService.ShowInfo("Стенды пронумерованы");
    }

    public async void OnUpdateObvInStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var selectedStand = CurrentProjectModel?.SelectedStand;

            if (Guard.ExitIfNull("Не был выбран стенд", _notificationService, selectedStand))
                return;

            var correctNN = _uiValidatorService.ValidateCorrectObvNN(selectedStand.NN);

            if (!correctNN)
                return;

            var freeNN = _uiValidatorService.ValidateFreeObvNN(this, selectedStand.NN, true);

            if (!freeNN)
                return;

           // var isCorrectSensorsData = _uiValidatorService.ValidateSensorsQuantityInNewObv(this);

           // if (!isCorrectSensorsData)
           //   return;


            await _projectService.UpdateObvInStandAsync(CurrentProjectModel);

            OnObvyazkiInStandChanged();
            OnPropertyChanged(nameof(CurrentProjectModel.SelectedStand.NewAdditionalEquip.Purposes));
            OnPropertyChanged(nameof(CurrentProjectModel.SelectedStand.NewElectricalComponent.Purposes));
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
           await _containerService.AddContainerToBatchAsync(CurrentProjectModel));
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
        // Совместимый синхронный вызов, чтобы не дедлокалось в процессе загрузки
        _ = ResetProjectAsync();
    }

    public async Task ResetProjectAsync()
    {
        CurrentProjectModel = new ProjectModel();
        CurrentStandModel = new StandModel();

        NewStand = new StandModel { Number = 1 };

        var projects = await _projectRepository.GetAllAsync();
        var maxProjNumber = projects?.Max(proj => proj.Number) ?? 0;

        CurrentProjectModel.Number = maxProjNumber + 1;

        InitializeTime();
        OnPropertyChanged(nameof(CurrentProjectModel));
        OnPropertyChanged(nameof(CurrentStandModel));
    }

    #region Методы загрузки данных на view

    public async Task LoadStandsDataAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _standService.LoadStandsDataAsync(CurrentProjectModel.Stands);
            await _standService.LoadAllStandsDataAsync(CurrentProjectModel.CurrentProjectId, CurrentProjectModel.Stands);
        });
    }

    public async Task LoadPurposesInStandsAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        await _standService.LoadPurposesInStands(CurrentProjectModel.Stands));
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
            await _projectDataLoaderService.LoadAllAvailDataToViewModelAsync(this);
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

    #endregion Методы загрузки данных на view

    #region Методы для CRUD с проектами и стендами

    private async Task AddObvToStandAsync()
    {
        var selectedStand = CurrentProjectModel.SelectedStand;

        if (Guard.ExitIfNull("Не выбран стенд!", _notificationService, selectedStand))
            return;

        if (Guard.ExitIfNull("Не выбран тип обвязки!", _notificationService, SelectedObvyazka))
            return;

        //автонумерация
        selectedStand.NN = MaxObvNN + 1;

        var entity = await _standService.CreateObvyazkaAsync(selectedStand, SelectedObvyazka);

        if (Guard.ExitIfNull("Не был выбран тип обвязки", _notificationService, entity))
            return;

        await _standService.AddObvyazkaToStandAsync(selectedStand.Id, entity);

        //сравнение по типу
        var isAlreadyExist = CurrentProjectModel.ObvyazkiInProject.Any(obv => obv.ObvyazkaName == entity.ObvyazkaName);

        if (!isAlreadyExist)
        {
            CurrentProjectModel.ObvyazkiInProject.Add(entity);
        }

        CollectionRefreshHelper.SafeRefreshCollection(CurrentProjectModel.SelectedStand.ObvyazkaAdditionalComponents);
    }

    private async Task DeleteObvFromStandAsync()
    {
        var stand = CurrentProjectModel?.SelectedStand;
        var selectedObv = stand?.SelectedObvyazkaInStand;

        if (Guard.ExitIfNull("Стенд или обвязка не выбраны", _notificationService, stand, selectedObv))
            return;

        var standId = stand.Id;
        var obvId = selectedObv.Id;

        await _projectService.DeleteObvFromStandAsync(standId, obvId);

        var toRemove = stand.ObvyazkiInStand?.FirstOrDefault(o => o.Id == obvId);
        if (toRemove != null)
            stand.ObvyazkiInStand.Remove(toRemove);

        //CurrentProjectModel.ObvyazkiInProject.Remove(toRemove);

        stand.SelectedObvyazkaInStand = null;

        OnObvyazkiInStandChanged();

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

        var isCorrectStandNumber = _uiValidatorService.ValidateCorrectStandNN(NewStand.Number);

        if (!isCorrectStandNumber)
            return;

        var isFreeStandNumber = _uiValidatorService.ValidateFreeStandNN(this, NewStand.Number, false);

        if (!isFreeStandNumber)
            return;

        var newStandModel = new StandModel
        {
            KKSCode = NewStand.KKSCode,
            Design = NewStand.Design,
            BraceType = NewStand.BraceType,
            Devices = NewStand.Devices,
            Width = NewStand.Width,
            SerialNumber = NewStand.SerialNumber,
            Weight = NewStand.Weight,
            StandSummCost = NewStand.StandSummCost,
            Number = NewStand.Number,
            MaterialLine = NewStand.MaterialLine,
            Armature = NewStand.Armature,
            TreeSocket = NewStand.TreeSocket,
            KMCH = NewStand.KMCH,
            ProjectId = CurrentProjectModel.CurrentProjectId
        };

        var newStandEntity = StandDataConverter.ConvertToStandEntity(newStandModel);

        var addedStandEntity =
            await _projectRepository.AddStandAsync(CurrentProjectModel.CurrentProjectId, newStandEntity);

        newStandModel.Id = addedStandEntity.Id;
        newStandModel.ProjectId = addedStandEntity.ProjectInfoId;

        CurrentProjectModel.Stands.Add(newStandModel);

        CurrentProjectModel.SelectedStand = newStandModel;

        await CreateDefaultPurposesAsync(newStandModel);

        UpdateNewStandNN();

        OnPropertyChanged(nameof(CurrentStandModel));
        OnPropertyChanged(nameof(NewStand));

        OnStandsInProjectChanged();

        _notificationService.ShowInfo($"Стенд успешно добавлен!");
    }

    private async Task CreateDefaultPurposesAsync(StandModel newStandModel)
    {
        await _initializeService.InitializeStandDefaultPurposes(newStandModel);

        newStandModel.NewElectricalComponent.Purposes = CurrentProjectModel.SelectedStand.AllElectricalPurposesInStand.ToList();
        newStandModel.NewDrainage.Purposes = CurrentProjectModel.SelectedStand.AllDrainagePurposesInStand.ToList();
        newStandModel.NewAdditionalEquip.Purposes = CurrentProjectModel.SelectedStand.AllAdditionalEquipPurposesInStand.ToList();

        await _standService.AddCustomDrainageAsync(newStandModel.Id,
            newStandModel.NewDrainage.Purposes.ToList(),
            newStandModel.NewDrainage);

        await _standService.AddCustomElectricalComponentAsync(newStandModel.Id,
            newStandModel.NewElectricalComponent.Purposes.ToList(),
            newStandModel.NewElectricalComponent);

        await _standService.AddCustomAdditionalEquipAsync(newStandModel.Id,
            newStandModel.NewAdditionalEquip.Purposes.ToList(),
            newStandModel.NewAdditionalEquip);
    }

    private async Task SaveChangesInStandAsync()
    {
        if (CurrentProjectModel.CurrentProjectId == 0)
        {
            _notificationService.ShowInfo("Сначала создайте проект");
            return;
        }

        var selectedStand = CurrentProjectModel?.SelectedStand;

        if (Guard.ExitIfNull("Стенд не выбран!", _notificationService, selectedStand))
            return;

        var isCorrectStandNumber = _uiValidatorService.ValidateCorrectStandNN(NewStand.Number);

        if (!isCorrectStandNumber)
            return;

        var isFreeStandNumber = _uiValidatorService.ValidateFreeStandNN(this, NewStand.Number, true);

        if (!isFreeStandNumber)
            return;

        var newStandEntity = StandDataConverter.ConvertToStandEntity(NewStand);
        var selectedStandEntity = StandDataConverter.ConvertToStandEntity(selectedStand);

        selectedStandEntity.Number = newStandEntity.Number;
        selectedStandEntity.KKSCode = newStandEntity.KKSCode;
        selectedStandEntity.Design = newStandEntity.Design;
        selectedStandEntity.BraceType = newStandEntity.BraceType;
        selectedStandEntity.Devices = newStandEntity.Devices;
        selectedStandEntity.Width = newStandEntity.Width;
        selectedStandEntity.SerialNumber = newStandEntity.SerialNumber;
        selectedStandEntity.Weight = newStandEntity.Weight;
        selectedStandEntity.StandSummCost = newStandEntity.StandSummCost;
        selectedStandEntity.Comments = newStandEntity.Comments;
        selectedStandEntity.DesigneStand = newStandEntity.DesigneStand;

        await _projectRepository.UpdateStandAsync(selectedStandEntity);

        //отдельно обновляем UI
        selectedStand.Number = newStandEntity.Number;
        selectedStand.KKSCode = newStandEntity.KKSCode;
        selectedStand.Design = newStandEntity.Design;
        selectedStand.BraceType = newStandEntity.BraceType;
        selectedStand.Devices = newStandEntity.Devices;
        selectedStand.Width = newStandEntity.Width;
        selectedStand.SerialNumber = newStandEntity.SerialNumber;
        selectedStand.Weight = newStandEntity.Weight;
        selectedStand.StandSummCost = newStandEntity.StandSummCost;
        selectedStand.Comments = newStandEntity.Comments;
        selectedStand.DesignStand = newStandEntity.DesigneStand;

        OnStandsInProjectChanged();
        UpdateNewStandNN();

        _notificationService.ShowInfo("Изменения стенда сохранены");
    }

    private async Task DeleteStandFromProject()
    {
        var selected = CurrentProjectModel.SelectedStand;
        if (selected == null)
        {
            _notificationService.ShowInfo("Стенд не выбран");
            return;
        }

        await _projectService.DeleteStandAsync(CurrentProjectModel.CurrentProjectId, selected.Id);
        CurrentProjectModel.Stands.Remove(selected);

        UpdateNewStandNN();
        OnStandsInProjectChanged();
    }

    private async Task CreateNewProjectCardAsync()
    {
        await _projectService.CreateProjectAsync(CurrentProjectModel);

        CurrentProjectModel.Stands.Clear();
        CurrentStandModel = new StandModel();

        // Сброс шаблона добавления стенда
        NewStand = new StandModel { Number = 1 };
        OnPropertyChanged(nameof(NewStand));
    }

    private void SelectEquipment<T>(Action<string> setProperty,
                                    Action<string> setMeasure,
                                    Action<string> setCost,
                                    Action<int> setExportDays,
                                    Action<float> setWeight)
        where T : class, IBaseEquip, new()
    {
        ExceptionHelper.SafeExecute(() =>
        {
            var equipment = _dialogService.ShowEquipDialog<T>();
            if (equipment != null && CurrentProjectModel.SelectedStand != null)
            {
                setProperty(equipment.Name);
                setMeasure(equipment.Measure);
                setCost(equipment.Cost.ToString());
                setExportDays((int)equipment.ExportDays);
            }

            if (equipment is BaseEquip baseEquip)
            {
                setWeight((float)baseEquip.Weight);
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

    public async void OnRenumerateObvInStandAsyncCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var selectedStand = CurrentProjectModel.SelectedStand;

            if (selectedStand == null)
            {
                _notificationService.ShowError("Стенд не выбран");
                return;
            }


            int obvNumber = 1;

            foreach (var obv in selectedStand.ObvyazkiInStand)
            {
                obv.NN = obvNumber;

                await _projectRepository.UpdateObvInStandAsync(selectedStand.Id, obv);

                obvNumber++;
            }

            CollectionRefreshHelper.SafeSortAndRefreshCollection(selectedStand.ObvyazkiInStand, "NN", false);

            _notificationService.ShowInfo("Обвязки пронумерованы");

        });
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
                    dp.ExportDays = selected.ExportDays;
                    dp.Weight = selected.Weight;
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
                    ap.ExportDays = selected.ExportDays;
                    ap.Weight = selected.Weight;
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
                    ep.ExportDays = selected.ExportDays;
                    ep.Weight = selected.Weight;
                    CollectionRefreshHelper.SafeRefreshCollection(CurrentStandModel.NewElectricalComponent.Purposes);
                    CollectionRefreshHelper.SafeRefreshCollection(CurrentProjectModel.SelectedStand
                        .AllElectricalPurposesInStand);
                    CollectionRefreshHelper.SafeRefreshCollection(CurrentProjectModel.SelectedStand
                        .ElectricalComponentsInStand);
                    return;

                case ObvyazkaAdditionalEquipPurpose obv:
                    obv.Material = selected.Name;
                    obv.CostPerUnit = selected.Cost;
                    obv.Measure = selected.Measure;
                    obv.Weight = selected.Weight;
                    CollectionRefreshHelper.SafeRefreshCollection(CurrentProjectModel.SelectedStand.ObvyazkaAdditionalComponents);
                    return;

                case ContainerStand cs:
                    cs.Name = selected.Name;
                    cs.ContainerCost = selected.Cost;
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

    #endregion Методы для CRUD с проектами и стендами

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
        //Проверяем на дубликаты KKS
        bool hasDuplicates = CurrentProjectModel.Stands
            .GroupBy(stand => stand.KKSCode)
            .Any(group => group.Count() > 1);

        if (hasDuplicates)
        {
            bool confirmationResult = _notificationService.ShowConfirmation("Обнаружены дублирования KKS-кодов стендов.\nПродолжить?");

            if (!confirmationResult)
            {
                _notificationService.ShowInfo("Генерация отчета отменена");
                return;
            }
        }

        await _dialogService.RunWithProgressDialogAsync(() =>
                _reportService.GenerateReportAsync(typeGenerator, CurrentProjectModel.CurrentProjectId));


        if (_notificationService.ShowConfirmation($"Ведомость {reportName} создана!\nОткрыть папку с отчётами?"))
        {
            var reportDir = SettingsManager.GetReportDirectory();
            Process.Start("explorer.exe", reportDir);
        }
    }

    #endregion Методы расчёта и создания отчётности

    #region Обновление UI
    public void OnObvyazkiInStandChanged()
    {
        Debug.WriteLine("Обвязки поменялись");

        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        
        UpdateTablesQuantity();
        UpdateClampsQuantity();
        UpdateBracketsQuantity();
        UpdateElectricEquipment();

        CollectionRefreshHelper.SafeRefreshCollection(selectedStand.AllElectricalPurposesInStand);
        CollectionRefreshHelper.SafeRefreshCollection(selectedStand.AllAdditionalEquipPurposesInStand);

        CollectionRefreshHelper.SafeSortAndRefreshCollection(
            collection: selectedStand.ObvyazkiInStand,
            fieldToSortBy: "NN",
            descending: false);


        selectedStand.StandSensorsQuantity = selectedStand.CountElectricSensorsQuantity();
    }

    public void OnFramesInStandChanged()
    {
        Debug.WriteLine("Рамы поменялись");

        UpdateChannelsQuantity();
        UpdateDrainage();

        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        CollectionRefreshHelper.SafeRefreshCollection(selectedStand.AllAdditionalEquipPurposesInStand);
        CollectionRefreshHelper.SafeRefreshCollection(selectedStand.AllDrainagePurposesInStand);
    }

    public void OnSelectedStandChanged()
    {
        Debug.WriteLine("Выбранный стенд изменился");

        OnFramesInStandChanged();
        OnObvyazkiInStandChanged();
        UpdateBracketsQuantity();

        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        selectedStand.AdditionalPurposesChanges = false;
        selectedStand.ElectricalPurposesChanges = false;
        selectedStand.DrainagePurposesChanges = false;

    }

    public void OnStandsInProjectChanged()
    {
        Debug.WriteLine("Стенды изменились");

        //отсортировываем по возрастанию номера
        CollectionRefreshHelper.SafeSortAndRefreshCollection(
            collection: CurrentProjectModel.Stands,
            fieldToSortBy: "Number",
            descending: false);


        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        UpdateChannelsQuantity();

        CollectionRefreshHelper.SafeRefreshCollection(collection: selectedStand.AllAdditionalEquipPurposesInStand);

    }

    //обновляем поле NN в обвязке
    public void UpdateNewObvNN()
    {
        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        selectedStand.NN = MaxObvNN + 1;
    }

    //обновляем № п/п стенда
    public void UpdateNewStandNN()
    {
        if (NewStand == null) return;

        NewStand.Number = MaxStandNN + 1;
    }

    //обновляем кол-во швеллера
    public void UpdateChannelsQuantity()
    {
        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        var standBraceType = selectedStand.BraceType;

        if (string.IsNullOrEmpty(standBraceType))
            return;

        var additionalEquips = selectedStand.AllAdditionalEquipPurposesInStand;
        var channelRecord = additionalEquips.FirstOrDefault(equip => equip.Purpose == "Швеллер");

        if (channelRecord == null)
            return;

        //швеллер в штуках
        const int channelPerFrame = 1;

        if (channelRecord.IsAutoCalculationEnabled != true)
            return;

        if (standBraceType == "Швеллер")
        {
            var framesCount = selectedStand.FramesInStand.Count;
            channelRecord.Quantity = framesCount * channelPerFrame;
        }
        else
        {
            channelRecord.Quantity = 0;
        }

        selectedStand.AdditionalPurposesChanges = true;

    }

    //обновляем кол-во хомутов
    public void UpdateClampsQuantity()
    {

        var standsSettings = StandSettings;

        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        var additionalEquips = selectedStand.AllAdditionalEquipPurposesInStand;
        var clampsRecord = additionalEquips.FirstOrDefault(equip => equip.Purpose == "Хомуты");

        if (clampsRecord == null)
            return;

        if (clampsRecord.IsAutoCalculationEnabled == true)
        {
            clampsRecord.Quantity = selectedStand.ObvyazkiInStand.Sum(obv => obv.Clamp) ?? 0.0f;

            selectedStand.AdditionalPurposesChanges = true;
        }
    }

    //обновляем кол-во табличек
    public void UpdateTablesQuantity()
    {
        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        var sensorsQuantity = selectedStand.CountSensorsQuantity();

        var additionalComponents = selectedStand.AllAdditionalEquipPurposesInStand;
        var tableRecord = additionalComponents.FirstOrDefault(purpose => purpose.Purpose == "Табличка");

        if (tableRecord == null)
            return;

        if (tableRecord.IsAutoCalculationEnabled == true)
        {
            tableRecord.Quantity = sensorsQuantity;

            selectedStand.AdditionalPurposesChanges = true;
        }
    }

    //обновляем кол-во кронштейнов
    public void UpdateBracketsQuantity()
    {
        var standsSettings = StandSettings;

        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        const int bracketsPerDifSensor = 1;

        var difSensorsQuantity = selectedStand.CountDifSensorsQuantity();

        var additionalComponents = selectedStand.AllAdditionalEquipPurposesInStand;
        var difSensorsBracketRecord = additionalComponents.FirstOrDefault(purpose => purpose.Purpose == "Кронштейн перепадчика");

        if (difSensorsBracketRecord != null && difSensorsBracketRecord.IsAutoCalculationEnabled == true)
        {
            difSensorsBracketRecord.Quantity = bracketsPerDifSensor * difSensorsQuantity;

            selectedStand.AdditionalPurposesChanges = true;
        }

        const int bracketsPerAbsoluteSensor = 2;

        var standBraceType = selectedStand.BraceType;

        if (!string.IsNullOrEmpty(standBraceType) && standBraceType == "На кронштейне")
        {
            var absSensorsQuantity = selectedStand.CountAbsoluteSensorsQuantity();

            var absSensorsBracketsRecord = additionalComponents.FirstOrDefault(purpose => purpose.Purpose == "Кронштейн абсолютника");

            if (absSensorsBracketsRecord != null && absSensorsBracketsRecord.IsAutoCalculationEnabled == true)
            {
                absSensorsBracketsRecord.Quantity = bracketsPerAbsoluteSensor * absSensorsQuantity;

                selectedStand.AdditionalPurposesChanges = true;
            }
        }

        const int universalBracketQuantity = 2;

        if (!string.IsNullOrEmpty(standBraceType) && standBraceType == "Швеллер")
        {
            var universalBracketRecord = additionalComponents.FirstOrDefault(purpose => purpose.Purpose == "Кронштейн универсальный");

            if (universalBracketRecord != null && universalBracketRecord.IsAutoCalculationEnabled == true)
            {
                universalBracketRecord.Quantity = universalBracketQuantity;

                selectedStand.AdditionalPurposesChanges = true;
            }
        }
    }

    //обновляем данные по дренажу
    public void UpdateDrainage()
    {
        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        var drainageParts = selectedStand.AllDrainagePurposesInStand;
        var mainPipeRecord = drainageParts.FirstOrDefault(part => part.Purpose == "Основная труба");

        if (mainPipeRecord == null)
            return;


        if (mainPipeRecord.IsAutoCalculationEnabled == true)
        {
            mainPipeRecord.Quantity = selectedStand.FramesInStand.Sum(frame => frame.Width) / 1000.0f;

            selectedStand.DrainagePurposesChanges = true;
        }
    }

    //обновляем данные по электрике
    public void UpdateElectricEquipment()
    {
        Debug.WriteLine("Пересчет электрики начат");

        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        var electricComponents = selectedStand.AllElectricalPurposesInStand;

        //кабельные ввода
        const int cableInputsPerSensor = 2;
        var cableInputsRecord = electricComponents.FirstOrDefault(purpose => purpose.Purpose == "Кабельные вводы");

        var cableInputsQuantity = 0;

        var sensorsQuantity = selectedStand.CountElectricSensorsQuantity();



        if (cableInputsRecord != null && cableInputsRecord.IsAutoCalculationEnabled == true)
        {
            cableInputsQuantity = sensorsQuantity * cableInputsPerSensor;
            cableInputsRecord.Quantity = cableInputsQuantity;

            selectedStand.ElectricalPurposesChanges = true;
        }

        //сигнальный кабель
        var standsSettings = StandSettings;

        var signalCableRecord = electricComponents.FirstOrDefault(purpose => purpose.Purpose == "Сигнальный кабель");

        var signalCablePerSensor = 0;
        int? signalCabelQuantity = 0;

        if (signalCableRecord != null && signalCableRecord.IsAutoCalculationEnabled == true)
        {
            signalCablePerSensor = sensorsQuantity switch
            {
                >= 0 and <= 2 => 2,
                >= 3 and <= 5 => 3,
                >= 6 => 4,
                _ => 0
            };

            signalCabelQuantity = sensorsQuantity * signalCablePerSensor;

            signalCableRecord.Quantity = signalCabelQuantity;

            selectedStand.ElectricalPurposesChanges = true;
        }

        //кабель 4 мм
        var fourMmCableRecord = electricComponents.FirstOrDefault(purpose => purpose.Purpose == "Кабель 4мм");

        if (fourMmCableRecord != null && fourMmCableRecord.IsAutoCalculationEnabled == true)
        {
            fourMmCableRecord.Quantity = cableInputsQuantity;
        }

        //металлорукав
        var metalHoseRecord = electricComponents.FirstOrDefault(purpose => purpose.Purpose == "Металлорукав");

        if (metalHoseRecord != null && metalHoseRecord.IsAutoCalculationEnabled == true)
        {
            metalHoseRecord.Quantity = signalCabelQuantity;

            selectedStand.ElectricalPurposesChanges = true;
        }
    }

    #endregion Обновление UI


}
