using ReportEngine.App.ViewModels;
using System.Windows;

namespace ReportEngine.App.Views
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
        }
    }
}
