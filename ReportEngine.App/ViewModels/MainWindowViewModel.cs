using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using DevExpress.DataProcessing.InMemoryDataProcessor;
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

    public async void OnRecalculateProjectCommandExecuted(object e)
    {
        await _exceptionService.SafeExecuteAsync(RecalculateProjectAsync);

        _notificationService.ShowInfo("Переформирование завершено");
    }

    public async void OnEditProjectCommandExecuted(object e)
    {
        if (MainWindowModel.SelectedProject == null) return;

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

    public async void OnCopyProjectCommandExecuted(object e)
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            await _dialogService.RunWithProgressDialogAsync(async () =>
            {
                var newProject = MainWindowModel.SelectedProject;

                await _entityProjectClonerService.CloneProjectEntity(newProject);

                MainWindowModel.AllProjects.Add(newProject);

                _logger.Success($"Скопирован проект {MainWindowModel.SelectedProject.OrderCustomer} Статус: Успешно");
            });
        });
    }

    public void OnOpenMainWindowCommandExecuted(object e)
    {
        _ = _exceptionService.SafeExecuteAsync(async () =>
        {
            var projectViewModel = _serviceProvider.GetRequiredService<ProjectViewModel>();

            //принудительно обновляем количество стендов при закрытии проекта и подгружаем свежие данные
            //костыль - приходится постоянно пересчитывать, что занимает время
            //можно при желании распраллелить
            if (projectViewModel != null)
            {
               await RecalculateProjectAsync();

               await UpdateSingleProject(projectViewModel.CurrentProjectModel.CurrentProjectId);
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

    public async void OnCheckDbConnectionCommandExecuted(object e)
    {
        await _exceptionService.SafeExecuteAsync(CheckDbConnectionAsync);
    }

    public async void OnShowAllProjectsCommandExecuted(object e)
    {
        await _exceptionService.SafeExecuteAsync(ShowAllProjectsAsync);
    }

    public async void OnDeleteSelectedProjectExecuted(object e)
    {
        await _exceptionService.SafeExecuteAsync(DeleteSelectedProjectAsync);
    }

    public async Task CheckDbConnectionAsync()
    {
        var context = _serviceProvider.GetRequiredService<ReAppContext>();
        MainWindowModel.IsConnected = context.Database.CanConnect();
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
    public async Task UpdateSingleProject(int projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId); 

        // всратая вставка, но пока быстро (до 1к проектов)
        var index = MainWindowModel.AllProjects.IndexOf(MainWindowModel.AllProjects.First(p => p.Id == projectId));

        if (index >= 0)
            MainWindowModel.AllProjects[index] = project;
    }



    public async Task DeleteSelectedProjectAsync()
    {
        var result = _notificationService.ShowConfirmation("Вы уверены, что хотите удалить проект?");

        if (!result)
            return;



        var currentProject = MainWindowModel.SelectedProject;


        var deletingProjectInfo = new
        {
            OrderCustomer = currentProject.OrderCustomer,
            Description = currentProject.Description
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
