using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.Commands;
using ReportEngine.App.Commands.Initializers;
using ReportEngine.App.Commands.Providers;
using ReportEngine.App.Model;
using ReportEngine.App.Services.Cloners;
using ReportEngine.App.Services.Core;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.Services.Logger;
using ReportEngine.App.Services.Navigation;
using ReportEngine.App.Services.Notification;
using ReportEngine.App.Views.Controls;
using ReportEngine.App.Views.Settings.CalculationParameters;
using ReportEngine.App.Views.Windows;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;

namespace ReportEngine.App.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    private readonly AuditService _auditService;
    private readonly ICalculationService _calculationService;
    private readonly IDialogService _dialogService;
    private readonly EntityProjectClonerService _entityProjectClonerService;
    private readonly ExceptionService _exceptionService;
    private readonly UiLogger _logger;
    private readonly NavigationService _navigation;
    private readonly INotificationService _notificationService;
    private readonly IProjectInfoRepository _projectRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly SessionService _sessionService;


    #region Конструктор

    public MainWindowViewModel(
        IServiceProvider serviceProvider,
        NavigationService navigation,
        IProjectInfoRepository projectRepository,
        ICalculationService calculationService,
        INotificationService notificationService,
        IDialogService dialogService,
        EntityProjectClonerService entityProjectClonerService,
        SessionService sessionService,
        AuditService auditService,
        UiLogger logger,
        ExceptionService exceptionService)
    {
        _notificationService = notificationService;
        _calculationService = calculationService;
        _serviceProvider = serviceProvider;
        _projectRepository = projectRepository;
        _navigation = navigation;
        _dialogService = dialogService;
        _entityProjectClonerService = entityProjectClonerService;
        _sessionService = sessionService;
        _auditService = auditService;
        _logger = logger;
        _exceptionService = exceptionService;

        _sessionService.PropertyChanged += SessionChanged;

        _ = CheckDbConnectionAsync();
        
        InitializeMainWindowCommands();
        InitializeGenericEquipCommands();
    }

    #endregion Конструктор

    public MainWindowModel MainWindowModel { get; set; } = new();
    public GenericEquipCommandProvider GenericEquipCommandProvider { get; set; } = new();
    public MainWindowCommandProvider MainWindowCommandProvider { get; set; } = new();

    public User? CurrentUser => _sessionService.CurrentUser;
    public string? CurrentUserLogin => _sessionService.CurrentUser?.UserLogin;
    public string DatabaseMode => JsonHandler.GetDatabaseMode(DirectoryHelper.GetConfigPath());

    private void SessionChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SessionService.CurrentUser))
        {
            OnPropertyChanged(nameof(CurrentUser));
            OnPropertyChanged(nameof(CurrentUserLogin));
        }
    }

    #region Дженерик команды

    public void OnOpenGenericWindowCommandExecuted<T, TEquip>(object e)
        where T : class, IBaseEquip, new()
    {
        _exceptionService.SafeExecute(() => _navigation.ShowGenericWindow<T, T>());
    }

    #endregion Дженерик команды

    #region Методы

    public void InitializeMainWindowCommands()
    {
        MainWindowCommandsInitializer.InitializeCommands(this);
    }

    public void InitializeGenericEquipCommands()
    {
        MainWindowCommandsInitializer.InitializeGenericCommands(this);
    }

    #endregion Методы

    #region Комманды главного окна

    public bool CanAllCommandsExecute(object e)
    {
        return true;
    }

    public void OnSetDbOffline(object e)
    {
        JsonHandler.SetDatabaseMode(DirectoryHelper.GetConfigPath(), "Offline");
        RestartApp();
    }

    public void OnSetDbOnline(object e)
    {
        JsonHandler.SetDatabaseMode(DirectoryHelper.GetConfigPath(), "Online");
        RestartApp();
    }

    private void RestartApp()
    {
        try
        {
            var confirm = _notificationService.ShowConfirmation("""
                                                                Приложение будет перезагружено.
                                                                Все несохранённые данные будут потеряны.
                                                                Продолжить?
                                                                """);

            if (!confirm)
                return;

            var exePath = Process.GetCurrentProcess().MainModule?.FileName;

            if (exePath == null)
                return;

            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                UseShellExecute = true
            });

            Application.Current.Shutdown();
        }
        catch (Exception e)
        {
            MessageBox.Show($"Ошибка {e.Message}");
            throw;
        }
    }

    public async Task OnRecalculateProjectCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(RecalculateProjectAsync);

        _notificationService.ShowInfo("Переформирование завершено");
    }

    public async Task OnEditProjectCommandExecuted()
    {
        if (MainWindowModel.SelectedProject == null) 
            return;

        await _exceptionService.SafeExecuteAsync(async () =>
        {
            var projectViewModel = _serviceProvider.GetRequiredService<ProjectViewModel>();

            await _dialogService.RunWithProgressDialogAsync(async () =>
            {
                await projectViewModel.LoadProjectInfoAsync(MainWindowModel.SelectedProject.Id);
                _navigation.ShowContent<TreeProjectView>();
                _logger.Success($"Отрыт проект {MainWindowModel.SelectedProject.OrderCustomer} Статус: Успешно");
            });
        });
    }

    public async Task OnCopyProjectCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            var selectedProject = MainWindowModel.SelectedProject;

            if (selectedProject == null)
            {
                _notificationService.ShowInfo("Проект не выбран");
                return;
            }

            await _dialogService.RunWithProgressDialogAsync(async () =>
            {
                await _entityProjectClonerService.CloneProjectEntity(selectedProject);

                _logger.Success($"Скопирован проект {selectedProject.OrderCustomer} Статус: Успешно");

                await ShowAllProjectsAsync();


            });
        });
    }

    public async Task OnOpenMainWindowCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            var projectViewModel = _serviceProvider.GetRequiredService<ProjectViewModel>();

            await _dialogService.RunWithProgressDialogAsync(async () =>
            {
                //принудительно обновляем количество стендов при закрытии проекта и подгружаем свежие данные
                //при пересчете всего проекта начинает подтормаживать, поэтому оставляем только обновление количества стендов
                if (projectViewModel?.CurrentProjectModel != null &&
                    projectViewModel.CurrentProjectModel.CurrentProjectId != 0)
                {
                    //await RecalculateProjectAsync();
                    await _calculationService.CalculateAndUpdateStandQuantity(projectViewModel.CurrentProjectModel);

                    await UpdateProjectStandsQuantity(projectViewModel.CurrentProjectModel.CurrentProjectId);
                }

                //if (CheckUnsafeDetails(projectViewModel))
                //{
                //    var result = _notificationService.ShowConfirmation("У вас есть несохраненные изменения. \nВы уверены, что хотите вернуться на главный экран?", "Подтверждение");
                //    if (!result)
                //        return;
                //}

                //if(projectViewModel.CurrentProjectModel.Stands.Count == 0 || projectViewModel.CurrentProjectModel.Stands == null)
                //{
                //    _navigation.CloseContent();
                //}

                _navigation.CloseContent();
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.MainContentControl.Content = mainWindow.MainGrid;
                CollectionRefreshHelper.SafeRefreshCollection(MainWindowModel.AllProjects);
            });
        });
    }

    private bool CheckUnsafeDetails(ProjectViewModel projectViewModel)
    {
        var stands = projectViewModel.CurrentProjectModel.Stands;

        if (stands.Count == 0 || stands == null)
            return false;

        if (stands.Any(s => s.ElectricalPurposesChanges || s.DrainagePurposesChanges)) //s.AdditionalPurposesChanges
            return true;

        return false;
    }

    public void OpenOthersWindowCommandExecuted<T>(object e)
        where T : Window
    {
        _exceptionService.SafeExecute(_navigation.ShowWindow<T>);
    }

    public void OpenAuthWindowCommandExecuted<T>(object e)
        where T : Window
    {
        _exceptionService.SafeExecute(_navigation.ShowWindow<AuthWindow>);
    }

    public void OnOpenCalculationParametersCommandExecuted(object e)
    {
        _exceptionService.SafeExecute(() => _navigation.ShowWindow<CalculationParametersWindow>());
    }

    public void OpenAnotherControlsCommandExecuted<T>(object e)
        where T : UserControl
    {
        _exceptionService.SafeExecute(() =>
        {
            // Если открываем TreeProjectView, сбрасываем проект
            if (typeof(T) == typeof(TreeProjectView))
            {
                var projectViewModel = _serviceProvider.GetRequiredService<ProjectViewModel>();

                projectViewModel.ResetProject();
            }

            _navigation.ShowContent<T>();
        });
    }

    public async Task OnCheckDbConnectionCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(CheckDbConnectionAsync);
    }

    public async Task OnShowAllProjectsCommandExecuted()
    {
        await _exceptionService.SafeExecuteAsync(ShowAllProjectsAsync);
    }

    public async Task OnDeleteSelectedProjectExecuted()
    {
        await _exceptionService.SafeExecuteAsync(DeleteSelectedProjectAsync);
    }

    private async Task CheckDbConnectionAsync()
    {
        var context = _serviceProvider.GetRequiredService<ReAppContext>();
        MainWindowModel.IsConnected = await context.Database.CanConnectAsync();
        MainWindowModel.ConnectionStatusMessage =
            MainWindowModel.IsConnected ? "Соединение установлено" : "Соединение не установлено";
    }

    public async Task<bool> CanAppConnect()
    {
        var context = _serviceProvider.GetRequiredService<ReAppContext>();

        if (context.Database.CanConnect())
            return true;

        return false;
    }

    public async Task ShowAllProjectsAsync()
    {
        var projects = await _projectRepository.GetAllAsync();

        MainWindowModel.AllProjects.Clear();
        foreach (var project in projects)
            MainWindowModel.AllProjects.Add(project);

        _logger.Success("Проекты загружены. Статус: Успешно");
    }

    //Обновление информации о проекте в коллекции AllProjects
    private async Task UpdateProjectStandsQuantity(int projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null || project.Stands.Count == 0) return;

        var existingProject = MainWindowModel.AllProjects.FirstOrDefault(p => p.Id == projectId);

        
        if (existingProject == null)
        {
            MainWindowModel.AllProjects.Add(project);
            existingProject = MainWindowModel.AllProjects.FirstOrDefault(p => p.Id == projectId);
        }

        var index = MainWindowModel.AllProjects.IndexOf(existingProject);
        if (index >= 0)
        {
            MainWindowModel.AllProjects[index] = project;
        }

    }



    public async Task DeleteSelectedProjectAsync()
    {
        var result = _notificationService.ShowConfirmation("Вы уверены, что хотите удалить проект?");

        if (!result)
            return;


        var currentProject = MainWindowModel.SelectedProject;


        var deletingProjectInfo = new
        {
            currentProject.OrderCustomer,
            currentProject.Description
        };

        await _projectRepository.DeleteAsync(currentProject);
        await ShowAllProjectsAsync();

        _notificationService.ShowInfo("Проект успешно удалён");

        _logger.Success($"Удалён проект {deletingProjectInfo.Description} " +
                        $"c заказом покупателя {deletingProjectInfo.OrderCustomer} " +
                        $"Статус: Успешно");

        await _auditService.LogEventAsync(
            _sessionService.CurrentUser.UserLogin,
            $"Пользователь {_sessionService.CurrentUser.UserLogin} удалил проект {deletingProjectInfo.Description}",
            $"Удаление проекта с заказом покупателя {deletingProjectInfo.OrderCustomer}");
    }

    private async Task RecalculateProjectAsync()
    {
        if (MainWindowModel.SelectedProject == null)
        {
            _notificationService.ShowInfo("Проект не выбран");
            return;
        }

        var projectViewModel = _serviceProvider.GetRequiredService<ProjectViewModel>();
        var projectService = _serviceProvider.GetRequiredService<IProjectService>();

        await projectViewModel.LoadProjectInfoAsync(MainWindowModel.SelectedProject.Id);

        await _calculationService.CalculateProjectAsync(projectViewModel.CurrentProjectModel);

        await projectService.UpdateProjectAsync(projectViewModel.CurrentProjectModel);

        CollectionRefreshHelper.SafeRefreshCollection(MainWindowModel.AllProjects);
    }

    #endregion Комманды главного окна
}
