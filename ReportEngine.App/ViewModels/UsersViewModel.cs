using ReportEngine.App.Commands;
using ReportEngine.App.Model;
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

        public UserModel CurrentUser { get; set; } = new UserModel();

        #region Конструктор
        public UsersViewModel(IBaseRepository<User> userRepository, NavigationService navigation)
        {
            InitializeCommands();

            _userRepository = userRepository;
            _navigation = navigation;
        }
        #endregion

        #region Методы
        public void InitializeCommands()
        {
            ShowAllUsersCommand = new RelayCommand(OnShowAllUsersCommandExecuted, CanShowAllUsersCommandExecute);
            HideUsersCommand = new RelayCommand(OnHideUsersCommandExecuted, CanHideUsersCommandExecute);
            CloseUsersCommand = new RelayCommand(OnCloseUsersCommandExecuted, CanCloseUsersCommandExecute);
            DeleteUserCommand = new RelayCommand(OnDeleteUserCommandExecuted, CanDeleteUserCommandExecute);
            AddNewUserCommand = new RelayCommand(OnAddNewUserCommandExecuted, CanAddNewUserCommandExecute);
        }
        #endregion
        #region Комманды
        public ICommand HideUsersCommand { get; set; }
        public bool CanHideUsersCommandExecute(object p) => true;
        public void OnHideUsersCommandExecuted(object p) => _navigation.HideWindow();

        public ICommand CloseUsersCommand { get; set; }
        public bool CanCloseUsersCommandExecute(object e) => true;
        public void OnCloseUsersCommandExecuted(object e) => _navigation.CloseWindow();
        
        public ICommand ShowAllUsersCommand { get; set; }
        public bool CanShowAllUsersCommandExecute(object p) => true;
        public async void OnShowAllUsersCommandExecuted(object p)
        {
            try
            {
                var users = await _userRepository.GetAllAsync();

                CurrentUser.AllUsers = new ObservableCollection<User>(users);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при полученни данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }       
        public ICommand DeleteUserCommand { get; set; }
        public bool CanDeleteUserCommandExecute(object e) => true;
        public async void OnDeleteUserCommandExecuted(object e)
        {
            try
            {
                if (CurrentUser.SelectedUser != null)
                {
                    await _userRepository.DeleteAsync(CurrentUser.SelectedUser);
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
                    SecondName = CurrentUser.SelectedUser.SecondName,
                    Name = CurrentUser.SelectedUser.Name,
                    LastName = CurrentUser.SelectedUser.LastName,
                    Email = CurrentUser.SelectedUser.Email,
                    Cabinet = CurrentUser.SelectedUser.Cabinet,
                    Position = CurrentUser.SelectedUser.Position,
                    PhoneContact = CurrentUser.SelectedUser.PhoneContact
                };


                CurrentUser.AllUsers.Add(newUser);               

                await _userRepository.AddAsync(newUser);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при добавлении пользователя", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
