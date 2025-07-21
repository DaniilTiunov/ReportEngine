using ReportEngine.App.Commands;
using ReportEngine.App.Services;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Config.DebugConsol;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels
{
    public class UsersViewModel : BaseViewModel
    {
        private readonly NavigationService _navigation;
        private readonly IBaseRepository<User> _userRepository;

        #region Приватные свойства для хранения данных
        private ObservableCollection<User> _allUsers;

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

        #region Конструктор
        public UsersViewModel(IBaseRepository<User> userRepository, NavigationService navigation)
        {
            ShowAllUsersCommand = new RelayCommand(OnShowAllUsersCommandExecuted, CanShowAllUsersCommandExecute);
            HideUsersCommand = new RelayCommand(OnHideUsersCommandExecuted, CanHideUsersCommandExecute);
            CloseUsersCommand = new RelayCommand(OnCloseUsersCommandExecuted, CanCloseUsersCommandExecute);
            DeleteUserCommand = new RelayCommand(OnDeleteUserCommandExecuted, CanDeleteUserCommandExecute);
            AddNewUserCommand = new RelayCommand(OnAddNewUserCommandExecuted, CanAddNewUserCommandExecute);

            _userRepository = userRepository;
            _navigation = navigation;
        }
        #endregion

        #region Комманды
        public ICommand HideUsersCommand { get; }
        public bool CanHideUsersCommandExecute(object p) => true;
        public void OnHideUsersCommandExecuted(object p) => _navigation.HideWindow();

        public ICommand CloseUsersCommand { get; }
        public bool CanCloseUsersCommandExecute(object e) => true;
        public void OnCloseUsersCommandExecuted(object e) => _navigation.CloseWindow();
        #endregion

        #region Команда показать всех пользователей
        public ICommand ShowAllUsersCommand { get; set; }
        public bool CanShowAllUsersCommandExecute(object p) => true;
        public async void OnShowAllUsersCommandExecuted(object p)
        {
            try
            {
                var users = await _userRepository.GetAllAsync();

                AllUsers = new ObservableCollection<User>(users);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при полученни данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
        public ICommand DeleteUserCommand { get; }
        public bool CanDeleteUserCommandExecute(object e) => true;
        public async void OnDeleteUserCommandExecuted(object e)
        {
            try
            {
                if (SelectedUser != null)
                {
                    DebugConsole.WriteLine($"Удаляем пользователя {SelectedUser.SecondName} {SelectedUser.Name} {SelectedUser.LastName}");
                    await _userRepository.DeleteAsync(SelectedUser);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при удалении пользователя", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public ICommand AddNewUserCommand { get; set; }
        public bool CanAddNewUserCommandExecute(object e) => true;
        public async void OnAddNewUserCommandExecuted(object p)
        {
            try
            {
                var newUser = new User
                {
                    SecondName = SelectedUser.SecondName,
                    Name = SelectedUser.Name,
                    LastName = SelectedUser.LastName,
                    Email = SelectedUser.Email,
                    Cabinet = SelectedUser.Cabinet,
                    Position = SelectedUser.Position,
                    PhoneContact = SelectedUser.PhoneContact
                };
                

                AllUsers.Add(newUser);               

                await _userRepository.AddAsync(newUser);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при добавлении пользователя", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
