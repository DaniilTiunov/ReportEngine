using System.ComponentModel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Enums;
using ReportEngine.Domain.Repositories;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.Services.Core;

public class SessionService : INotifyPropertyChanged
{
    private User? _currentUser;
    private readonly AuditService _auditService;
    private readonly IUserRepository _userRepository;

    public SessionService(
        AuditService auditService,
        IUserRepository userRepository)
    {
        _auditService = auditService;
        _userRepository = userRepository;

        FirstStartSession();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

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

    public async void FirstStartSession()
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
        await _auditService.LogEventAsync(
            CurrentUser.UserLogin,
            "Выполнен выход в систему",
            $"Пользователь {CurrentUser.UserLogin} вышёл из систему");

        CurrentUser = null;
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this,
            new PropertyChangedEventArgs(propertyName));
    }
}
