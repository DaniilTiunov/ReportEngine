using System.Collections.ObjectModel;
using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.Model.Contacts;

public class UserModel : BaseViewModel
{
    public User CreateNewUser()
    {
        return new User
        {
            SecondName = SelectedUser.SecondName,
            Name = SelectedUser.Name,
            LastName = SelectedUser.LastName,
            Email = SelectedUser.Email,
            Cabinet = SelectedUser.Cabinet,
            Position = SelectedUser.Position,
            PhoneContact = SelectedUser.PhoneContact
        };
    }

    #region Приватные свойства для хранения данных

    private ObservableCollection<User> _allUsers = new();
    private User _selectedUser;

    public ObservableCollection<User> AllUsers
    {
        get => _allUsers;
        set => Set(ref _allUsers, value);
    }

    public User SelectedUser
    {
        get => _selectedUser;
        set => Set(ref _selectedUser, value);
    }

    #endregion
}