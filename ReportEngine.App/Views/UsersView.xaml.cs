using ReportEngine.App.ViewModels;
using System.Windows;

namespace ReportEngine.App.Views
{
    /// <summary>
    /// Логика взаимодействия для UserTableView.xaml
    /// </summary>
    public partial class UserView : Window
    {
        public UserView(UsersViewModel usersViewModel)
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
