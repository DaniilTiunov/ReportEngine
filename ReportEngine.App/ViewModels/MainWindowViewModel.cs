using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Views.UpdateInformation;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly IBaseRepository<User> _userRepository;

        private readonly IServiceProvider _serviceProvider;     
        public string ConnectionStatusMessage { get; private set; } = string.Empty;
        public bool IsConnected { get; private set; }
           
        public MainWindowViewModel(IBaseRepository<User> userRepository, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _userRepository = userRepository;           
            Task.Run(() => UpdateConnectionStatus()).Wait();
        }
        private void UpdateConnectionStatus()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ReAppContext>();

            IsConnected = context.Database.CanConnect();
            ConnectionStatusMessage = IsConnected ? "Соединение установлено" : "Соединение не установлено";
        }      
    }  
}
