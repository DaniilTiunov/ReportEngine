using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;
using System.Collections.ObjectModel;

namespace ReportEngine.App.Model
{
    public class UserModel : BaseViewModel
    {
        #region Приватные свойства для хранения данных
        public ObservableCollection<User> _allUsers;

        private string _name;

        private string _secondName;

        private string _lastName;

        private string _email;

        private string _cabinet;

        private string _position;

        private string _phoneContact;

        private User _selectedUser;
        #endregion

        #region Публичные свойства для привязки к контролам
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        public string SecondName
        {
            get => _secondName;
            set => Set(ref _secondName, value);
        }
        public string LastName
        {
            get => _lastName;
            set => Set(ref _lastName, value);
        }
        public string Email
        {
            get => _email;
            set => Set(ref _email, value);
        }
        public string Cabinet
        {
            get => _cabinet;
            set => Set(ref _cabinet, value);
        }
        public string Position
        {
            get => _position;
            set => Set(ref _position, value);
        }
        public string PhoneContact
        {
            get => _phoneContact;
            set => Set(ref _phoneContact, value);
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
        #endregion
    }
}
