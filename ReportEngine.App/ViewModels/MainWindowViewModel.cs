using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.Commands;
using ReportEngine.App.Commands.Initializers;
using ReportEngine.App.Commands.Providers;
using ReportEngine.App.Model;
using ReportEngine.App.Services.Cloners;
using ReportEngine.App.Services.Core;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.Services.Navigation;
using ReportEngine.App.Views.Controls;
using ReportEngine.App.Views.Settings.CalculationParameters;
using ReportEngine.App.Views.Windows;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    private readonly ICalculationService _calculationService;
    private readonly IDialogService _dialogService;
    private readonly EntityProjectClonerService _entityProjectClonerService;
    private readonly NavigationService _navigation;
    private readonly INotificationService _notificationService;
    private readonly IProjectInfoRepository _projectRepository;
    private readonly IProjectService _projectService;
    private readonly IServiceProvider _serviceProvider;
    private readonly SessionService _sessionService;

    #region Конструктор

    public MainWindowViewModel(
        IServiceProvider serviceProvider,
        NavigationService navigation,
        IProjectInfoRepository projectRepository,
        ICalculationService calculationService,
        INotificationService notificationService,
        IProjectService projectService,
        IDialogService dialogService,
        EntityProjectClonerService entityProjectClonerService,
        SessionService  sessionService)
    {
        _notificationService = notificationService;
        _calculationService = calculationService;
        _serviceProvider = serviceProvider;
        _projectRepository = projectRepository;
        _navigation = navigation;
        _projectService = projectService;
        _dialogService = dialogService;
        _entityProjectClonerService = entityProjectClonerService;
        _sessionService = sessionService;
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
        ExceptionHelper.SafeExecute(() => _navigation.ShowGenericWindow<T, T>());
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

    public async void OnRecalculateProjectCommandExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(RecalculateProjectAsync);
    }

    public async void OnEditProjectCommandExecuted(object e)
    {
        if (MainWindowModel.SelectedProject == null) return;

        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var projectViewModel = _serviceProvider.GetRequiredService<ProjectViewModel>();

            await _dialogService.RunWithProgressDialogAsync(async () =>
            {
                await projectViewModel.LoadProjectInfoAsync(MainWindowModel.SelectedProject.Id);
                _navigation.ShowContent<TreeProjectView>();
            });
        });
    }

    public async void OnCopyProjectCommandExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            await _dialogService.RunWithProgressDialogAsync(async () =>
            {
                var newProject = MainWindowModel.SelectedProject;

                await _entityProjectClonerService.CloneProjectEntity(newProject);

                MainWindowModel.AllProjects.Add(newProject);
            });
        });
    }

    public void OnOpenMainWindowCommandExecuted(object e)
    {
        ExceptionHelper.SafeExecute(() =>
        {
            var projectViewModel = _serviceProvider.GetRequiredService<ProjectViewModel>();

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
        ExceptionHelper.SafeExecute(_navigation.ShowWindow<T>);
    }

    public void OpenAuthWindowCommandExecuted<T>(object e)
        where T : Window
    {
        ExceptionHelper.SafeExecute(_navigation.ShowWindow<AuthWindow>);
    }

    public void OnOpenCalculationParametersCommandExecuted(object e)
    {
        ExceptionHelper.SafeExecute(() => _navigation.ShowWindow<CalculationParametersWindow>());
    }

    public void OpenAnotherControlsCommandExecuted<T>(object e)
        where T : UserControl
    {
        ExceptionHelper.SafeExecute(() =>
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
        await ExceptionHelper.SafeExecuteAsync(CheckDbConnectionAsync);
    }

    public async void OnShowAllProjectsCommandExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(ShowAllProjectsAsync);
    }

    public async void OnDeleteSelectedProjectExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(DeleteSelectedProjectAsync);
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
        foreach (var project in projects) MainWindowModel.AllProjects.Add(project);
    }

    public async Task DeleteSelectedProjectAsync()
    {
        var currentProject = MainWindowModel.SelectedProject;
        await _projectRepository.DeleteAsync(currentProject);
        await ShowAllProjectsAsync();
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

        _notificationService.ShowInfo("Переформирование завершено");
    }

    #endregion Комманды главного окна
}
