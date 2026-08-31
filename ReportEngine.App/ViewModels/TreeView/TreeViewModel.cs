using System.Diagnostics;
using System.Text;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.AsyncCommands;
using ReportEngine.App.Enums;
using ReportEngine.App.Model;
using ReportEngine.App.Services.Calculation;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.Views.Windows.Dialog;
using ReportEngine.Domain.Entities;
using ReportEngine.Export.DTO;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Generators;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;

namespace ReportEngine.App.ViewModels.TreeView;

public class TreeViewModel
{
    private readonly ICalculationService _calculationService;
    private readonly IDialogService _dialogService;
    private readonly INotificationService _notificationService;
    private readonly ProjectViewModel _projectViewModel;
    private readonly IReportService _reportService;

    private readonly IServiceProvider _serviceProvider;
    private readonly UpdaterStandService _updaterStandService;

    public TreeViewModel(
        ProjectViewModel projectViewModel,
        INotificationService notificationService,
        IReportService reportService,
        IDialogService dialogService,
        ICalculationService calculationService,
        UpdaterStandService updaterStandService,
        IServiceProvider serviceProvider)
    {
        _notificationService = notificationService;
        _reportService = reportService;
        _dialogService = dialogService;
        _calculationService = calculationService;
        _updaterStandService = updaterStandService;
        _projectViewModel = projectViewModel;
        _serviceProvider = serviceProvider;

        InitializeCommands();
    }

    private ProjectModel _project => _projectViewModel.CurrentProjectModel;

    public IAsyncCommand CreateSummaryReportAsync { get; private set; }
    public IAsyncCommand CreateComponentsListReportAsync { get; private set; }
    public IAsyncCommand CreateNamePlatesReportAsync { get; private set; }
    public IAsyncCommand CreateMarksReportAsync { get; private set; }
    public IAsyncCommand CreateProductionListReportAsync { get; private set; }
    public IAsyncCommand CreateFinPlanReportAsync { get; private set; }
    public IAsyncCommand CreateContainersReportAsync { get; private set; }
    public IAsyncCommand CreatePassportReportAsync { get; private set; }
    public IAsyncCommand CreateTechCardsReportAsync { get; private set; }
    public IAsyncCommand CreateFlatSummaryReportCommandAsync { get; private set; }
    public IAsyncCommand CalculateProjectCommandAsync { get; private set; }
    public IAsyncCommand RecalculateProjectCommandAsync { get; private set; }



    private void InitializeCommands()
    {
        CreateSummaryReportAsync = new AsyncRelayCommand(OnCreateSummaryReportAsync);
        CreateComponentsListReportAsync = new AsyncRelayCommand(OnCreateComponentsListReportAsync);
        CreateNamePlatesReportAsync = new AsyncRelayCommand(OnCreateNamePlatesReportAsync);
        CreateMarksReportAsync = new AsyncRelayCommand(OnCreateMarksReportAsync);
        CreateProductionListReportAsync = new AsyncRelayCommand(OnCreateProductionReportAsync);
        CreateFinPlanReportAsync = new AsyncRelayCommand(OnCreateFinPlanReportAsync);
        CreateContainersReportAsync = new AsyncRelayCommand(OnCreateContainerReportAsync);
        CreatePassportReportAsync = new AsyncRelayCommand(OnCreatePassportReportAsync);
        CreateTechCardsReportAsync = new AsyncRelayCommand(OnCreateTechCardReportAsync);
        CreateFlatSummaryReportCommandAsync = new AsyncRelayCommand(OnCreateFlatSummaryReportAsync);

        CalculateProjectCommandAsync = new AsyncRelayCommand(OnCalculateProjectAsync);
        RecalculateProjectCommandAsync = new AsyncRelayCommand(OnRecalculateProjectAsync);

    }





    private async Task OnCreateSummaryReportAsync(object arg)
    {
        var continueWithDuplicates = CheckDuplicates();

        if (!continueWithDuplicates)
        {
            _notificationService.ShowInfo("Генерация отчета отменена");
            return;
        }

        await _dialogService.RunWithProgressDialogAsync(async () =>
        {
            await _reportService.GenerateReportAsync(
                ReportType.SummaryReport,
                _project.CurrentProjectId);
        });

        ShowReportSuccesfullWindow("Сводная ведомость");
    }


    private async Task OnCreateComponentsListReportAsync(object arg)
    {
        var continueWithDuplicates = CheckDuplicates();

        if (!continueWithDuplicates)
        {
            _notificationService.ShowInfo("Генерация отчета отменена");
            return;
        }

        await _dialogService.RunWithProgressDialogAsync(async () =>
        {
            await _reportService.GenerateReportAsync(
                ReportType.ComponentsListReport,
                _project.CurrentProjectId);
        });

        ShowReportSuccesfullWindow("Ведомость комплектующих");
    }


    private async Task OnCreateNamePlatesReportAsync(object arg)
    {
        var continueWithDuplicates = CheckDuplicates();

        if (!continueWithDuplicates)
        {
            _notificationService.ShowInfo("Генерация отчета отменена");
            return;
        }

        await _dialogService.RunWithProgressDialogAsync(async () =>
        {
            await _reportService.GenerateReportAsync(
                ReportType.NameplatesReport,
                _project.CurrentProjectId);
        });

        ShowReportSuccesfullWindow("Ведомость шильдиков и табличек");
    }

    private async Task OnCreateMarksReportAsync(object arg)
    {
        var continueWithDuplicates = CheckDuplicates();

        if (!continueWithDuplicates)
        {
            _notificationService.ShowInfo("Генерация отчета отменена");
            return;
        }

        await _dialogService.RunWithProgressDialogAsync(async () =>
        {
            await _reportService.GenerateReportAsync(
                ReportType.MarksReport,
                _project.CurrentProjectId);
        });

        ShowReportSuccesfullWindow("Ведомость маркировки");
    }

    private async Task OnCreateProductionReportAsync(object arg)
    {
        var continueWithDuplicates = CheckDuplicates();

        if (!continueWithDuplicates)
        {
            _notificationService.ShowInfo("Генерация отчета отменена");
            return;
        }

        await _dialogService.RunWithProgressDialogAsync(async () =>
        {
            await _reportService.GenerateReportAsync(
                ReportType.ProductionReport,
                _project.CurrentProjectId);
        });

        ShowReportSuccesfullWindow("Ведомость производства");
    }

    private async Task OnCreateFinPlanReportAsync(object arg)
    {
        var continueWithDuplicates = CheckDuplicates();

        if (!continueWithDuplicates)
        {
            _notificationService.ShowInfo("Генерация отчета отменена");
            return;
        }

        await _dialogService.RunWithProgressDialogAsync(async () =>
        {
            await _reportService.GenerateReportAsync(
                ReportType.FinPlanReport,
                _project.CurrentProjectId);
        });

        ShowReportSuccesfullWindow("Финансовый план");
    }

    private async Task OnCreateContainerReportAsync(object arg)
    {
        var continueWithDuplicates = CheckDuplicates();

        if (!continueWithDuplicates)
        {
            _notificationService.ShowInfo("Генерация отчета отменена");
            return;
        }

        await _dialogService.RunWithProgressDialogAsync(async () =>
        {
            await _reportService.GenerateReportAsync(
                ReportType.ContainerReport,
                _project.CurrentProjectId);
        });

        ShowReportSuccesfullWindow("Тара");
    }


    private async Task OnCreatePassportReportAsync(object arg)
    {
        var continueWithDuplicates = CheckDuplicates();

        if (!continueWithDuplicates)
        {
            _notificationService.ShowInfo("Генерация отчета отменена");
            return;
        }

        await _dialogService.RunWithProgressDialogAsync(async () =>
        {
            await _reportService.GenerateReportAsync(
                ReportType.PassportsReport,
                _project.CurrentProjectId);
        });

        ShowReportSuccesfullWindow("Паспорт");
    }

    private async Task OnCreateTechCardReportAsync(object arg)
    {
        var continueWithDuplicates = CheckDuplicates();

        if (!continueWithDuplicates)
        {
            _notificationService.ShowInfo("Генерация отчета отменена");
            return;
        }


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



        await _dialogService.RunWithProgressDialogAsync(async () =>
        {
            await _reportService.GenerateReportAsync(
                ReportType.TechnologicalCards,
                _project.CurrentProjectId);
        });

        ShowReportSuccesfullWindow("Технологические карты");
    }


    private async Task OnCreateFlatSummaryReportAsync(object arg)
    {
        var continueWithDuplicates = CheckDuplicates();

        if (!continueWithDuplicates)
        {
            _notificationService.ShowInfo("Генерация отчета отменена");
            return;
        }

        await _dialogService.RunWithProgressDialogAsync(async () =>
        {
            await _reportService.GenerateReportAsync(
                ReportType.FlatSummaryReport,
                _project.CurrentProjectId);
        });

        ShowReportSuccesfullWindow("Сводная ведомость (1С)");
    }



    

    private async Task OnCalculateProjectAsync(object obj)
    {
        await _dialogService.RunWithProgressDialogAsync(async () =>
        {
            await _calculationService.CalculateProjectAsync(_project);
        });

        _notificationService.ShowInfo($"""
                                       Расчёт проекта завершён!
                                       Заказ покупателя: {_project.OrderCustomer}
                                       Обозначение КД: {_project.Description}
                                       """);
    }

    private async Task OnRecalculateProjectAsync(object obj)
    {
        await _dialogService.RunWithProgressDialogAsync(async () =>
        {
            await _updaterStandService.ApplyChangesAndSaveAsync(_project);

            await _calculationService.CalculateProjectAsync(_project);
        });

        _notificationService.ShowInfo($"""
                                       Проект обновлён и пересчитан!
                                       Заказ покупателя: {_project.OrderCustomer}
                                       Обозначение КД: {_project.Description}
                                       """);

    }






    private bool CheckDuplicates()
    {
        var kksDuplicates = _project.Stands
                .GroupBy(stand => stand.KKSCode)
                .Where(group => group.Count() > 1)
                .ToList();

        if (kksDuplicates.Count > 0)
        {
            var warningMessage = "Обнаружены дублирования KKS-кодов стендов:\n\n" +
                                     string.Join("\n", kksDuplicates.Select(g => $"- {g.Key} ({g.Count()} шт.)")) +
                                     "\n\nПродолжить генерацию отчета?";

            return _notificationService.ShowConfirmation(warningMessage);
        }
        else
        {
            return true;
        }
    }



    private bool ShowReportSuccesfullWindow(string reportName)
    {
        var succesfulMessage = $"""
                   Отчёт "{reportName}" создан по проекту:
                   Заказ покупателя: {_project.OrderCustomer}
                   Обозначение КД: {_project.Description}
                   
                   Открыть папку с отчетами?
                   """;

       var confirmationResult = _notificationService.ShowConfirmation(succesfulMessage);

       if (confirmationResult)
       {
            var reportDir = JsonHandler.GetSaveReportDirectory(DirectoryHelper.GetConfigPath());
            Process.Start("explorer.exe", reportDir);
       }
       return confirmationResult;
    }




}
