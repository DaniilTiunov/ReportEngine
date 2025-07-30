using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Commands;
using ReportEngine.App.Services;
using ReportEngine.App.Views.Controls;
using ReportEngine.App.Views.Windows;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Armautre;
using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.Drainage;
using ReportEngine.Domain.Entities.ElectricSockets;
using ReportEngine.Domain.Entities.Frame;
using ReportEngine.Domain.Entities.Pipes;
using ReportEngine.Domain.Repositories.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region DI сервисов
        private readonly IProjectInfoRepository _projectRepository;
        private readonly NavigationService _navigation;
        private readonly IServiceProvider _serviceProvider;
        #endregion

        #region Приватные поля
        private string _connectionStatusMessage;
        private ObservableCollection<ProjectInfo> _allProjects;
        private ProjectInfo _selectedProject;
        #endregion

        #region Публичные поля для привязки
        public ProjectInfo SelectedProject
        {
            get => _selectedProject;
            set => Set(ref _selectedProject, value);
        }
        public string ConnectionStatusMessage
        {
            get => _connectionStatusMessage;
            set => Set(ref _connectionStatusMessage, value);
        }
        public bool IsConnected { get; private set; }
        public ObservableCollection<ProjectInfo> AllProjects
        {
            get => _allProjects;
            set => Set(ref _allProjects, value);
        }
        #endregion

        #region Конструктор
        public MainWindowViewModel(IServiceProvider serviceProvider, NavigationService navigation, IProjectInfoRepository projectRepository)
        {
            _serviceProvider = serviceProvider;
            _projectRepository = projectRepository;
            _navigation = navigation;

            InitializeCommands();
            InitializeGenericEquipCommands();
        }
        #endregion

        #region Методы        
        public void InitializeCommands()
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
            OpenHeaterPipeCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, HeaterPipe>, CanAllCommandsExecute);
            OpenCarbonPipeCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, CarbonPipe>, CanAllCommandsExecute);
            OpenStainlessPipeCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, StainlessPipe>, CanAllCommandsExecute);

            OpenHeaterArmatureCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, HeaterArmature>, CanAllCommandsExecute);
            OpenCarbonArmatureCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, CarbonArmature>, CanAllCommandsExecute);
            OpenStainlessArmatureCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, StainlessArmature>, CanAllCommandsExecute);

            OpenCarbonSocketsCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, CarbonSocket>, CanAllCommandsExecute);
            OpenStainlessSocketsCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, StainlessSocket>, CanAllCommandsExecute);
            OpenHeaterSocketsCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, HeaterSocket>, CanAllCommandsExecute);

            OpenDrainageCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, Drainage>, CanAllCommandsExecute);

            OpenFrameDetailsCommand = new RelayCommand(OnOpenGenericWindowCommandExecuted<IBaseEquip, FrameDetail>, CanAllCommandsExecute);
        }
        #endregion

        #region Комманды
        public ICommand OpenMainWindowCommand { get; set; }
        public bool CanAllCommandsExecute(object e) => true;
        public void OnOpenMainWindowCommandExecuted(object e)
        {
            try
            {
                _navigation.CloseContent();
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.MainContentControl.Content = mainWindow.MainDataGrid;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public ICommand OpenTreeViewCommand { get; set; }
        public void OnOpenTreeViewCommandExecuted(object e)
        {
            try
            {
                _navigation.ShowContent<TreeProjectView>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public ICommand CloseAppCommand { get; set; }
        public void OnCloseAppCommandExecuted(object e) => Application.Current.Shutdown();
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

            IsConnected = context.Database.CanConnect();
            ConnectionStatusMessage = IsConnected ? "Соединение установлено" : "Соединение не установлено";

        }
        public ICommand ShowAllProjectsCommand { get; set; }
        public async void OnShowAllProjectsCommandExecuted(object e)
        {
            try
            {
                var projects = await _projectRepository.GetAllAsync();
                AllProjects = new ObservableCollection<ProjectInfo>(projects);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        public ICommand DeleteSelectedProjectCommand { get; set; }
        public async void OnDeleteSelectedProjectExecuted(object e)
        {
            try
            {
                var currentProject = _selectedProject;

                await _projectRepository.DeleteAsync(currentProject);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        #endregion

        #region Дженерик команды
        public ICommand OpenCarbonPipeCommand { get; set; }
        public ICommand OpenHeaterPipeCommand { get; set; }
        public ICommand OpenStainlessPipeCommand { get; set; }
        public ICommand OpenCarbonArmatureCommand { get; set; }
        public ICommand OpenHeaterArmatureCommand { get; set; }
        public ICommand OpenStainlessArmatureCommand { get; set; }
        public ICommand OpenCarbonSocketsCommand { get; set; }
        public ICommand OpenStainlessSocketsCommand { get; set; }
        public ICommand OpenHeaterSocketsCommand { get; set; }
        public ICommand OpenDrainageCommand { get; set; }
        public ICommand OpenGenericWindowCommand { get; set; }
        public ICommand OpenFrameDetailsCommand { get; set; }
        public void OnOpenGenericWindowCommandExecuted<T, TEquip>(object e)
            where T : IBaseEquip
            where TEquip : class, new()
        {
            try
            {
                _navigation.ShowGenericWindow<T, TEquip>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
    }
}
