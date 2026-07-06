using ReportEngine.App.AsyncCommands;
using ReportEngine.App.Model;
using ReportEngine.App.Services.Calculation;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Generators;
using ReportEngine.Export.ExcelWork.Services.Interfaces;

namespace ReportEngine.App.ViewModels.TreeView;

public class TreeViewModel
{
    private readonly ICalculationService _calculationService;
    private readonly IDialogService _dialogService;
    private readonly INotificationService _notificationService;
    private readonly ProjectModel _project;
    private readonly IReportService _reportService;
    private readonly UpdaterStandService _updaterStandService;
    private readonly FlatSummaryReportGenerator _flatSummaryReportGenerator;

    public TreeViewModel(
        ProjectViewModel projectViewModel,
        INotificationService notificationService,
        IReportService reportService,
        IDialogService dialogService,
        ICalculationService calculationService,
        UpdaterStandService updaterStandService,
        FlatSummaryReportGenerator flatSummaryReportGenerator)
    {
        _notificationService = notificationService;
        _reportService = reportService;
        _dialogService = dialogService;
        _calculationService = calculationService;
        _updaterStandService = updaterStandService;
        _flatSummaryReportGenerator = flatSummaryReportGenerator;
        _project = projectViewModel.CurrentProjectModel;

        InitializeCommands();
    }

    public IAsyncCommand CreateSummaryReportAsync { get; private set; }
    public IAsyncCommand CreateComponentsListReportAsync { get; private set; }
    public IAsyncCommand CreateNamePlatesReportAsync { get; private set; }
    public IAsyncCommand CreateMarksReportReportAsync { get; private set; }
    public IAsyncCommand CreateProductionListReportAsync { get; private set; }
    public IAsyncCommand CreateFinPlanReportAsync { get; private set; }
    public IAsyncCommand CreateContainersReportReportAsync { get; private set; }
    public IAsyncCommand CreatePassportReportAsync { get; private set; }
    public IAsyncCommand CreateTechCardsReportAsync { get; private set; }
    public IAsyncCommand CalculateProjectCommandAsync { get; private set; }
    public IAsyncCommand RecalculateProjectCommandAsync { get; private set; }
    public IAsyncCommand CreateFlatSummaryReportCommandAsync { get; private set; }

    private void InitializeCommands()
    {
        CreateSummaryReportAsync = CreateReportCommand(
            ReportType.SummaryReport,
            $"""
             Сводная ведомость создана по проекту:
             Заказ покупателя: {_project.OrderCustomer}
             Обозначение КД: {_project.Description}
             """);
        CreateComponentsListReportAsync = CreateReportCommand(
            ReportType.ComponentsListReport,
            $"""
             Ведомость комплектующих создана про проекту:
             Заказ покупателя: {_project.OrderCustomer}
             Обозначение КД: {_project.Description}
             """);
        CreateNamePlatesReportAsync = CreateReportCommand(
            ReportType.NameplatesReport,
            $"""
             Ведомость шильдиков и табличек создана по проекту:
             Заказ покупателя: {_project.OrderCustomer}
             Обозначение КД: {_project.Description}
             """);
        CreateMarksReportReportAsync = CreateReportCommand(
            ReportType.MarksReport,
            $"""
             Ведомость маркировки создана по проекту:
             Заказ покупателя: {_project.OrderCustomer}
             Обозначение КД: {_project.Description}
             """);
        CreateProductionListReportAsync = CreateReportCommand(
            ReportType.ProductionReport,
            $"""
             Ведомость производства создана по проекту:
             Заказ покупателя: {_project.OrderCustomer}
             Обозначение КД: {_project.Description}
             """);
        CreateFinPlanReportAsync = CreateReportCommand(
            ReportType.FinPlanReport,
            $"""
             Фин. план создан по проекту:
             Заказ покупателя: {_project.OrderCustomer}
             Обозначение КД: {_project.Description}
             """);
        CreateContainersReportReportAsync = CreateReportCommand(
            ReportType.ContainerReport,
            $"""
             Ведомость тары создана по проекту:
             Заказ покупателя: {_project.OrderCustomer}
             Обозначение КД: {_project.Description}
             """);
        CreatePassportReportAsync = CreateReportCommand(
            ReportType.PassportsReport,
            $"""
             Ведомость паспортов создана по проекту:
             Заказ покупателя: {_project.OrderCustomer}
             Обозначение КД: {_project.Description}
             """);
        CreateTechCardsReportAsync = CreateReportCommand(
            ReportType.TechnologicalCards,
            $"""
             Ведомость технологических карт создана по проекту:
             Заказ покупателя: {_project.OrderCustomer}
             Обозначение КД: {_project.Description}
             """);

        CalculateProjectCommandAsync =
            new AsyncRelayCommand(CalculateProjectAsync);
        RecalculateProjectCommandAsync =
            new AsyncRelayCommand(RecalculateProjectAsync);
        CreateFlatSummaryReportCommandAsync =
            new AsyncRelayCommand(CreateFlatSummaryReportAsync);
    }

    private IAsyncCommand CreateReportCommand(ReportType type, string successMessage)
    {
        return new AsyncRelayCommand(async _ =>
        {
            await _dialogService.RunWithProgressDialogAsync(async () =>
            {
                await _reportService.GenerateReportAsync(
                    type,
                    _project.CurrentProjectId);
            });

            _notificationService.ShowInfo(successMessage);
        });
    }

    private async Task CreateFlatSummaryReportAsync(object obj)
    {
        await _dialogService.RunWithProgressDialogAsync(async () =>
        {
            await _flatSummaryReportGenerator.GenerateAsync(_project.CurrentProjectId);

            _notificationService.ShowInfo($"""
                                           Сводная ведомость для 1С создана по проекту:
                                           Заказ покупателя: {_project.OrderCustomer}
                                           Обозначение КД: {_project.Description}
                                           """);
        });
    }

    private async Task CalculateProjectAsync(object obj)
    {
        await _calculationService.CalculateProjectAsync(_project);

        _notificationService.ShowInfo($"""
                                       Расчёт проекта завершён! ✅
                                       Заказ покупателя: {_project.OrderCustomer}
                                       Обозначение КД: {_project.Description}
                                       """);
    }

    private async Task RecalculateProjectAsync(object obj)
    {
        await _updaterStandService.ApplyChangesAndSaveAsync(_project);

        await _calculationService.CalculateProjectAsync(_project);

        _notificationService.ShowInfo($"""
                                       Проект обновлён и пересчитан! ✅
                                       Заказ покупателя: {_project.OrderCustomer}
                                       Обозначение КД: {_project.Description}
                                       """);
    }
}
