using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Commands;
using ReportEngine.App.Model.Contacts;
using ReportEngine.App.Services.Core;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.Views.Windows;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Enums;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.ViewModels.Contacts;

public class AuthWindowViewModel : BaseViewModel
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly INotificationService _notificationService;
    private readonly string _password = "12345";
    
    private UserModel _currentUser = new();
    private string _inputMegaSecretPassword;
    
    public UserModel CurrentUser
    {
        get => _currentUser;
        set => Set(ref _currentUser, value);
    }

    public string InputMegaSecretPassword
    {
        get => _inputMegaSecretPassword;
        set => Set(ref _inputMegaSecretPassword, value);
    }


    public AuthWindowViewModel(
        IBaseRepository<User> userRepository,
        INotificationService notificationService,
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _userRepository = userRepository;
        _notificationService = notificationService;
        
        LoginCommand = new RelayCommand(OnLogin, CanLogin);
        ExitCommand = new RelayCommand(LogOut, CanLogin);
    }
    
    public ICommand LoginCommand { get; }
    
    public ICommand ExitCommand { get;  }

    private void OnLogin(object obj) //TODO: Доделать пароль
    {
        if (!string.IsNullOrWhiteSpace(InputMegaSecretPassword) && 
            InputMegaSecretPassword == _password)
        {
            SessionService.CurrentUser = CurrentUser.SelectedUser;

            var mainViewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
            var authWindow = _serviceProvider.GetRequiredService<AuthWindow>();
            
            mainViewModel.OnPropertyChanged(nameof(mainViewModel.CurrentUser));
            mainViewModel.OnPropertyChanged(nameof(mainViewModel.CurrentUserLogin));

            _notificationService.ShowInfo("Вход выполнен успешно!");

            authWindow.Close();
        }
        else
        {
            _notificationService.ShowError("Неверный пароль!");
            InputMegaSecretPassword = string.Empty; // сбросим ввод
        }
    }
    
    private void LogOut(object obj)
    {
        SessionService.CurrentUser = new User{ SystemRole = SystemRole.User};
        
        var mainViewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
        var authWindow = _serviceProvider.GetRequiredService<AuthWindow>();
        
        mainViewModel.OnPropertyChanged(nameof(mainViewModel.CurrentUser));
        mainViewModel.OnPropertyChanged(nameof(mainViewModel.CurrentUserLogin));
        
        _notificationService.ShowInfo("Выход из системы!");
        
        authWindow.Close();
    }

    private bool CanLogin(object obj) => CurrentUser.SelectedUser != null;

    public async Task LoadAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();

        CurrentUser.AllUsers.Clear();
        foreach (var user in users)
        {
            CurrentUser.AllUsers.Add(user);
        }
    }
}