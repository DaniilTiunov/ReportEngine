using System.Windows.Input;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ReportEngine.App.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly IBaseRepository<User> _userRepository;

        private string _userId;
        private string _userName;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string UserId
        {
            get => _userId;
            set
            {
                _userId = value;
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

        public MainWindowViewModel(IBaseRepository<User> userRepository)
        {
            _userRepository = userRepository;
            AddUserCommand = new RelayCommand(async _ => await AddUser());
        }

        private async Task AddUser()
        {
            if (int.TryParse(UserId, out int id))
            {
                var newUser = new User { Id = id, Name = UserName };
                await _userRepository.AddAsync(newUser);
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
