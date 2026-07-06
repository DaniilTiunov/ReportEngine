using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ReportEngine.App.Commands;
using ReportEngine.App.Model.Contacts;
using ReportEngine.App.Services.Notification;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.ViewModels.Contacts;

public class UsersViewModel : BaseViewModel
{
    private readonly ExceptionService _exceptionService;
    private readonly IUserRepository _userRepository;

    public UsersViewModel(
        IUserRepository userRepository,
        ExceptionService exceptionService)
    {
        InitializeCommands();

        _userRepository = userRepository;
        _exceptionService = exceptionService;
        LoadAllUsersAsync();
    }

    public UserModel CurrentUser { get; set; } = new();

    public ICommand ShowAllUsersCommand { get; set; }
    public ICommand AddNewUserCommand { get; set; }
    public ICommand SaveUserCommand { get; set; }

    public ICommand DeleteUserCommand { get; set; }

    public void InitializeCommands()
    {
        ShowAllUsersCommand = new RelayCommand(OnShowAllUsersCommandExecuted, CanAllCommandsExecute);
        DeleteUserCommand = new RelayCommand(OnDeleteUserCommandExecuted, CanAllCommandsExecute);
        AddNewUserCommand = new RelayCommand(OnAddNewUserCommandExecuted, CanAllCommandsExecute);
        SaveUserCommand = new RelayCommand(OnSaveUserCommandExecuted, CanAllCommandsExecute);
    }

    public bool CanAllCommandsExecute(object p)
    {
        return true;
    }

    public async void OnShowAllUsersCommandExecuted(object p)
    {
        await LoadAllUsersAsync();
    }

    public async void OnDeleteUserCommandExecuted(object e)
    {
        await DeleteSelectedUserAsync();
    }

    public async void OnAddNewUserCommandExecuted(object p)
    {
        await AddNewUserAsync();
    }

    public async void OnSaveUserCommandExecuted(object e)
    {
        await SaveUsersChangesAsync();
    }


    private async Task LoadAllUsersAsync()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            var users = await _userRepository.GetAllAsync();
            CurrentUser.AllUsers = new ObservableCollection<User>(users);
        });
    }

    private async Task DeleteSelectedUserAsync()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            if (CurrentUser.SelectedUser != null)
            {
                await _userRepository.DeleteAsync(CurrentUser.SelectedUser);
                CurrentUser.AllUsers.Remove(CurrentUser.SelectedUser);
                CurrentUser.SelectedUser = null;
            }
        });
    }

    private async Task AddNewUserAsync()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            var newUser = CurrentUser.CreateNewUser();
            CurrentUser.AllUsers.Add(newUser);
            await _userRepository.AddAsync(newUser);
        });
    }

    private async Task SaveUsersChangesAsync()
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            if (CurrentUser.SelectedUser != null)
            {
                await _userRepository.UpdateAsync(CurrentUser.SelectedUser);
                MessageBox.Show("Изменения сохранены");
            }
        });
    }
}
