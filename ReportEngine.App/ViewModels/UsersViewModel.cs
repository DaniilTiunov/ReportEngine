using ReportEngine.App.Commands;
using ReportEngine.App.Services;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
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

        private string _userName;

        private string _userSecondName;

        private string _lastName;

        private string _email;

        private string _cabinet;

        private string _position;

        private string _phoneContact;
        #endregion

        #region Публичные свойства для привязки к контролам
        public string UserName
        {
            get => _userName;
            set => Set(ref _userName, value);
        }
        public string UserSecondName
        {
            get => _userSecondName;
            set => Set(ref _userSecondName, value);
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

            _userRepository = userRepository;
            _navigation = navigation;
        }
        #endregion

        #region Комманды
        public ICommand AddUserCommand { get; set; }

        #region Команда закрыть окно
        public ICommand HideUsersCommand { get; set; }
        public bool CanHideUsersCommandExecute(object p) => true;
        public void OnHideUsersCommandExecuted(object p) => _navigation.HideWindow(); 
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
        #endregion

    }
}
