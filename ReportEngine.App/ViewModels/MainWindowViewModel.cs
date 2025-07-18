using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Commands;
using ReportEngine.App.Services;
using ReportEngine.App.Views.Controls;
using ReportEngine.App.Views.Windows;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Config.DebugConsol;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        #endregion

        #region Публичные поля для привязки
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
            #region Комманды
            CloseAppCommand = new RelayCommand(OnCloseAppCommandExecuted, CanCloseAppCommandExecute);
            OpenAllUsersCommand = new RelayCommand(OnOpenAllUsersCommandExecuted, CanOpenAllUsersCommandExecute);
            ChekDbConnectionCommand = new RelayCommand(OnChekDbConnectionCommandExecuted, CanChekDbConnectionCommandExecute);
            OpenTreeViewCommand = new RelayCommand(OnOpenTreeViewCommandExecuted, CanOpenTreeViewCommandExecute);
            ShowAllProjectsCommand = new RelayCommand(OnShowAllProjectsCommandExecuted, CanShowAllProjectsCommandExecute);
            #endregion

            _serviceProvider = serviceProvider;
            _projectRepository = projectRepository;
            _navigation = navigation;
        }
        #endregion
        #region Комманды
        public ICommand OpenTreeViewCommand { get; }
        public bool CanOpenTreeViewCommandExecute(object e) => true;
        public void OnOpenTreeViewCommandExecuted(object e)
        {
            try
            {
                _navigation.ShowContent<TreeProjectView>();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        } 
        public ICommand CloseAppCommand { get; }
        public bool CanCloseAppCommandExecute(object e) => true;
        public void OnCloseAppCommandExecuted(object e) => Application.Current.Shutdown();

        public ICommand OpenAllUsersCommand { get; }
        public bool CanOpenAllUsersCommandExecute(object e) => true;
        public void OnOpenAllUsersCommandExecuted(object e)
        {
            _navigation.ShowWindow<UsersView>();
        }

        public ICommand ChekDbConnectionCommand { get; }
        public bool CanChekDbConnectionCommandExecute(object e) => true;
        public void OnChekDbConnectionCommandExecuted(object e)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ReAppContext>();

            IsConnected = context.Database.CanConnect();
            ConnectionStatusMessage = IsConnected ? "Соединение установлено" : "Соединение не установлено";

        }

        public ICommand ShowAllProjectsCommand {  get; }
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
        #endregion


        #region Методы

        #endregion
    }
}
