using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.Commands;
using ReportEngine.App.Commands.Initializers;
using ReportEngine.App.Commands.Providers;
using ReportEngine.App.Model;
using ReportEngine.App.Services;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.Views.Controls;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    private readonly ICalculationService _calculationService;
    private readonly NavigationService _navigation;
    private readonly INotificationService _notificationService;
    private readonly IProjectInfoRepository _projectRepository;
    private readonly IServiceProvider _serviceProvider;

    #region Конструктор

    public MainWindowViewModel(
        IServiceProvider serviceProvider,
        NavigationService navigation,
        IProjectInfoRepository projectRepository,
        ICalculationService calculationService,
        INotificationService notificationService)
    {
        _notificationService = notificationService;
        _calculationService = calculationService;
        _serviceProvider = serviceProvider;
        _projectRepository = projectRepository;
        _navigation = navigation;

        InitializeMainWindowCommands();
        InitializeGenericEquipCommands();
    }

    #endregion

    public MainWindowModel MainWindowModel { get; set; } = new();
    public GenericEquipCommandProvider GenericEquipCommandProvider { get; set; } = new();
    public MainWindowCommandProvider MainWindowCommandProvider { get; set; } = new();

    #region Дженерик команды

    public void OnOpenGenericWindowCommandExecuted<T, TEquip>(object e)
        where T : class, IBaseEquip, new()
    {
        ExceptionHelper.SafeExecute(() => _navigation.ShowGenericWindow<T, T>());
    }

    #endregion

    #region Методы

    public void InitializeMainWindowCommands() // Нужно придумать как отрефакторить этого монстра 
    {
        MainWindowCommandsInitializer.InitializeCommands(this);
    }

    public void InitializeGenericEquipCommands() // Нужно придумать как отрефакторить этого монстра
    {
        MainWindowCommandsInitializer.InitializeGenericCommands(this);
    }

    #endregion

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
            await projectViewModel.LoadProjectInfoAsync(MainWindowModel.SelectedProject.Id);
            _navigation.ShowContent<TreeProjectView>();
        });
    }

    public void OnOpenMainWindowCommandExecuted(object e)
    {
        ExceptionHelper.SafeExecute(() =>
        {
            _navigation.CloseContent();
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.MainContentControl.Content = mainWindow.MainDataGrid;
        });
    }

    public void OpenOthersWindowCommandExecuted<T>(object e)
        where T : Window
    {
        ExceptionHelper.SafeExecute(() => _navigation.ShowWindow<T>());
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
        MainWindowModel.AllProjects = new ObservableCollection<ProjectInfo>(projects);
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

    #endregion
}