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
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }        
    }
}
