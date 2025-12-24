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
using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.ElectricSockets;
using ReportEngine.Domain.Entities.Pipes;
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

    public bool CanAllCommandsExecute(object? e)
    {
        return true;
    }

    public async void OnUpdateUICommandExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(UpdateUI);
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
            var companyName = _dialogService.ShowCompanyDialog();

            CurrentProjectModel.Company = companyName;
        });
    }

    public async void OnShowSubjectDialogExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var subjectName = _dialogService.ShowSubjectDialog();

            CurrentProjectModel.Object = subjectName;
        });
    }

    public async void OnShowFrameDialogExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var totalWidth = _projectService.GetSummWidthObvyzakaAsync(CurrentProjectModel);
            _notificationService.ShowInfo("Рекомендуемая рама: Рама с длиной " + totalWidth);

            var selectedFrame = _dialogService.ShowFrameDialog();

            if (Guard.ExitIfNull("Рама или стенд не выбраны!",
                           _notificationService,
                           selectedFrame,
                           CurrentProjectModel.SelectedStand)) return;

            await _standService.AddFrameToStandAsync(CurrentProjectModel.SelectedStand.Id, selectedFrame.Id);
            CurrentProjectModel.SelectedStand.FramesInStand.Add(selectedFrame);

            OnFramesInStandChanged();
        });
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
            await CreateNewProjectCardAsync();
            await _projectService.GetOrAddCompnayAsync(CurrentProjectModel.Company);
            await _projectService.GetOrAddSubjectAsync(CurrentProjectModel.Object, CurrentProjectModel.Company);
        });
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
            if (Guard.ExitIfNull("Стенды для копирования не выбраны!", _notificationService, CurrentProjectModel.SelectedStand))
                return;

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
        await ExceptionHelper.SafeExecuteAsync(SaveProjectChangesAsync);
    }

    public async void OnSaveObvCommandExecuted(object e)
    {
        if (Guard.ExitIfNull("Не был выбран тип обвязки", _notificationService, SelectedObvyazka))
            return;

        var selectedStand = CurrentProjectModel?.SelectedStand;

        if (Guard.ExitIfNull("Не был выбран стенд", _notificationService, selectedStand))
            return;

        //проверка введенного NN обвязки
        if (!ValidateCorrectObvNN(selectedStand.NN))
            return;

        if (!ValidateNotExistingObvNN(selectedStand.NN, false))
            return;

        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await AddObvToStandAsync();
            await LoadObvyazkiAsync(); // Перезагрузить данные из БД

            OnObvyazkiInStandChanged();
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
            newObvyazka.NN = MaxObvNN + 1;

            await _standService.AddObvyazkaToStandAsync(standId, newObvyazka);

            OnPropertyChanged(nameof(CurrentProjectModel.SelectedStand.ObvyazkiInStand));
            OnPropertyChanged(nameof(CurrentProjectModel.ObvyazkiInProject));

            await LoadObvyazkiAsync();

            OnObvyazkiInStandChanged();

            _notificationService.ShowInfo("Обвязка скопирована в стенд");
        });
    }

    public void OnSelectObvCommandExecuted(object p)
    {
        var selectedStand = CurrentProjectModel.SelectedStand;

        ExceptionHelper.SafeExecute(() =>
        {
            SelectedObvyazka = _dialogService.ShowObvyazkaDialog();

            if (Guard.ExitIfNull("Не был выбран тип обвязки", _notificationService, SelectedObvyazka))
                return;

            var stand = CurrentProjectModel.SelectedStand;

            var tmp = stand.SelectedObvyazkaInStand;

            tmp.ImageName = SelectedObvyazka.ImageName;

            stand.MaterialLineCount = SelectedObvyazka.LineLength;
            stand.ArmatureCount = SelectedObvyazka.ZraCount;
            stand.TreeSocketMaterialCount = SelectedObvyazka.TreeSocket;
            stand.KMCHCount = SelectedObvyazka.KMCHCount;

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

    public async void OnCreateTechnologicalCardsCommandExecute(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await CreateReportAsync(ReportType.TechnologicalCards, "технологические карты");
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

                if (Guard.ExitIfNull("Стенд или доп. комплектующее не выбраны!",
                    _notificationService,
                    CurrentProjectModel.SelectedStand,
                    selectedComponent))
                    return;

                selectedPurpose.FormedAdditionalEquipId = selectedComponent.Id;

                _notificationService.ShowError("Нет дополнительного компонента для назначения.");
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
            await _standService.FillStandFieldsFromObvyazka(CurrentProjectModel.SelectedStand,
                                        CurrentProjectModel.SelectedStand.SelectedObvyazkaInStand);
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
        var (fromNumber, toNumber) = _dialogService.ShowRenumerateDialog();

        var incorrectRange = fromNumber < 1 || toNumber < 1;

        if (incorrectRange)
            return;

        if (CurrentProjectModel.Stands == null)
        {
            _notificationService.ShowError("Список стендов пуст");
            return;
        }

        var renumeratedStand = CurrentProjectModel.Stands
            .Where(stand => stand.Number >= fromNumber && stand.Number <= toNumber)
            .OrderBy(stand => stand.Number)
            .ToList();

        var standNumber = renumeratedStand.FirstOrDefault()?.Number;

        if (!standNumber.HasValue)
        {
            _notificationService.ShowError("Не найдены подходящие стенды");
            return;
        }

        var standEntities = new List<Stand>();

        foreach (var stand in renumeratedStand)
        {
            stand.SerialNumber = $"SN-1488.{standNumber}.ЖОПА";

            var newStandEntity = StandDataConverter.ConvertToStandEntity(stand);
            standEntities.Add(newStandEntity);
            standNumber++;
        }

        await _projectRepository.UpdateStandsGroupAsync(standEntities);

        ;
        _notificationService.ShowInfo("Стенды пронумерованы");

    }

    public async void OnUpdateObvInStandCommandExecuted(object obj)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {

            var selectedStand = CurrentProjectModel?.SelectedStand;

            if (Guard.ExitIfNull("Не был выбран стенд", _notificationService, selectedStand))
                return;

            //проверка введенного NN обвязки
            if (!ValidateCorrectObvNN(selectedStand.NN))
                return;

            if (!ValidateNotExistingObvNN(selectedStand.NN, true))
                return;

            await _projectService.UpdateObvInStandAsync(CurrentProjectModel, SelectedObvyazka);

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
        // Совместимый синхронный вызов, чтобы не дедлокалось в процессе загрузки
        _ = ResetProjectAsync();
    }

    public async Task ResetProjectAsync()
    {
        CurrentProjectModel = new ProjectModel();
        CurrentStandModel = new StandModel();

        NewStand = new StandModel { Number = 1 };

        CurrentProjectModel.Number = await _projectService.GetProjectsCountAsync();

        InitializeTime();
        OnPropertyChanged(nameof(CurrentProjectModel));
        OnPropertyChanged(nameof(CurrentStandModel));
    }

    #region Инициализация

    public void InitializeTime()
    {
        CurrentProjectModel.CreationDate = DateTime.Now.Date;
        CurrentProjectModel.StartDate = DateTime.Now.Date;
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

    #endregion Методы загрузки данных на view

    #region Методы для CRUD с проектами и стендами

    private async Task AddObvToStandAsync()
    {
        var selectedStand = CurrentProjectModel.SelectedStand;


        if (Guard.ExitIfNull("Не выбран стенд!", _notificationService, selectedStand))
            return;

        if (Guard.ExitIfNull("Не выбран тип обвязки!", _notificationService, SelectedObvyazka))
            return;

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



        //
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

        if (!ValidateCorrectStandNN(NewStand.Number))
            return;

        if (!ValidateNotExistingStandNN(NewStand.Number, false))
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

        await newStandModel.InitializeDefaultPurposes();

        CurrentProjectModel.Stands.Add(newStandModel);

        UpdateNewStandNN();

        OnPropertyChanged(nameof(CurrentStandModel));
        OnPropertyChanged(nameof(NewStand));

        OnStandsInProjectChanged();


        //выбираем добавленный стенд
        var addedStand = CurrentProjectModel.Stands.FirstOrDefault(stand => stand.Id == newStandModel.Id);

        if (addedStand != null)
        {
            CurrentProjectModel.SelectedStand = addedStand;
        }

        _notificationService.ShowInfo($"Стенд с ID {addedStandEntity.Id} успешно добавлен!");
    }
    ///
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

        if (!ValidateCorrectStandNN(NewStand.Number))
            return;

        if (!ValidateNotExistingStandNN(NewStand.Number, true))
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

        _notificationService.ShowInfo("Стенд удалён");
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
                setExportDays(equipment.ExportDays);
            }

            if (equipment is BaseEquip baseEquip)
            {
                setWeight(baseEquip.Weight);
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
                    dp.ExportDays = selected.ExportDays;
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
                    CollectionRefreshHelper.SafeRefreshCollection(CurrentStandModel.NewElectricalComponent.Purposes);
                    CollectionRefreshHelper.SafeRefreshCollection(CurrentProjectModel.SelectedStand
                        .AllElectricalPurposesInStand);
                    CollectionRefreshHelper.SafeRefreshCollection(CurrentProjectModel.SelectedStand
                        .ElectricalComponentsInStand);
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
        await _reportService.GenerateReportAsync(typeGenerator, CurrentProjectModel.CurrentProjectId);

        if (_notificationService.ShowConfirmation($"Ведомость {reportName} создана!\nОткрыть папку с отчётами?"))
        {
            var reportDir = SettingsManager.GetReportDirectory();
            Process.Start("explorer.exe", reportDir);
        }
    }

    #endregion

    #region Обновление UI

    public async Task UpdateUI()
    {
        await LoadStandsDataAsync();
        await LoadObvyazkiAsync();
        await LoadPurposesInStandsAsync();
        await LoadAllAvaileDataAsync();
        await LoadProjectInfoAsync(CurrentProjectModel.CurrentProjectId);
    }

    public void OnObvyazkiInStandChanged()
    {
        Debug.WriteLine("Обвязки поменялись");

        UpdateNewObvNN();
        UpdateTablesQuantity();
        UpdateClampsQuantity();
        UpdateBracketsQuantity();
        UpdateElectricEquipment();

        CollectionRefreshHelper.SafeRefreshCollection(CurrentStandModel.NewElectricalComponent.Purposes);
        CollectionRefreshHelper.SafeRefreshCollection(CurrentStandModel.NewAdditionalEquip.Purposes);

        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        CollectionRefreshHelper.SafeSortAndRefreshCollection(
            collection: selectedStand.ObvyazkiInStand,
            fieldToSortBy: "NN",
            descending: false);
    }

    public void OnFramesInStandChanged()
    {
        Debug.WriteLine("Рамы поменялись");

        UpdateChannelsQuantity();
        UpdateDrainage();

        CollectionRefreshHelper.SafeRefreshCollection(CurrentStandModel.NewAdditionalEquip.Purposes);
        CollectionRefreshHelper.SafeRefreshCollection(CurrentStandModel.NewDrainage.Purposes);
    }

    public void OnSelectedStandChanged()
    {
        Debug.WriteLine("Выбранный стенд изменился");

        OnFramesInStandChanged();
        OnObvyazkiInStandChanged();
        UpdateBracketsQuantity();
    }

    public void OnStandsInProjectChanged()
    {
        Debug.WriteLine("Стенды изменились");

        //отсортировываем по возрастанию номера
        CollectionRefreshHelper.SafeSortAndRefreshCollection(
            collection: CurrentProjectModel.Stands,
            fieldToSortBy: "Number",
            descending: false);
    }




    //возвращает максимальный NN обвязок в стенде
    public int MaxObvNN
    {
        get => CurrentProjectModel?.SelectedStand?.ObvyazkiInStand.Max(obv => obv.NN) ?? 0;
    }

    //возвращает максимальный NN стендов в проекте
    public int MaxStandNN
    {
        get => CurrentProjectModel.Stands.Count() > 0 ? CurrentProjectModel.Stands.Max(stand => stand.Number) : 0;

    }

    //обновляем поле NN в обвязке
    public void UpdateNewObvNN()
    {
        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        selectedStand.NN = MaxObvNN + 1;


        Debug.WriteLine("Новый NN обвязки изменен");
    }

    //обновляем № п/п стенда
    public void UpdateNewStandNN()
    {
        if (NewStand == null) return;

        NewStand.Number = MaxStandNN + 1;

        Debug.WriteLine("Новый NN стенда изменен");
    }

    //обновляем кол-во швеллера
    public void UpdateChannelsQuantity()
    {
        Debug.WriteLine("Пересчет швеллера начат");

        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return;

        var standBraceType = selectedStand.BraceType;

        if (string.IsNullOrEmpty(standBraceType) || standBraceType != "Швеллер")
            return;

        var additionalEquips = CurrentStandModel.NewAdditionalEquip.Purposes;
        var channelRecord = additionalEquips.FirstOrDefault(equip => equip.Purpose == "Швеллер");

        if (channelRecord == null)
            return;

        //швеллер в метрах
        var framesWidthSum = selectedStand.FramesInStand.Sum(frame => frame.Width);
        channelRecord.Quantity = framesWidthSum / 1000.0f;


        Debug.WriteLine("Пересчет швеллера завершен");
    }

    //обновляем кол-во хомутов
    public void UpdateClampsQuantity()
    {
        Debug.WriteLine("Пересчет хомутов начат");

        var standsSettings = CalculationSettingsManager.Load<StandSettings, StandSettingsData>();

        var additionalEquips = CurrentStandModel.NewAdditionalEquip.Purposes;
        var clampsRecord = additionalEquips.FirstOrDefault(equip => equip.Purpose == "Хомуты");

        if (clampsRecord == null)
            return;

        clampsRecord.Quantity = CurrentStandModel.ObvyazkiInStand.Sum(obv => obv.Clamp) ?? 0.0f;

        Debug.WriteLine("Пересчет хомутов завершен");
    }

    //обновляем кол-во табличек
    public void UpdateTablesQuantity()
    {
        Debug.WriteLine("Пересчет табличек начат");

        var sensorsQuantity = CurrentStandModel.CountSensorsQuantity();

        var additionalComponents = CurrentStandModel.NewAdditionalEquip.Purposes;
        var tableRecord = additionalComponents.FirstOrDefault(purpose => purpose.Purpose == "Табличка");

        if (tableRecord == null)
            return;

        tableRecord.Quantity = sensorsQuantity;


        Debug.WriteLine("Пересчет табличек завершен");
    }

    //обновляем кол-во кронштейнов
    public void UpdateBracketsQuantity()
    {
        Debug.WriteLine("Пересчет кронштейнов начат");

        var standsSettings = CalculationSettingsManager.Load<StandSettings, StandSettingsData>();

        const int bracketsPerDifSensor = 1;

        var difSensorsQuantity = CurrentStandModel.CountDifSensorsQuantity();

        var additionalComponents = CurrentStandModel.NewAdditionalEquip.Purposes;
        var difSensorsBracketRecord = additionalComponents.FirstOrDefault(purpose => purpose.Purpose == "Кронштейн перепадчика");

        if (difSensorsBracketRecord != null)
        {
            difSensorsBracketRecord.Quantity = bracketsPerDifSensor * difSensorsQuantity;
        }



        const int bracketsPerAbsoluteSensor = 2;

        var standBraceType = CurrentProjectModel?.SelectedStand?.BraceType;

        if (!string.IsNullOrEmpty(standBraceType) && standBraceType == "На кронштейне")
        {
            var absSensorsQuantity = CurrentStandModel.CountAbsoluteSensorsQuantity();

            var absSensorsBracketsRecord = additionalComponents.FirstOrDefault(purpose => purpose.Purpose == "Кронштейн абсолютника");

            if (absSensorsBracketsRecord != null)
            {
                absSensorsBracketsRecord.Quantity = bracketsPerAbsoluteSensor * absSensorsQuantity;
            }
        }


        const int universalBracketQuantity = 2;

        if (!string.IsNullOrEmpty(standBraceType) && standBraceType == "Швеллер")
        {
            var universalBracketRecord = additionalComponents.FirstOrDefault(purpose => purpose.Purpose == "Кронштейн универсальный");

            if (universalBracketRecord != null)
            {
                universalBracketRecord.Quantity = universalBracketQuantity;
            }
        }

        Debug.WriteLine("Пересчет кронштейнов завершен");
    }

    //обновляем данные по дренажу
    public void UpdateDrainage()
    {
        Debug.WriteLine("Пересчет дренажной трубы начат");

        var selectedStand = CurrentProjectModel?.SelectedStand;

        var drainageParts = CurrentStandModel.NewDrainage.Purposes;
        var mainPipeRecord = drainageParts.FirstOrDefault(part => part.Purpose == "Основная труба");

        if (mainPipeRecord == null || selectedStand == null)
            return;

        mainPipeRecord.Quantity = selectedStand.FramesInStand.Sum(frame => frame.Width) / 1000.0f;


        Debug.WriteLine("Пересчет дренажной трубы завершен");
    }

    //обновляем данные по электрике
    public void UpdateElectricEquipment()
    {
        Debug.WriteLine("Пересчет электрики начат");

        var electricComponents = CurrentStandModel.NewElectricalComponent.Purposes;

        //кабельные ввода
        var cableInputsPerSensor = 2;
        var cableInputsRecord = electricComponents.FirstOrDefault(purpose => purpose.Purpose == "Кабельные вводы");

        var cableInputsQuantity = 0;

        var sensorsQuantity = CurrentProjectModel.SelectedStand?.CountSensorsQuantity();

        if (cableInputsRecord != null && sensorsQuantity.HasValue)
        {
            cableInputsQuantity = sensorsQuantity.Value * cableInputsPerSensor;
            cableInputsRecord.Quantity = cableInputsQuantity;
        }


        //сигнальный кабель
        var standsSettings = CalculationSettingsManager.Load<StandSettings, StandSettingsData>();

        var signalCableRecord = electricComponents.FirstOrDefault(purpose => purpose.Purpose == "Сигнальный кабель");

        var signalCablePerSensor = 0;
        int? signalCabelQuantity = 0;


        if (sensorsQuantity.HasValue && signalCableRecord != null)
        {
            signalCablePerSensor = sensorsQuantity.Value switch
            {
                >= 0 and <= 2 => 2,
                >= 3 and <= 5 => 3,
                >= 6 => 4,
                _ => 0
            };

            signalCabelQuantity = sensorsQuantity * signalCablePerSensor;

            signalCableRecord.Quantity = signalCabelQuantity;

        }

        //кабель 4 мм
        var fourMmCableRecord = electricComponents.FirstOrDefault(purpose => purpose.Purpose == "Кабель 4мм");

        if (fourMmCableRecord != null)
        {
            fourMmCableRecord.Quantity = cableInputsQuantity;
        }


        //металлорукав
        var metalHoseRecord = electricComponents.FirstOrDefault(purpose => purpose.Purpose == "Металлорукав");

        if (metalHoseRecord != null)
        {
            metalHoseRecord.Quantity = signalCabelQuantity;
        }

        Debug.WriteLine("Пересчет электрики завершен");
    }

    #endregion

    #region Валидация

    //валидация номера обвязки
    public bool ValidateCorrectObvNN(int newObvNN)
    {
        var invalidNN = newObvNN < 1;

        if (invalidNN)
        {
            _notificationService.ShowError("Указанный № обвязки некорректен!");
            return false;
        }

        return true;
    }

    public bool ValidateNotExistingObvNN(int newObvNN, bool excludeSelected)
    {

        var selectedStand = CurrentProjectModel.SelectedStand;

        if (selectedStand == null)
            return false;


        var obvCollection = selectedStand.ObvyazkiInStand;
        var selectedObv = selectedStand.SelectedObvyazkaInStand;


        if (obvCollection == null)
            return true;

        var isAlreadyExist = true;

        if (!excludeSelected)
        {
            isAlreadyExist = obvCollection
                .Any(obv => obv.NN == newObvNN);
        }
        else if (selectedObv != null)
        {
            isAlreadyExist = obvCollection
                .Where(obv => obv.NN != selectedObv.NN)
                .Any(obv => obv.NN == newObvNN);
        }

        if (isAlreadyExist)
        {
            _notificationService.ShowError("Указанный № обвязки уже существует!");
            return false;
        }

        return true;

    }

    public bool ValidateCorrectStandNN(int newStandNumber)
    {

        var invalidNN = newStandNumber < 1;

        if (invalidNN)
        {
            _notificationService.ShowError("Указанный № стенда некорректен!");
            return false;
        }

        return true;

    }

    public bool ValidateNotExistingStandNN(int newStandNumber, bool excludeSelected)
    {
        var standsCollection = CurrentProjectModel.Stands;
        var selectedStand = CurrentProjectModel.SelectedStand;

        if (standsCollection == null)
            return true;

        var isAlreadyExist = true;

        if (!excludeSelected)
        {
            isAlreadyExist = standsCollection
                .Any(stand => stand.Number == newStandNumber);
        }
        else if (selectedStand != null)
        {
            isAlreadyExist = standsCollection
                .Where(stand => stand.Number != selectedStand.Number)
                .Any(stand => stand.Number == newStandNumber);
        }

        if (isAlreadyExist)
        {
            _notificationService.ShowError("Указанный № стенда уже существует!");
            return false;
        }

        return true;
    }

    #endregion
}
