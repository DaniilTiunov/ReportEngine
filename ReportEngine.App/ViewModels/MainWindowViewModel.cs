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
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IServiceProvider _serviceProvider;

        private string _userSecondName;
        private string _userName;

        public string ConnectionStatusMessage { get; private set; } = string.Empty;
        public bool IsConnected { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public string UserSecondName
        {
            get => _userSecondName;
            set
            {
                _userSecondName = value;
                OnPropertyChanged();
            }
        }
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }
        public ICommand AddUserCommand { get; }
        public ICommand CheckForUpdatesCommand { get; }

        public MainWindowViewModel(IBaseRepository<User> userRepository, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _userRepository = userRepository;
            AddUserCommand = new RelayCommand(async _ => await AddUser());
            CheckForUpdatesCommand = new RelayCommand(async _ => await CheckForUpdates());
            Task.Run(() => UpdateConnectionStatus()).Wait();
        }

        private void UpdateConnectionStatus()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ReAppContext>();

            IsConnected = context.Database.CanConnect();
            ConnectionStatusMessage = IsConnected ? "Соединение установлено" : "Соединение не установлено";
        }

        private async Task AddUser()
        {
            var newUser = new User { SecondName = UserSecondName, Name = UserName };
            await _userRepository.AddAsync(newUser);
        }

        private async Task CheckForUpdates()
        {
            try
            {

                string configPath = DirectoryHelper.GetConfigPath(); //Тянется жысон

                Updater.CheckForUpdate(JsonHandler.GetVersionOnServer(configPath), JsonHandler.GetLocalVersion(configPath));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке обновлений: {ex.Message}");
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Func<object, Task> _execute;
        private readonly Predicate<object>? _canExecute;

        public RelayCommand(Func<object, Task> execute, Predicate<object>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter!);
        }

        public async void Execute(object? parameter)
        {
            await _execute(parameter!);
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
