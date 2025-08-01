﻿using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;
using System.Collections.ObjectModel;

namespace ReportEngine.App.Model
{
    public class UserModel : BaseViewModel
    {
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
}
