using System.ComponentModel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.Services.Core;

public class SessionService : INotifyPropertyChanged
{
    private readonly AuditService _auditService;
    private readonly IUserRepository _userRepository;
    private User? _currentUser;

    public SessionService(
        AuditService auditService,
        IUserRepository userRepository)
    {
        _auditService = auditService;
        _userRepository = userRepository;

        if (StartUp.CanConnect)
        {
            FirstStartSession();
        }
        else
        {
            CurrentUser = new User
            {
                Id = 0,
                Name = "Гость",
                SecondName = "Гость",
                LastName = "Гость"
            };
        }
    }

    public User? CurrentUser
    {
        get => _currentUser;
        private set
        {
            _currentUser = value;
            OnPropertyChanged(nameof(CurrentUser));
            OnPropertyChanged(nameof(CurrentUser.UserLogin));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private async void FirstStartSession()
    {
        _currentUser = await _userRepository.GetByUserLoginAsync("Гость");
    }

    public async void SignIn(User user)
    {
        CurrentUser = user;
        await _auditService.LogEventAsync(
            CurrentUser.UserLogin,
            "Выполнен вход в систему",
            $"Пользователь {CurrentUser.UserLogin} вошёл в систему");
    }

    public async void SignOut()
    {
        if (CurrentUser == null) { return; }

        await _auditService.LogEventAsync(
            CurrentUser.UserLogin,
            "Выполнен выход из системы",
            $"Пользователь {CurrentUser.UserLogin} вышёл из системы");

        CurrentUser = null;
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this,
            new PropertyChangedEventArgs(propertyName));
    }
}
