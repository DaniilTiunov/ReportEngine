using System.Collections.ObjectModel;
using System.Windows.Input;
using ReportEngine.App.Commands;
using ReportEngine.App.Extensions;
using ReportEngine.App.Services.Core;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.ViewModels.Contacts;

public class AuthWindowViewModel : BaseViewModel
{
    private readonly INotificationService _notificationService;
    private readonly SessionService _sessionService;
    private readonly IBaseRepository<User> _userRepository;
    private ObservableCollection<User> _allUsers = new();
    private User _selectedUser = new();

    public AuthWindowViewModel(
        IBaseRepository<User> userRepository,
        INotificationService notificationService,
        SessionService sessionService)
    {
        _userRepository = userRepository;
        _notificationService = notificationService;
        _sessionService = sessionService;

        LoginCommand = new RelayCommand(OnLogin, CanLogin);
        ExitCommand = new RelayCommand(LogOut, CanLogin);
    }

    public User SelectedUser
    {
        get => _selectedUser;
        set => Set(ref _selectedUser, value);
    }

    public ObservableCollection<User> AllUsers
    {
        get => _allUsers;
        set => Set(ref _allUsers, value);
    }

    public ICommand LoginCommand { get; }

    public ICommand ExitCommand { get; }

    private bool CanLogin(object obj)
    {
        return true;
    }

    private void OnLogin(object obj)
    {
        if (SelectedUser == null)
            return;

        _sessionService.SignIn(SelectedUser);

        _notificationService.ShowInfo(
            $"Вход выполнен: {SelectedUser.UserLogin}");
    }

    private void LogOut(object obj)
    {
        _sessionService.SignOut();
    }

    public async Task LoadAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();

        AllUsers = users.ToObservable();

        SelectedUser = AllUsers.FirstOrDefault();
    }
}
