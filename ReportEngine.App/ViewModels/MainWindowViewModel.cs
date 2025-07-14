using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Commands;
using ReportEngine.App.Services;
using ReportEngine.App.Views;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using System.Windows;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region DI сервисов
        private readonly NavigationService _navigation;
        private readonly IServiceProvider _serviceProvider;
        #endregion

        #region Приватные поля
        private string _connectionStatusMessage;
        #endregion

        #region Публичные поля для привязки
        public string ConnectionStatusMessage { get => _connectionStatusMessage; set => Set(ref _connectionStatusMessage, value); }
        public bool IsConnected { get; private set; }
        #endregion

        #region Комманды
        public ICommand CloseAppCommand { get; }
        public bool CanCloseAppCommandExecute(object e) => true;
        public void OnCloseAppCommandExecuted(object e) => Application.Current.Shutdown();

        public ICommand OpenAllUsersCommand { get; }
        public bool CanOpenAllUsersCommandExecute(object e) => true;
        public void OnOpenAllUsersCommandExecuted(object e)
        {
            _navigation.ShowWindow<UsersView>();
        }

        public ICommand OpenAllUsersAsContentCommand { get; }
        public bool CanOpenAllUsersAsContentCommandExecute(object e) => true;
        public void OnOpenAllUsersAsContentCommandExecuted(object e) => _navigation.ShowAsContent<UsersView>();

        public ICommand ChekDbConnectionCommand { get; }
        public bool CanChekDbConnectionCommandCommandExecute(object e) => true;
        public void OnChekDbConnectionCommandExecuted(object e) 
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ReAppContext>();

            IsConnected = context.Database.CanConnect();
            ConnectionStatusMessage = IsConnected ? "Соединение установлено" : "Соединение не установлено";
        }
        #endregion

        #region Конструктор
        public MainWindowViewModel(IServiceProvider serviceProvider, NavigationService navigation)
        {
            #region Комманды
            CloseAppCommand = new RelayCommand(OnCloseAppCommandExecuted, CanCloseAppCommandExecute);
            OpenAllUsersCommand = new RelayCommand(OnOpenAllUsersCommandExecuted, CanOpenAllUsersCommandExecute);
            OpenAllUsersAsContentCommand = new RelayCommand(OnOpenAllUsersAsContentCommandExecuted, CanOpenAllUsersAsContentCommandExecute);
            ChekDbConnectionCommand = new RelayCommand(OnChekDbConnectionCommandExecuted, CanChekDbConnectionCommandCommandExecute);
            #endregion

            _serviceProvider = serviceProvider;
            _navigation = navigation;
        }
        #endregion

        #region Методы

        #endregion
    }
}
