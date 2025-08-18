using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Commands;
using ReportEngine.App.Model;
using ReportEngine.App.Services;
using ReportEngine.App.Views.Controls;
using ReportEngine.App.Views.Windows;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Armautre;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.Braces;
using ReportEngine.Domain.Entities.Drainage;
using ReportEngine.Domain.Entities.ElectricComponents;
using ReportEngine.Domain.Entities.ElectricSockets;
using ReportEngine.Domain.Entities.Frame;
using ReportEngine.Domain.Entities.Other;
using ReportEngine.Domain.Entities.Pipes;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;

namespace ReportEngine.App.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    #region Конструктор

    public MainWindowViewModel(IServiceProvider serviceProvider, NavigationService navigation,
        IProjectInfoRepository projectRepository)
    {
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

    #region DI сервисов

    private readonly IProjectInfoRepository _projectRepository;
    private readonly NavigationService _navigation;
    private readonly IServiceProvider _serviceProvider;

    #endregion

    #region Методы

    public void InitializeMainWindowCommands() // Нужно придумать как отрефакторить этого монстра 
    {
        MainWindowCommandProvider.CloseAppCommand = new RelayCommand(OnCloseAppCommandExecuted, CanAllCommandsExecute);
        MainWindowCommandProvider.OpenAllUsersCommand =
            new RelayCommand(OpenOthersWindowCommandExecuted<UsersView>, CanAllCommandsExecute);
        MainWindowCommandProvider.OpenAllObvyazkiCommand =
            new RelayCommand(OpenOthersWindowCommandExecuted<ObvyazkiView>, CanAllCommandsExecute);
        MainWindowCommandProvider.OpenAllCompaniesCommand =
            new RelayCommand(OpenOthersWindowCommandExecuted<CompanyView>, CanAllCommandsExecute);
        MainWindowCommandProvider.OpenFormedFramesCommand =
            new RelayCommand(OpenOthersWindowCommandExecuted<FormedFrameView>, CanAllCommandsExecute);
        MainWindowCommandProvider.OpenTreeViewCommand =
            new RelayCommand(OpenAnotherControlsCommandExecuted<TreeProjectView>, CanAllCommandsExecute);
        MainWindowCommandProvider.ChekDbConnectionCommand =
            new RelayCommand(OnChekDbConnectionCommandExecuted, CanAllCommandsExecute);
        MainWindowCommandProvider.ShowAllProjectsCommand =
            new RelayCommand(OnShowAllProjectsCommandExecuted, CanAllCommandsExecute);
        MainWindowCommandProvider.DeleteSelectedProjectCommand =
            new RelayCommand(OnDeleteSelectedProjectExecuted, CanAllCommandsExecute);
        MainWindowCommandProvider.OpenMainWindowCommand =
            new RelayCommand(OnOpenMainWindowCommandExecuted, CanAllCommandsExecute);
        MainWindowCommandProvider.EditProjectCommand =
            new RelayCommand(OnEditProjectCommandExecuted, CanAllCommandsExecute);
        MainWindowCommandProvider.OpenAllDrainagesCommand =
            new RelayCommand(OpenOthersWindowCommandExecuted<FormedDrainagesView>, CanAllCommandsExecute);
    }

    public void InitializeGenericEquipCommands() // Нужно придумать как отрефакторить этого монстра
    {
        GenericEquipCommandProvider.OpenHeaterPipeCommand =
            new RelayCommand(OnOpenGenericWindowCommandExecuted<HeaterPipe, HeaterPipe>, CanAllCommandsExecute);
        GenericEquipCommandProvider.OpenCarbonPipeCommand =
            new RelayCommand(OnOpenGenericWindowCommandExecuted<CarbonPipe, CarbonPipe>, CanAllCommandsExecute);
        GenericEquipCommandProvider.OpenStainlessPipeCommand =
            new RelayCommand(OnOpenGenericWindowCommandExecuted<StainlessPipe, StainlessPipe>, CanAllCommandsExecute);

        GenericEquipCommandProvider.OpenHeaterArmatureCommand =
            new RelayCommand(OnOpenGenericWindowCommandExecuted<HeaterArmature, HeaterArmature>, CanAllCommandsExecute);
        GenericEquipCommandProvider.OpenCarbonArmatureCommand =
            new RelayCommand(OnOpenGenericWindowCommandExecuted<CarbonArmature, CarbonArmature>, CanAllCommandsExecute);
        GenericEquipCommandProvider.OpenStainlessArmatureCommand = new RelayCommand(
            OnOpenGenericWindowCommandExecuted<StainlessArmature, StainlessArmature>, CanAllCommandsExecute);

        GenericEquipCommandProvider.OpenCarbonSocketsCommand =
            new RelayCommand(OnOpenGenericWindowCommandExecuted<CarbonSocket, CarbonSocket>, CanAllCommandsExecute);
        GenericEquipCommandProvider.OpenStainlessSocketsCommand = new RelayCommand(
            OnOpenGenericWindowCommandExecuted<StainlessSocket, StainlessSocket>, CanAllCommandsExecute);
        GenericEquipCommandProvider.OpenHeaterSocketsCommand =
            new RelayCommand(OnOpenGenericWindowCommandExecuted<HeaterSocket, HeaterSocket>, CanAllCommandsExecute);

        GenericEquipCommandProvider.OpenDrainageCommand =
            new RelayCommand(OnOpenGenericWindowCommandExecuted<Drainage, Drainage>, CanAllCommandsExecute);

        GenericEquipCommandProvider.OpenFrameDetailsCommand =
            new RelayCommand(OnOpenGenericWindowCommandExecuted<FrameDetail, FrameDetail>, CanAllCommandsExecute);
        GenericEquipCommandProvider.OpenPillarEquipCommand =
            new RelayCommand(OnOpenGenericWindowCommandExecuted<PillarEqiup, PillarEqiup>, CanAllCommandsExecute);
        GenericEquipCommandProvider.OpenFrameRollCommand =
            new RelayCommand(OnOpenGenericWindowCommandExecuted<FrameRoll, FrameRoll>, CanAllCommandsExecute);

        GenericEquipCommandProvider.OpenBoxesBracesommand =
            new RelayCommand(OnOpenGenericWindowCommandExecuted<BoxesBrace, BoxesBrace>, CanAllCommandsExecute);
        GenericEquipCommandProvider.OpenDrainageBracesCommand =
            new RelayCommand(OnOpenGenericWindowCommandExecuted<DrainageBrace, DrainageBrace>, CanAllCommandsExecute);
        GenericEquipCommandProvider.OpenSensorsBracesCommand =
            new RelayCommand(OnOpenGenericWindowCommandExecuted<SensorBrace, SensorBrace>, CanAllCommandsExecute);

        GenericEquipCommandProvider.OpenCabelBoxeCommand =
            new RelayCommand(OnOpenGenericWindowCommandExecuted<CabelBoxe, CabelBoxe>, CanAllCommandsExecute);
        GenericEquipCommandProvider.OpenCabelInputCommand =
            new RelayCommand(OnOpenGenericWindowCommandExecuted<CabelInput, CabelInput>, CanAllCommandsExecute);
        GenericEquipCommandProvider.OpenCabelProductionCommand = new RelayCommand(
            OnOpenGenericWindowCommandExecuted<CabelProduction, CabelProduction>, CanAllCommandsExecute);
        GenericEquipCommandProvider.OpenCabelProtectionCommand = new RelayCommand(
            OnOpenGenericWindowCommandExecuted<CabelProtection, CabelProtection>, CanAllCommandsExecute);
        GenericEquipCommandProvider.OpenHeaterCommand =
            new RelayCommand(OnOpenGenericWindowCommandExecuted<Heater, Heater>, CanAllCommandsExecute);

        GenericEquipCommandProvider.OpenConteinersCommand =
            new RelayCommand(OnOpenGenericWindowCommandExecuted<Container, Container>, CanAllCommandsExecute);
        GenericEquipCommandProvider.OpenOthersCommand =
            new RelayCommand(OnOpenGenericWindowCommandExecuted<Other, Other>, CanAllCommandsExecute);
    }

    #endregion

    #region Комманды главного окна

    public bool CanAllCommandsExecute(object e)
    {
        return true;
    }

    public async void OnEditProjectCommandExecuted(object e)
    {
        if (MainWindowModel.SelectedProject == null) return;

        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var projectViewModel = _serviceProvider.GetRequiredService<ProjectViewModel>();
            await projectViewModel.LoadProjectInfoAsync(MainWindowModel.SelectedProject.Id);
            await projectViewModel.LoadStandsDataAsync();
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

    public static void OnCloseAppCommandExecuted(object e)
    {
        Application.Current.Shutdown();
    }

    public void OnChekDbConnectionCommandExecuted(object e)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ReAppContext>();

        MainWindowModel.IsConnected = context.Database.CanConnect();
        MainWindowModel.ConnectionStatusMessage =
            MainWindowModel.IsConnected ? "Соединение установлено" : "Соединение не установлено";
    }

    public async void OnShowAllProjectsCommandExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var projects = await _projectRepository.GetAllAsync();
            MainWindowModel.AllProjects = new ObservableCollection<ProjectInfo>(projects);
        });
    }

    public async void OnDeleteSelectedProjectExecuted(object e)
    {
        var currentProject = MainWindowModel.SelectedProject;

        await ExceptionHelper.SafeExecuteAsync(() => _projectRepository.DeleteAsync(currentProject));

        OnShowAllProjectsCommandExecuted(e);
    }

    #endregion
}