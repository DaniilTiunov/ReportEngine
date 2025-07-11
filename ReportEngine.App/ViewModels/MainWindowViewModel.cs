using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Commands;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using System.Windows;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels
{
    public class MainWindowViewModel
    {
        #region DI сервисов
        private readonly IBaseRepository<User> _userRepository;
        private readonly IServiceProvider _serviceProvider;
        #endregion

        #region Публичные поля для привязки
        public string ConnectionStatusMessage { get; private set; } = string.Empty;
        public bool IsConnected { get; private set; }
        #endregion

        #region Комманды
        public ICommand CloseAppCommand { get; }
        public bool CanCloseAppCommandExecute(object e) => true;
        public void OnCloseAppCommandExecuted(object e) => Application.Current.Shutdown();
        #endregion

        #region Конструктор
        public MainWindowViewModel(IBaseRepository<User> userRepository, IServiceProvider serviceProvider)
        {
            #region Комманды
            CloseAppCommand = new RelayCommand(OnCloseAppCommandExecuted, CanCloseAppCommandExecute);
            #endregion

            _serviceProvider = serviceProvider;
            _userRepository = userRepository;
            Task.Run(() => UpdateConnectionStatus()).Wait();
        }
        #endregion

        #region Методы
        private void UpdateConnectionStatus()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ReAppContext>();

            IsConnected = context.Database.CanConnect();
            ConnectionStatusMessage = IsConnected ? "Соединение установлено" : "Соединение не установлено";
        }
        #endregion
    }
}
