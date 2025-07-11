using ReportEngine.App.ViewModels;
using System.Windows;

namespace ReportEngine.App.Views
{
    /// <summary>
    /// Логика взаимодействия для UserTableView.xaml
    /// </summary>
    public partial class UserTableView : Window
    {
        public UserTableView(UsersViewModel usersViewModel)
        {
            InitializeComponent();
            DataContext = usersViewModel;
        }
        private void InitializeComponent()
        {
            // Initialize UI components here
        }
    }
}
