using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Commands;
using ReportEngine.App.Model.Contacts;
using ReportEngine.App.Services.Core;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.ViewModels.Contacts;

public class AuthWindowViewModel : BaseViewModel
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly INotificationService _notificationService;
    
    private UserModel _currentUser = new();

    public UserModel CurrentUser
    {
        get => _currentUser;
        set => Set(ref _currentUser, value);
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
    }
    
    public ICommand LoginCommand { get; }

    private void OnLogin(object obj)
    {
        SessionService.CurrentUser = CurrentUser.SelectedUser;
        
        var mainViewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
    
        mainViewModel.OnPropertyChanged(nameof(mainViewModel.CurrentUser));
        mainViewModel.OnPropertyChanged(nameof(mainViewModel.CurrentUserLogin));
        
        _notificationService.ShowInfo("Вход выполнен успешно!");

        ;
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