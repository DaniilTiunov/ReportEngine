using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ReportEngine.App.ViewModels
{
    public class UsersViewModel : BaseViewModel
    {
        private readonly IBaseRepository<User> _userRepository;

        #region Приватные свойства для хранения данных
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
        #endregion
        public UsersViewModel(IBaseRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }
    }
}
