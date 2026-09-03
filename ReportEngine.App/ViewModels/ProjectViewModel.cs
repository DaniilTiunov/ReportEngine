using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.Commands.Initializers;
using ReportEngine.App.Commands.Providers;
using ReportEngine.App.Enums;
using ReportEngine.App.Model;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.ModelWrappers;
using ReportEngine.App.Services.Calculation;
using ReportEngine.App.Services.Cloners;
using ReportEngine.App.Services.Core;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.Services.Logger;
using ReportEngine.App.Services.Notification;
using ReportEngine.App.ViewModels.Utils;
using ReportEngine.App.Views.Windows.Dialog;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Armautre;
using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.CalculationParameters.Enums;
using ReportEngine.Domain.Entities.ElectricSockets;
using ReportEngine.Domain.Entities.Other;
using ReportEngine.Domain.Entities.Pipes;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Domain.Store;
using ReportEngine.Export.DTO;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Extensions.Extensions;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;

namespace ReportEngine.App.ViewModels;

public class ProjectViewModel : BaseViewModel
{
    private readonly AdditionalEquipService _additionalEquipService;
    private readonly AuditService _auditService;
    private readonly ICalculationService _calculationService;
    private readonly ContainerService _containerService;
    private readonly IDialogService _dialogService;
    private readonly EntityStandClonerService _entityStandCloner;
    private readonly ExceptionService _exceptionService;
    private readonly InitializeService _initializeService;
    private readonly UiLogger _logger;
    private readonly INotificationService _notificationService;
    private readonly ParametersStore _parametersStore;
    private readonly IProjectDataLoaderService _projectDataLoaderService;
    private readonly IProjectInfoRepository _projectRepository;
    private readonly IProjectService _projectService;
    private readonly IReportService _reportService;
    private readonly IServiceProvider _serviceProvider;
    private readonly SessionService _sessionService;
    private readonly IStandService _standService;
    private readonly UIValidatorService _uiValidatorService;
    private readonly UpdaterStandService _updaterStandService;

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
        InitializeService initializeService,
        EntityStandClonerService entityStandCloner,
        ParametersStore parametersStore,
        AuditService auditService,
        SessionService sessionService,
        ExceptionService exceptionService,
        UiLogger logger,
        IServiceProvider serviceProvider)
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
        _updaterStandService = updaterStandService;
        _additionalEquipService = additionalEquipService;
        _uiValidatorService = uiValidatorService;
        _initializeService = initializeService;
        _entityStandCloner = entityStandCloner;
        _parametersStore = parametersStore;
        _sessionService = sessionService;
        _auditService = auditService;
        _exceptionService = exceptionService;
        _logger = logger;
        _serviceProvider = serviceProvider;

        NewStand = new StandModel { Number = 1 };

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
    public StandModel NewStand { get; set; } = new();
    public ProjectModel CurrentProjectModel { get; set; } = new();
    public ProjectCommandProvider ProjectCommandProvider { get; set; } = new();
    public MaterialLinesModel CurrentMaterials { get; set; } = new();
    public int MaxObvNN => CurrentProjectModel?.SelectedStand?.ObvyazkiInStand.Max(obv => obv.NN) ?? 0;

    public int MaxStandNN =>
        CurrentProjectModel.Stands.Count > 0 ? CurrentProjectModel.Stands.Max(stand => stand.Number) : 0;

    public bool CanAllCommandsExecute(object? e)
    {
        return true;
    }

    public void OnOpenAllSortamentsDialogExecuted(object e)
    {
        var selected = _dialogService.ShowAllSortamentsDialog(e);

        if (selected == null)
            return;

        ApplySelectedEquipToPurpose(e, selected);
    }

    public async Task OnShowCompanyDialogExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            CurrentProjectModel.Company = _dialogService.ShowCompanyDialog();
        });
    }

    public async Task OnShowSubjectDialogExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
            CurrentProjectModel.Object = _dialogService.ShowSubjectDialog());
    }

    //добавление новой обвязки
    public async Task OnOpenObvSettingsWindowCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            CurrentProjectModel.SelectedStand.ObvyazkaAdditionalComponents.Clear();
            //перед открытием создания обвязки обновляем номер в окне
            UpdateNewObvNn();
            
            _dialogService.ShowObvSettingsWindow(this);
        });
    }

    public async Task OnShowFrameDialogExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            if (Guard.ExitIfNull("Сначала создайте стенд!",
                    _notificationService,
                    CurrentProjectModel.SelectedStand))
                return;

            var totalWidth = _projectService.GetSummWidthObvyzaka(CurrentProjectModel);
            _notificationService.ShowInfo("Рекомендуемая рама: Рама с длиной " + totalWidth);

            if (CurrentProjectModel.SelectedStand == null)
            {
                _notificationService.ShowError("Стенд не выбран!");
                return;
            }

            var selectedFrame = _dialogService.ShowFrameDialog();

            if (selectedFrame == null)
                return;

            await _standService.AddFrameToStandAsync(CurrentProjectModel.SelectedStand.Id, selectedFrame.Id);

            if (selectedFrame.Disassembled == true)
                await DisambledFrameUpdateAsync();

            CurrentProjectModel.SelectedStand.FramesInStand.Add(selectedFrame);

            await OnFramesInStandChanged();
        });
    }

    public async Task OnAdditionalTestCommandExecuted()
    {
        CurrentProjectModel.SelectedStand.AdditionalPurposesChanges = true;

        await _additionalEquipService.CreateEquipsFromObvyzkaAsync(CurrentProjectModel);
    }

    private async Task DisambledFrameUpdateAsync()
    {
        var materialFirstEquip =
            _parametersStore.GetParameterEquip(
                _parametersStore.GetCurrentParameter(CalculationParameterType.Equipments, "MaterialOne"));
        var materialSecondEquip =
            _parametersStore.GetParameterEquip(
                _parametersStore.GetCurrentParameter(CalculationParameterType.Equipments, "MaterialTwo"));
        var materialFirstQuantity = _parametersStore.GetParameterEquip(
            _parametersStore.GetCurrentParameter(CalculationParameterType.Equipments, "MaterialOneQuantity"));
        var materialSecondQuantity = _parametersStore.GetParameterEquip(
            _parametersStore.GetCurrentParameter(CalculationParameterType.Equipments, "MaterialTwoQuantity"));

        var items = new List<AdditionalEquipPurpose>
        {
            new()
            {
                Material = materialFirstEquip.Parameter.Value,
                Quantity = materialFirstQuantity.Parameter.Value.ToFloat(),
                CostPerUnit = materialFirstEquip.Equipment.Cost,
                Measure = "шт",
                FormedAdditionalEquipId = CurrentProjectModel.SelectedStand.AdditionalEquipsInStand.FirstOrDefault().Id
            },
            new()
            {
                Material = materialSecondEquip.Parameter.Value,
                Quantity = materialSecondQuantity.Parameter.Value.ToFloat(),
                CostPerUnit = materialSecondEquip.Equipment.Cost,
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
                    weight => CurrentProjectModel.SelectedStand.MaterialLineWeight = weight);
                break;

            case "Нержавеющие":
                SelectEquipment<StainlessPipe>(
                    name => CurrentProjectModel.SelectedStand.MaterialLine = name,
                    measure => CurrentProjectModel.SelectedStand.MaterialLineMeasure = measure,
                    cost => CurrentProjectModel.SelectedStand.MaterialLineCostPerUnit = cost,
                    exportDays => CurrentProjectModel.SelectedStand.MaterialLineExportDays = exportDays,
                    weight => CurrentProjectModel.SelectedStand.MaterialLineWeight = weight);
                break;

            case "Углеродистые":
                SelectEquipment<CarbonPipe>(
                    name => CurrentProjectModel.SelectedStand.MaterialLine = name,
                    measure => CurrentProjectModel.SelectedStand.MaterialLineMeasure = measure,
                    cost => CurrentProjectModel.SelectedStand.MaterialLineCostPerUnit = cost,
                    exportDays => CurrentProjectModel.SelectedStand.MaterialLineExportDays = exportDays,
                    weight => CurrentProjectModel.SelectedStand.MaterialLineWeight = weight);
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
                    weight => CurrentProjectModel.SelectedStand.ArmatureWeight = weight);
                break;

            case "Нержавеющие":
                SelectEquipment<StainlessArmature>(
                    name => CurrentProjectModel.SelectedStand.Armature = name,
                    measure => CurrentProjectModel.SelectedStand.ArmatureMeasure = measure,
                    cost => CurrentProjectModel.SelectedStand.ArmatureCostPerUnit = cost,
                    exportDays => CurrentProjectModel.SelectedStand.ArmatureExportDays = exportDays,
                    weight => CurrentProjectModel.SelectedStand.ArmatureWeight = weight);
                break;

            case "Углеродистые":
                SelectEquipment<CarbonArmature>(
                    name => CurrentProjectModel.SelectedStand.Armature = name,
                    measure => CurrentProjectModel.SelectedStand.ArmatureMeasure = measure,
                    cost => CurrentProjectModel.SelectedStand.ArmatureCostPerUnit = cost,
                    exportDays => CurrentProjectModel.SelectedStand.ArmatureExportDays = exportDays,
                    weight => CurrentProjectModel.SelectedStand.ArmatureWeight = weight);
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
                    weight => CurrentProjectModel.SelectedStand.TreeSocketWeight = weight);
                break;

            case "Нержавеющие":
                SelectEquipment<StainlessSocket>(
                    name => CurrentProjectModel.SelectedStand.TreeSocket = name,
                    measure => CurrentProjectModel.SelectedStand.TreeSocketMaterialMeasure = measure,
                    cost => CurrentProjectModel.SelectedStand.TreeSocketMaterialCostPerUnit = cost,
                    exportDays => CurrentProjectModel.SelectedStand.TreeSocketExportDays = exportDays,
                    weight => CurrentProjectModel.SelectedStand.TreeSocketWeight = weight);
                break;

            case "Углеродистые":
                SelectEquipment<CarbonSocket>(
                    name => CurrentProjectModel.SelectedStand.TreeSocket = name,
                    measure => CurrentProjectModel.SelectedStand.TreeSocketMaterialMeasure = measure,
                    cost => CurrentProjectModel.SelectedStand.TreeSocketMaterialCostPerUnit = cost,
                    exportDays => CurrentProjectModel.SelectedStand.TreeSocketExportDays = exportDays,
                    weight => CurrentProjectModel.SelectedStand.TreeSocketWeight = weight);
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
                    weight => CurrentProjectModel.SelectedStand.KMCHWeight = weight);
                break;

            case "Нержавеющие":
                SelectEquipment<StainlessSocket>(
                    name => CurrentProjectModel.SelectedStand.KMCH = name,
                    measure => CurrentProjectModel.SelectedStand.KMCHMeasure = measure,
                    cost => CurrentProjectModel.SelectedStand.KMCHCostPerUnit = cost,
                    exportDays => CurrentProjectModel.SelectedStand.KMCHExportDays = exportDays,
                    weight => CurrentProjectModel.SelectedStand.KMCHWeight = weight);
                break;

            case "Углеродистые":
                SelectEquipment<CarbonSocket>(
                    name => CurrentProjectModel.SelectedStand.KMCH = name,
                    measure => CurrentProjectModel.SelectedStand.KMCHMeasure = measure,
                    cost => CurrentProjectModel.SelectedStand.KMCHCostPerUnit = cost,
                    exportDays => CurrentProjectModel.SelectedStand.KMCHExportDays = exportDays,
                    weight => CurrentProjectModel.SelectedStand.KMCHWeight = weight);
                break;
        }

        _standService.UpdateStandWeight(CurrentProjectModel.SelectedStand);
    }

    public async Task OnCreateNewCardCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            var isCorrectProjNumber = _uiValidatorService.ValidateCorrectProjNN(CurrentProjectModel.Number);

            if (!isCorrectProjNumber)
                return;

            var isFreeProjNumber =
                await _uiValidatorService.ValidateFreeProjNN(this, CurrentProjectModel.Number, false);

            if (!isFreeProjNumber)
                return;

            var isCorrectStatus = _uiValidatorService.ValidateProjectStatus(this);

            if (!isCorrectStatus)
                return;

            await CreateNewProjectCardAsync();
            await _projectService.GetOrAddCompanyAsync(CurrentProjectModel.Company);
            await _projectService.GetOrAddSubjectAsync(CurrentProjectModel.Object, CurrentProjectModel.Company);

            await _auditService.LogEventAsync(
                _sessionService.CurrentUser.UserLogin,
                $"Пользователь {_sessionService.CurrentUser.UserLogin} создал проект {CurrentProjectModel.OrderCustomer}",
                $"Создание проекта, заказ покупателя: {CurrentProjectModel.OrderCustomer}");
        });
    }

    public async Task OnOpenCreateNewStandCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            _dialogService.ShowStandsSettingsWindow(this, false);
        });
    }

    public async Task OnOpenEditStandCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            _dialogService.ShowEditStandsObvSettingsWindow(this, CurrentProjectModel.SelectedStand, true);
        });
    }

    public async Task OnAddNewStandCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(AddNewStandToProjectAsync);
    }

    public async Task OnCopyStandsCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
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

    public async Task OnDeleteSelectedStandFromProjectExecuted()
    {
        await _exceptionService.SafeExecuteAsync(DeleteStandFromProject);
    }

    public async Task OnDeleteSelectedStandsCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            foreach (var stand in StandsListHelper.SelectedStands)
                await _projectService.DeleteStandAsync(CurrentProjectModel.CurrentProjectId, stand.Id);

            await _projectDataLoaderService.LoadAllProjectStandsAsync(CurrentProjectModel.CurrentProjectId, this);

            _notificationService.ShowInfo("Стенды удалены из проекта!");
        });
    }

    public async Task OnSaveChangesCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
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

    public async Task OnAddObvCommandExecuted()
    {
        var selectedStand = CurrentProjectModel?.SelectedStand;

        if (Guard.ExitIfNull("Не был выбран стенд!", _notificationService, selectedStand))
            return;

        if (Guard.ExitIfNull("Не был выбран тип обвязки!", _notificationService, SelectedObvyazka))
            return;


        var correctNN = _uiValidatorService.ValidateCorrectObvNN(selectedStand.NN);

        if (!correctNN)
            return;

        var freeNN = _uiValidatorService.ValidateFreeObvNN(this, selectedStand.NN, false);

        if (!freeNN)
            return;

        //Валидация по количеству датчиков в обвязке
        //var isCorrectSensorsData = _uiValidatorService.ValidateSensorsQuantityInNewObv(this);

        //if (!isCorrectSensorsData)
        //   return;

        await _exceptionService.SafeExecuteAsync(async () =>
        {
            var entity = await _standService.CreateObvyazkaAsync(selectedStand, SelectedObvyazka);

            if (Guard.ExitIfNull("Не удалось создать обвязку!", _notificationService, entity))
                return;

            await _standService.AddObvyazkaToStandAsync(selectedStand.Id, entity);


            //сравнение по типу
            var isAlreadyExist =
                CurrentProjectModel.ObvyazkiInProject.Any(obv => obv.ObvyazkaName == entity.ObvyazkaName);

            if (!isAlreadyExist)
                CurrentProjectModel.ObvyazkiInProject.Add(entity);

            CollectionRefreshHelper.SafeRefreshCollection(
                CurrentProjectModel.SelectedStand.ObvyazkaAdditionalComponents);

            await LoadObvyazkiAsync(); // Перезагрузить данные из БД

            UpdateNewObvNn();
            await OnObvyazkiInStandChanged();
        });

        _notificationService.ShowInfo("Обвязка добавлена в стенд");
    }

    public async Task OnDeleteAdditionalEquipFromObvCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
            await _standService.DeleteAdditionalPurposeFromObvAsync(
                CurrentProjectModel.SelectedStand.SelectedObvyazkaAdditionalEquipPurpose,
                CurrentProjectModel.SelectedStand));
    }


    //TODO: перенести в отдельный  метод
    public async Task OnUpdateAdditionalEquipFromObvCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            var stand = CurrentProjectModel.SelectedStand;

            if (stand == null)
            {
                _notificationService.ShowError("Стенд не выбран!");
                return;
            }

            if (stand.SelectedObvyazkaInStand == null)
            {
                _notificationService.ShowError("Обвязка не выбрана!");
                return;
            }

            if (stand.SelectedObvyazkaInStand.Id == 0)
            {
                _notificationService.ShowError("Сначала сохраните обвязку!");
                return;
            }

            var obvComponents = stand.ObvyazkaAdditionalComponents.ToList();

            foreach (var obvComponent in obvComponents)
            {
                if (obvComponent.Id == 0) obvComponent.ObvyazkaInStandId = stand.SelectedObvyazkaInStand?.Id;

                await _standService.UpdateAdditionalPurposeFromObvAsync(obvComponent,
                    obvComponent.ObvyazkaInStandId ?? 0);
            }

            _notificationService.ShowInfo("Все комплектующие обвязок сохранены");
        });
    }

    public async Task OnRemoveObvCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(DeleteObvFromStandAsync);
    }

    public async Task OnRemoveFrameFromStandCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            await _projectService.DeleteFrameFromStandAsync(CurrentProjectModel);

            await OnFramesInStandChanged();

            _notificationService.ShowInfo("Рама удалена из стенда");
        });
    }

    public async Task OnUpdateStandsAfterEquipsCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            await _updaterStandService.ApplyChangesAndSaveAsync(CurrentProjectModel);
            await _calculationService.CalculateProjectAsync(CurrentProjectModel);
        });
    }

    public async Task OnAddDrainageToStandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(AddDrainageToStandAsync);
    }

    public async Task OnAddFrameToStandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(AddFrameToStandAsync);
    }

    public async Task OnCopyObvyazkaToStandsCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
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

            await OnObvyazkiInStandChanged();

            _notificationService.ShowInfo("Обвязка успешно добавлена в стенд!");
        });
    }

    public void OnSelectObvCommandExecuted(object p)
    {
        _exceptionService.SafeExecute(() =>
        {
            SelectedObvyazka = _dialogService.ShowObvyazkaDialog(true);

            //если не выбрали - просто выходим
            if (SelectedObvyazka == null)
                return;

            var stand = CurrentProjectModel.SelectedStand;


            stand.ObvWeight = SelectedObvyazka.Weight;
            stand.MaterialLineCount = SelectedObvyazka.LineLength;
            stand.ArmatureCount = SelectedObvyazka.ZraCount;
            stand.TreeSocketMaterialCount = SelectedObvyazka.TreeSocket;
            stand.KMCHCount = SelectedObvyazka.KMCHCount;
            stand.ImageName = SelectedObvyazka.ImageName;
        });
    }

    public async Task OnCalculateProjectCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(CalculateProjectAsync);
    }

    public async Task OnSaveChangesInStandCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(SaveChangesInStandAsync);
    }

    public async Task OnSaveAllChangesInComponentsCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
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

    public async Task OnDeleteElectricalComponentFromStandCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            await DeletePurposeAsync(CurrentProjectModel.SelectedStand.SelectedElectricalComponent,
                _standService.DeleteElectricalPurposeAsync,
                CurrentProjectModel.SelectedStand.AllElectricalPurposesInStand,
                "Электрический компонент удалён");
        });
    }

    public async Task OnUpdateElectricalComponentInStandCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            var stand = CurrentProjectModel.SelectedStand;

            if (Guard.ExitIfNull("Стенд не выбран!", _notificationService, stand))
                return;

            foreach (var purpose in stand.AllElectricalPurposesInStand.ToList())
            {
                if (purpose.Id == 0)
                {
                    var firstComponent = stand.AllElectricalPurposesInStand.FirstOrDefault();

                    if (firstComponent != null)
                        purpose.FormedElectricalComponentId = firstComponent.FormedElectricalComponentId;
                }

                await _standService.UpdateElectricalPurposeAsync(purpose);
            }

            CurrentProjectModel.SelectedStand.ElectricalPurposesChanges = false;
            OnPropertyChanged(nameof(CurrentProjectModel.SelectedStand.ElectricalPurposesChanges));

            _notificationService.ShowInfo("Все электрические компоненты сохранены");
        });
    }

    public async Task OnDeleteAdditionalComponentFromStandCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            await DeletePurposeAsync(CurrentProjectModel.SelectedStand.SelectedAdditionalEquip,
                _standService.DeleteAdditionalPurposeAsync,
                CurrentProjectModel.SelectedStand.AllAdditionalEquipPurposesInStand,
                "Доп. комплектующее удалено возврат");
        });
    }

    public async Task OnUpdateAdditionalComponentInStandCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            var stand = CurrentProjectModel.SelectedStand;

            if (Guard.ExitIfNull("Стенд не выбран!", _notificationService, stand))
                return;

            foreach (var purpose in stand.AllAdditionalEquipPurposesInStand.ToList())
            {
                if (purpose.Id == 0)
                {
                    var firstComponent = stand.AllAdditionalEquipPurposesInStand.FirstOrDefault();
                    if (firstComponent != null)
                        purpose.FormedAdditionalEquipId = firstComponent.FormedAdditionalEquipId;
                }

                await _standService.UpdateAdditionalPurposeAsync(purpose);
            }

            stand.AdditionalPurposesChanges = false;
            OnPropertyChanged(nameof(stand.AdditionalPurposesChanges));

            _notificationService.ShowInfo("Все доп. комплектующие сохранены");
        });
    }

    public async Task OnAddStandFromAllStandsCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            var selectedStandEntity = _dialogService.ShowSelectStandDialog();

            if (selectedStandEntity == null) return;

            await _dialogService.RunWithProgressDialogAsync(async () =>
            {
                var newStand = await _entityStandCloner.CloneStandEntity(selectedStandEntity);

                newStand.Number = MaxStandNN + 1;

                await _projectRepository.AddStandAsync(CurrentProjectModel.CurrentProjectId, newStand);

                var convertedStandModel = StandDataConverter.ConvertToStandModel(newStand);

                CurrentProjectModel.Stands.Add(convertedStandModel);

                //подгружаем все данные нового стенда
                await _standService.LoadStandsDataAsync([convertedStandModel]);
                await _standService.LoadObvyazkiInStandsAsync([convertedStandModel]);
                await _standService.LoadPurposesInStands([convertedStandModel]);

                CurrentProjectModel.SelectedStand = convertedStandModel;
            });

            _notificationService.ShowInfo("Стенд успешно добавлен!");
        });
    }

    public async Task OnDeleteDrainageComponentFromStandCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            await DeletePurposeAsync(CurrentProjectModel.SelectedStand.SelectedDrainagePurpose,
                _standService.DeleteDrainagePurposeAsync,
                CurrentProjectModel.SelectedStand.AllDrainagePurposesInStand,
                "Дренажное комплектующее удалено");
        });
    }

    public async Task OnUpdateDrainageComponentInStandCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            var stand = CurrentProjectModel.SelectedStand;

            if (Guard.ExitIfNull("Стенд не выбран!", _notificationService, stand))
                return;

            var purposes = stand.AllDrainagePurposesInStand.ToList();

            foreach (var purpose in purposes)
            {
                if (purpose.Id == 0)
                {
                    var firstDrainage = stand.AllDrainagePurposesInStand.FirstOrDefault();
                    if (firstDrainage != null)
                        purpose.FormedDrainageId = firstDrainage.FormedDrainageId;
                }

                await _standService.UpdateDrainagePurposeAsync(purpose);
            }

            CurrentProjectModel.SelectedStand.DrainagePurposesChanges = false;
            OnPropertyChanged(nameof(CurrentProjectModel.SelectedStand.DrainagePurposesChanges));

            _notificationService.ShowInfo("Все дренажные компоненты сохранены");
        });
    }

    public async Task OnEditObvSettingsCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            _dialogService.ShowEditObvSettingsWindow(this,
                CurrentProjectModel.SelectedStand,
                CurrentProjectModel.SelectedStand.SelectedObvyazkaInStand);
        });
    }

    public async Task OnFillObvFieldsCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            _standService.FillStandFieldsFromObvyazka(CurrentProjectModel.SelectedStand,
                CurrentProjectModel.SelectedObvyazkaToCopy);
        });
    }

    //TODO: вынести в standService
    public async Task OnFillStandFieldsFromSelectedStandCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
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

    public async Task OnRenumerateStandsCommandExecuted()
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

        var iteration = 1;

        foreach (var stand in renumeratedStand)
        {
            var iterPart = renumInfo.StartValue.Value + (iteration - 1) * renumInfo.Step.Value;
            var formattedIterPart = iterPart.ToString().PadLeft(renumInfo.StartValueLength, '0');


            stand.SerialNumber = $"{renumInfo.Prefix}{formattedIterPart}{renumInfo.Postfix}";

            var newStandEntity = StandDataConverter.ConvertToStandEntity(stand);
            standEntities.Add(newStandEntity);

            iteration++;
        }

        await _projectRepository.UpdateStandsGroupAsync(standEntities);

        _notificationService.ShowInfo("Стенды пронумерованы");
    }

    public async Task OnUpdateObvInStandCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            var selectedStand = CurrentProjectModel?.SelectedStand;

            if (selectedStand == null)
                return;

            var correctNN = _uiValidatorService.ValidateCorrectObvNN(selectedStand.NN);

            if (!correctNN)
                return;

            var freeNN = _uiValidatorService.ValidateFreeObvNN(this, selectedStand.NN, true);

            if (!freeNN)
                return;

            //Валидация по количеству датчиков в обвязке
            // var isCorrectSensorsData = _uiValidatorService.ValidateSensorsQuantityInNewObv(this);

            // if (!isCorrectSensorsData)
            //   return;


            //TODO: здесь бы по хорошему встроить сохранение всех доп комплектующих в обвязке

            await _projectService.UpdateObvInStandAsync(CurrentProjectModel);

            await OnObvyazkiInStandChanged();
            OnPropertyChanged(nameof(CurrentProjectModel.SelectedStand.NewAdditionalEquip.Purposes));
            OnPropertyChanged(nameof(CurrentProjectModel.SelectedStand.NewElectricalComponent.Purposes));
        });
    }


    public async Task OnFillMarkInObvCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            var proj = CurrentProjectModel;
            var selectedStand = proj.SelectedStand;

            if (selectedStand == null) return;

            var projectHasMarkPlus = !string.IsNullOrEmpty(proj.MarkPlus);
            var projectHasMarkMinus = !string.IsNullOrEmpty(proj.MarkMinus);

            //если совсем нет маркировки в проекте
            if (!projectHasMarkPlus && !projectHasMarkMinus)
            {
                _notificationService.ShowError("Отсутствует маркировка в проекте!");
                return;
            }

            var firstSensorHasKKS = !string.IsNullOrEmpty(selectedStand.FirstSensorKKS);
            var secondSensorHasKKS = !string.IsNullOrEmpty(selectedStand.SecondSensorKKS);
            var thirdSensorHasKKS = !string.IsNullOrEmpty(selectedStand.ThirdSensorKKS);

            if (projectHasMarkPlus)
            {
                selectedStand.FirstSensorMarkPlus = firstSensorHasKKS
                    ? selectedStand.FirstSensorKKS + proj.MarkPlus
                    : selectedStand.FirstSensorMarkPlus;

                selectedStand.SecondSensorMarkPlus = secondSensorHasKKS
                    ? selectedStand.SecondSensorKKS + proj.MarkPlus
                    : selectedStand.SecondSensorMarkPlus;


                selectedStand.ThirdSensorMarkPlus = thirdSensorHasKKS
                    ? selectedStand.ThirdSensorKKS + proj.MarkPlus
                    : selectedStand.ThirdSensorMarkPlus;
            }

            if (projectHasMarkMinus)
            {
                selectedStand.FirstSensorMarkMinus = firstSensorHasKKS
                    ? selectedStand.FirstSensorKKS + proj.MarkMinus
                    : selectedStand.FirstSensorMarkMinus;

                selectedStand.SecondSensorMarkMinus = secondSensorHasKKS
                    ? selectedStand.SecondSensorKKS + proj.MarkMinus
                    : selectedStand.SecondSensorMarkMinus;

                selectedStand.ThirdSensorMarkMinus = thirdSensorHasKKS
                    ? selectedStand.ThirdSensorKKS + proj.MarkMinus
                    : selectedStand.ThirdSensorMarkMinus;
            }
        });
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

    #region Отчеты по выбранным стендам

    public async Task OnCreateSelectedStandsComponentsListReportCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(() =>
            CreateReportAsync(ReportType.ComponentsListReport, "Ведомость комплектующих",
                StandsListHelper.SelectedStands));
    }

    public async Task OnCreateSelectedStandsSummaryReportCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(() =>
            CreateReportAsync(ReportType.SummaryReport, "Сводная ведомость", StandsListHelper.SelectedStands));
    }

    public async Task OnCreateSelectedStandsMarksReportCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(() =>
            CreateReportAsync(ReportType.MarksReport, "Ведомость маркировки", StandsListHelper.SelectedStands));
    }

    public async Task OnCreateSelectedStandsNameplatesReportCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(() =>
            CreateReportAsync(ReportType.NameplatesReport, "Ведомость шильдиков и табличек",
                StandsListHelper.SelectedStands));
    }

    public async Task OnCreateSelectedStandsContainerReportCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(() =>
            CreateReportAsync(ReportType.ContainerReport, "Тара", StandsListHelper.SelectedStands));
    }

    public async Task OnCreateSelectedStandsProductionReportCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(() =>
            CreateReportAsync(ReportType.ProductionReport, "Ведомость производства", StandsListHelper.SelectedStands));
    }

    public async Task OnCreateSelectedStandsFinplanReportCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(() =>
            CreateReportAsync(ReportType.FinPlanReport, "Финансовый план", StandsListHelper.SelectedStands));
    }

    public async Task OnCreateSelectedStandsPassportReportCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(() =>
            CreateReportAsync(ReportType.PassportsReport, "Паспорт", StandsListHelper.SelectedStands));
    }

    public async Task OnCreateSelectedStandsTechnologicalCardsCommandExecute()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
            await CreateReportAsync(ReportType.TechnologicalCards, "Технологические карты",
                StandsListHelper.SelectedStands));
    }

    #endregion

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

    #endregion Инициализация

    #region Методы загрузки данных на view

    public async Task LoadStandsDataAsync()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            await _standService.LoadStandsDataAsync(CurrentProjectModel.Stands);
        });
    }

    public async Task LoadPurposesInStandsAsync()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
            await _standService.LoadPurposesInStands(CurrentProjectModel.Stands));
    }

    public async Task LoadObvyazkiAsync()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            await _standService.LoadObvyazkiInStandsAsync(CurrentProjectModel.Stands);
            await _projectService.LoadAllObvyazkiInProject(CurrentProjectModel);
        });
    }

    public async Task LoadAllAvaileDataAsync()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            await _projectDataLoaderService.LoadAllAvailDataToViewModelAsync(this);
        });
    }

    public async Task LoadProjectInfoAsync(int projectId)
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            var loadedModel = await _projectService.LoadProjectInfoAsync(projectId);
            if (loadedModel == null)
                return;

            CurrentProjectModel = loadedModel;
            CurrentStandModel = loadedModel.SelectedStand ?? new StandModel();

            await LoadObvyazkiAsync();
            await LoadStandsDataAsync();
            await LoadPurposesInStandsAsync();

            OnPropertyChanged(nameof(CurrentStandModel));
        });
    }

    public async Task LoadContainersInfoAsync()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
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

        if (!isAlreadyExist) CurrentProjectModel.ObvyazkiInProject.Add(entity);

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

        await OnObvyazkiInStandChanged();

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

        await _auditService.LogEventAsync(
            _sessionService.CurrentUser.UserLogin,
            $"Пользователь {_sessionService.CurrentUser.UserLogin} сохранил изменения в проекте",
            $"Сохранил изменения в проекте, заказ покупателя:{CurrentProjectModel.OrderCustomer}");

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

        //костылек - после создания стенда данные по доп комплектующим не были синхронизированы
        //после создания стенда тут же запрашиваем обновленные данные по доп комплектующими
        await _standService.LoadStandsDataAsync([newStandModel]);

        UpdateNewStandNn();

        OnPropertyChanged(nameof(CurrentStandModel));
        OnPropertyChanged(nameof(NewStand));

        OnStandsInProjectChanged();

        _notificationService.ShowInfo("Стенд успешно добавлен!");

        await _auditService.LogEventAsync(
            _sessionService.CurrentUser.UserLogin,
            $"Пользователь {_sessionService.CurrentUser.UserLogin} добавил стенд в проект {addedStandEntity.KKSCode}",
            $"Добавление стенда в проект, заказ покупателя:{CurrentProjectModel.OrderCustomer}");
    }

    private async Task CreateDefaultPurposesAsync(StandModel newStandModel)
    {
        await _initializeService.InitializeStandDefaultPurposes(newStandModel);

        newStandModel.NewElectricalComponent.Purposes =
            CurrentProjectModel.SelectedStand.AllElectricalPurposesInStand.ToList();
        newStandModel.NewDrainage.Purposes = CurrentProjectModel.SelectedStand.AllDrainagePurposesInStand.ToList();
        newStandModel.NewAdditionalEquip.Purposes =
            CurrentProjectModel.SelectedStand.AllAdditionalEquipPurposesInStand.ToList();

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

        await OnStandsInProjectChanged();
        UpdateNewStandNn();

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

        _notificationService.ShowInfo("Стенд удалён из проекта");

        UpdateNewStandNn();
        await OnStandsInProjectChanged();

        await _auditService.LogEventAsync(
            _sessionService.CurrentUser.UserLogin,
            $"Пользователь {_sessionService.CurrentUser.UserLogin} удалил стенд из проекта {selected.KKSCode}",
            $"Удаление стенда из проект, заказ покупателя:{CurrentProjectModel.OrderCustomer}");
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
        _exceptionService.SafeExecute(() =>
        {
            var equipment = _dialogService.ShowEquipDialog<T>();
            if (equipment != null && CurrentProjectModel.SelectedStand != null)
            {
                setProperty(equipment.Name);
                setMeasure(equipment.Measure);
                setCost(equipment.Cost.ToString());
                setExportDays((int)equipment.ExportDays);
            }

            if (equipment is BaseEquip baseEquip) setWeight((float)baseEquip.Weight);
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
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            var selectedStand = CurrentProjectModel.SelectedStand;

            if (selectedStand == null)
            {
                _notificationService.ShowError("Стенд не выбран");
                return;
            }


            var obvNumber = 1;

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

        await _exceptionService.SafeExecuteAsync(async () =>
        {
            await _projectService.UpdateStandEntity(CurrentProjectModel);
            _notificationService.ShowInfo("Чертёж стенда сохранён");
        });
    }

    private void ApplySelectedEquipToPurpose(object target, IBaseEquip selected)
    {
        _exceptionService.SafeExecute(() =>
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
                    CollectionRefreshHelper.SafeRefreshCollection(CurrentProjectModel.SelectedStand
                        .ObvyazkaAdditionalComponents);
                    return;

                case ContainerStand cs:
                    if (selected is Container c)
                    {
                        cs.Name = c.Name;
                        cs.ContainerCost = c.Cost;
                        cs.ContainerWeight = c.Weight;
                        CollectionRefreshHelper.SafeRefreshCollection(
                            CurrentProjectModel.ContainerStandsInSelectedBatch);
                        return;
                    }

                    cs.Name = selected.Name;
                    cs.ContainerCost = selected.Cost;
                    cs.ContainerWeight = selected.Weight;
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


        //если тех карты - вызываем доп окно
        if (typeGenerator == ReportType.TechnologicalCards)
        {
            var reportTypeWindow = new TechCardElecrticDialog
            {
                Owner = Application.Current.MainWindow
            };
            var dialogResult = reportTypeWindow.ShowDialog();

            //если пользователь что-то выбрал
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

        // Генерация отчета — перегрузка в _reportService разберётся сама
        await _dialogService.RunWithProgressDialogAsync(() =>
            _reportService.GenerateReportAsync(typeGenerator, CurrentProjectModel.CurrentProjectId, selectedStands));

        if (_notificationService.ShowConfirmation(
                $"Отчёт \"{reportName}\" по выбранным стендам создана!\nОткрыть папку с отчётами?"))
        {
            var reportDir = JsonHandler.GetSaveReportDirectory(DirectoryHelper.GetConfigPath());
            Process.Start("explorer.exe", reportDir);
        }
    }

    #endregion Методы расчёта и создания отчётности

    #region Обновление UI

    public async Task OnObvyazkiInStandChanged()
    {
        Debug.WriteLine("Обвязки поменялись");

        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;


        await BackgroundExecutor.ExecuteAsync(() =>
        {
            UpdateTablesQuantity();
            UpdateClampsQuantity();
            UpdateBracketsQuantity();
            UpdateElectricEquipment();

            selectedStand.StandSensorsQuantity =
                selectedStand.CountElectricSensorsQuantity();
        });

        CollectionRefreshHelper.SafeRefreshCollection(selectedStand.AllElectricalPurposesInStand);
        CollectionRefreshHelper.SafeRefreshCollection(selectedStand.AllAdditionalEquipPurposesInStand);

        CollectionRefreshHelper.SafeSortAndRefreshCollection(
            selectedStand.ObvyazkiInStand,
            "NN",
            false);


        selectedStand.StandSensorsQuantity = selectedStand.CountElectricSensorsQuantity();
    }

    public async Task OnFramesInStandChanged()
    {
        Debug.WriteLine("Рамы поменялись");

        await BackgroundExecutor.ExecuteAsync(() =>
        {
            UpdateChannelsQuantity();
            UpdateDrainage();
        });
    
        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        CollectionRefreshHelper.SafeRefreshCollection(selectedStand.AllAdditionalEquipPurposesInStand);
        CollectionRefreshHelper.SafeRefreshCollection(selectedStand.AllDrainagePurposesInStand);
    }

    public async void OnSelectedStandChanged()
    {
        Debug.WriteLine("Выбранный стенд изменился");

        await OnFramesInStandChanged();
        await OnObvyazkiInStandChanged();
        UpdateBracketsQuantity();

        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        selectedStand.AdditionalPurposesChanges = false;
        selectedStand.ElectricalPurposesChanges = false;
        selectedStand.DrainagePurposesChanges = false;
    }

    public async Task OnStandsInProjectChanged()
    {
        Debug.WriteLine("Стенды изменились");

        //отсортировываем по возрастанию номера
        CollectionRefreshHelper.SafeSortAndRefreshCollection(
            CurrentProjectModel.Stands,
            "Number",
            false);


        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        await BackgroundExecutor.ExecuteAsync(UpdateChannelsQuantity);

        CollectionRefreshHelper.SafeRefreshCollection(selectedStand.AllAdditionalEquipPurposesInStand);
    }

    //обновляем поле NN в обвязке
    private void UpdateNewObvNn()
    {
        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        selectedStand.NN = MaxObvNN + 1;
    }

    //обновляем № п/п стенда
    private void UpdateNewStandNn()
    {
        if (NewStand == null) return;

        NewStand.Number = MaxStandNN + 1;
    }

    //обновляем кол-во швеллера
    private void UpdateChannelsQuantity()
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
    private void UpdateClampsQuantity()
    {
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
    private void UpdateTablesQuantity()
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
    private void UpdateBracketsQuantity()
    {
        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        const int bracketsPerDifSensor = 1;

        var difSensorsQuantity = selectedStand.CountDifSensorsQuantity();

        var additionalComponents = selectedStand.AllAdditionalEquipPurposesInStand;
        var difSensorsBracketRecord =
            additionalComponents.FirstOrDefault(purpose => purpose.Purpose == "Кронштейн перепадчика");

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

            var absSensorsBracketsRecord =
                additionalComponents.FirstOrDefault(purpose => purpose.Purpose == "Кронштейн абсолютника");

            if (absSensorsBracketsRecord != null && absSensorsBracketsRecord.IsAutoCalculationEnabled == true)
            {
                absSensorsBracketsRecord.Quantity = bracketsPerAbsoluteSensor * absSensorsQuantity;

                selectedStand.AdditionalPurposesChanges = true;
            }
        }

        const int universalBracketQuantity = 2;

        if (!string.IsNullOrEmpty(standBraceType) && standBraceType == "Швеллер")
        {
            var universalBracketRecord =
                additionalComponents.FirstOrDefault(purpose => purpose.Purpose == "Кронштейн универсальный");

            if (universalBracketRecord != null && universalBracketRecord.IsAutoCalculationEnabled == true)
            {
                universalBracketRecord.Quantity = universalBracketQuantity;

                selectedStand.AdditionalPurposesChanges = true;
            }
        }
    }

    //обновляем данные по дренажу
    private void UpdateDrainage()
    {
        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        var drainageParts = selectedStand.AllDrainagePurposesInStand;

        var mainPipeRecord = drainageParts.FirstOrDefault(part => part.Purpose == "Основная труба");
        var pipeBranch = drainageParts.FirstOrDefault(part => part.Purpose == "Патрубок");
        var plugMainPipeRecord = drainageParts.FirstOrDefault(part => part.Purpose == "Заглушка основной трубы");

        if (mainPipeRecord?.IsAutoCalculationEnabled == true)
        {
            mainPipeRecord.Quantity = selectedStand.FramesInStand.Sum(frame => frame.Width) / 1000.0f;

            selectedStand.DrainagePurposesChanges = true;
        }

        if (pipeBranch?.IsAutoCalculationEnabled == true)
        {
            pipeBranch.Quantity = 0.2f * selectedStand.FramesInStand.Count;

            selectedStand.DrainagePurposesChanges = true;
        }

        if (plugMainPipeRecord?.IsAutoCalculationEnabled == true)
        {
            plugMainPipeRecord.Quantity = 2 * selectedStand.FramesInStand.Count;

            selectedStand.DrainagePurposesChanges = true;
        }
    }

    //обновляем данные по электрике
    private void UpdateElectricEquipment()
    {
        Debug.WriteLine("Пересчет электрики начат");

        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        var electricComponents = selectedStand.AllElectricalPurposesInStand;

        //кабельные ввода
        const int cableInputsPerSensor = 2;
        var cableInputsRecord = electricComponents.FirstOrDefault(purpose => purpose.Purpose == "Кабельные вводы");


        var sensorsQuantity = selectedStand.CountElectricSensorsQuantity();

        var cableInputsQuantity = 0;

        if (cableInputsRecord != null)
        {
            if (cableInputsRecord.IsAutoCalculationEnabled == true)
            {
                cableInputsQuantity = sensorsQuantity * cableInputsPerSensor;
                cableInputsRecord.Quantity = cableInputsQuantity;

                selectedStand.ElectricalPurposesChanges = true;
            }
            else
            {
                cableInputsQuantity = (int)(cableInputsRecord.Quantity ?? 0.0);
            }
        }


        //сигнальный кабель

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
            fourMmCableRecord.Quantity = cableInputsQuantity;

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