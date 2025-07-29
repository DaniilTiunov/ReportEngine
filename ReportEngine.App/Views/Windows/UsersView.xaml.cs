using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;
using System.Windows;
using System.Windows.Controls;

namespace ReportEngine.App.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для UsersView.xaml
    /// </summary>
    public partial class UsersView : Window
    {
        public UsersView(UsersViewModel usersViewModel)
        {
            InitializeComponent();
            DataContext = usersViewModel;

            usersViewModel.OnShowAllUsersCommandExecuted(null);
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
        private void UserDataGrid_CallEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            User user = e.Row.Item as User;
        }
    }
}
