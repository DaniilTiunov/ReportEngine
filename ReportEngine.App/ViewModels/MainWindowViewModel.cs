using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Commands;
using ReportEngine.App.Services;
using ReportEngine.App.Views.Controls;
using ReportEngine.App.Views.Windows;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.ElectricComponents;
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
        private readonly IBaseRepository<ProjectInfo> _projectRepository;
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
        public MainWindowViewModel(IServiceProvider serviceProvider, NavigationService navigation, IBaseRepository<ProjectInfo> projectRepository)
        {
            _serviceProvider = serviceProvider;
            _projectRepository = projectRepository;
            _navigation = navigation;

            InitializeCommands();
        }
        #endregion

        #region Методы        
        public void InitializeCommands()
        {
            OpenCarbonPipeCommand = new RelayCommand(OnOpenCarbonPipeCommandExecuted, CanOpenCarbonPipeCommandExecute);
            CloseAppCommand = new RelayCommand(OnCloseAppCommandExecuted, CanCloseAppCommandExecute);
            OpenAllUsersCommand = new RelayCommand(OnOpenAllUsersCommandExecuted, CanOpenAllUsersCommandExecute);
            ChekDbConnectionCommand = new RelayCommand(OnChekDbConnectionCommandExecuted, CanChekDbConnectionCommandExecute);
            OpenTreeViewCommand = new RelayCommand(OnOpenTreeViewCommandExecuted, CanOpenTreeViewCommandExecute);
            ShowAllProjectsCommand = new RelayCommand(OnShowAllProjectsCommandExecuted, CanShowAllProjectsCommandExecute);
            DeleteSelectedProjectCommand = new RelayCommand(OnDeleteSelectedProjectExecuted, CanDeleteSelectedProjectExecute);
            OpenMainWindowCommand = new RelayCommand(OnOpenMainWindowCommandExecuted, CanOpenMainWindowCommandExecute);
        }
        #endregion
        #region Комманды
        public ICommand OpenMainWindowCommand { get; set; }
        public bool CanOpenMainWindowCommandExecute(object e) => true;
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
        public bool CanOpenTreeViewCommandExecute(object e) => true;
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
        public bool CanCloseAppCommandExecute(object e) => true;
        public void OnCloseAppCommandExecuted(object e) => Application.Current.Shutdown();

        public ICommand OpenAllUsersCommand { get; set; }
        public bool CanOpenAllUsersCommandExecute(object e) => true;
        public void OnOpenAllUsersCommandExecuted(object e)
        {
            _navigation.ShowWindow<UsersView>();
        }
        public ICommand ChekDbConnectionCommand { get; set; }
        public bool CanChekDbConnectionCommandExecute(object e) => true;
        public void OnChekDbConnectionCommandExecuted(object e)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ReAppContext>();

            IsConnected = context.Database.CanConnect();
            ConnectionStatusMessage = IsConnected ? "Соединение установлено" : "Соединение не установлено";

        }
        public ICommand ShowAllProjectsCommand { get; set; }
        public bool CanShowAllProjectsCommandExecute(object e) => true;
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
        public bool CanDeleteSelectedProjectExecute(object e) => true;
        public async void OnDeleteSelectedProjectExecuted(object e)
        {
            try
            {
                var currentProject = _selectedProject;

                await _projectRepository.DeleteAsync(currentProject);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }




        public ICommand OpenCarbonPipeCommand { get; set; }
        public bool CanOpenCarbonPipeCommandExecute(object e) => true;
        public void OnOpenCarbonPipeCommandExecuted(object e)
        {
            try
            {
                _navigation.ShowGenericWindow<CarbonPipe>();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        #endregion
    }
}
