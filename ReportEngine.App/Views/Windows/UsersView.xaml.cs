using System.ComponentModel;
using System.Windows;
using ReportEngine.App.ViewModels.Contacts;

namespace ReportEngine.App.Views.Windows;

/// <summary>
///     Логика взаимодействия для UsersView.xaml
/// </summary>
public partial class UsersView : Window
{
    public UsersView(UsersViewModel usersViewModel)
    {
        InitializeComponent();
        DataContext = usersViewModel;
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }
}