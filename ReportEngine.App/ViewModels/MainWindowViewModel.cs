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
using ReportEngine.Domain.Entities.Drainage;
using ReportEngine.Domain.Entities.ElectricSockets;
using ReportEngine.Domain.Entities.Frame;
using ReportEngine.Domain.Entities.Pipes;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region DI сервисов
        private readonly IProjectInfoRepository _projectRepository;
        private readonly NavigationService _navigation;
        private readonly IServiceProvider _serviceProvider;
        #endregion
        public MainWindowModel MainWindowModel { get; set; } = new();
        public GenericEquipCommandProvider GenericEquipCommandProvider { get; set; } = new();

        #region Конструктор
        public MainWindowViewModel(IServiceProvider serviceProvider, NavigationService navigation, IProjectInfoRepository projectRepository)
        {
            _serviceProvider = serviceProvider;
            _projectRepository = projectRepository;
            _navigation = navigation;

            InitializeMainWindowCommands();
            InitializeGenericEquipCommands();
        }
        #endregion

        #region Методы        
        public void InitializeMainWindowCommands()
        {
            CloseAppCommand = new RelayCommand(OnCloseAppCommandExecuted, CanAllCommandsExecute);
            OpenAllUsersCommand = new RelayCommand(OnOpenAllUsersCommandExecuted, CanAllCommandsExecute);
            ChekDbConnectionCommand = new RelayCommand(OnChekDbConnectionCommandExecuted, CanAllCommandsExecute);
            OpenTreeViewCommand = new RelayCommand(OnOpenTreeViewCommandExecuted, CanAllCommandsExecute);
            ShowAllProjectsCommand = new RelayCommand(OnShowAllProjectsCommandExecuted, CanAllCommandsExecute);
            DeleteSelectedProjectCommand = new RelayCommand(OnDeleteSelectedProjectExecuted, CanAllCommandsExecute);
            OpenMainWindowCommand = new RelayCommand(OnOpenMainWindowCommandExecuted, CanAllCommandsExecute);
        }
        public void InitializeGenericEquipCommands()
        {
            GenericEquipCommandProvider.OpenHeaterPipeCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, HeaterPipe>, CanAllCommandsExecute);
            GenericEquipCommandProvider.OpenCarbonPipeCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, CarbonPipe>, CanAllCommandsExecute);
            GenericEquipCommandProvider.OpenStainlessPipeCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, StainlessPipe>, CanAllCommandsExecute);

            GenericEquipCommandProvider.OpenHeaterArmatureCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, HeaterArmature>, CanAllCommandsExecute);
            GenericEquipCommandProvider.OpenCarbonArmatureCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, CarbonArmature>, CanAllCommandsExecute);
            GenericEquipCommandProvider.OpenStainlessArmatureCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, StainlessArmature>, CanAllCommandsExecute);

            GenericEquipCommandProvider.OpenCarbonSocketsCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, CarbonSocket>, CanAllCommandsExecute);
            GenericEquipCommandProvider.OpenStainlessSocketsCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, StainlessSocket>, CanAllCommandsExecute);
            GenericEquipCommandProvider.OpenHeaterSocketsCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, HeaterSocket>, CanAllCommandsExecute);

            GenericEquipCommandProvider.OpenDrainageCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, Drainage>, CanAllCommandsExecute);

            GenericEquipCommandProvider.OpenFrameDetailsCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, FrameDetail>, CanAllCommandsExecute);
        }
        #endregion
        #region Комманды главного окна
        public ICommand OpenMainWindowCommand { get; set; }
        public bool CanAllCommandsExecute(object e) => true;
        public void OnOpenMainWindowCommandExecuted(object e)
        {
            ExceptionHelper.SafeExecute(() =>
            {
                _navigation.CloseContent();
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.MainContentControl.Content = mainWindow.MainDataGrid;
            });
        }
        public ICommand OpenTreeViewCommand { get; set; }
        public void OnOpenTreeViewCommandExecuted(object e)
        {
            _navigation.ShowContent<TreeProjectView>();
        }
        public ICommand CloseAppCommand { get; set; }
        public static void OnCloseAppCommandExecuted(object e) => Application.Current.Shutdown();
        public ICommand OpenAllUsersCommand { get; set; }
        public void OnOpenAllUsersCommandExecuted(object e)
        {
            _navigation.ShowWindow<UsersView>();
        }
        public ICommand ChekDbConnectionCommand { get; set; }
        public void OnChekDbConnectionCommandExecuted(object e)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ReAppContext>();

            MainWindowModel.IsConnected = context.Database.CanConnect();
            MainWindowModel.ConnectionStatusMessage = MainWindowModel.IsConnected ? "Соединение установлено" : "Соединение не установлено";
        }
        public ICommand ShowAllProjectsCommand { get; set; }
        public async void OnShowAllProjectsCommandExecuted(object e)
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var projects = await _projectRepository.GetAllAsync();
                MainWindowModel.AllProjects = new ObservableCollection<ProjectInfo>(projects);
            });
        }
        public ICommand DeleteSelectedProjectCommand { get; set; }
        public async void OnDeleteSelectedProjectExecuted(object e)
        {
            var currentProject = MainWindowModel.SelectedProject;

            await ExceptionHelper.SafeExecuteAsync(() => _projectRepository.DeleteAsync(currentProject));
        }
        #endregion

        #region Дженерик команды
        public void OnOpenGenericWindowCommandExecuted<T, TEquip>(object e)
            where T : IBaseEquip
            where TEquip : class, new()
        {
            ExceptionHelper.SafeExecute(() => _navigation.ShowGenericWindow<T, TEquip>());
        }
        #endregion
    }
}
