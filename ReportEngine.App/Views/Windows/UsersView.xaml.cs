using MahApps.Metro.Controls;
using ReportEngine.App.ViewModels.Contacts;

namespace ReportEngine.App.Views.Windows;

/// <summary>
///     Логика взаимодействия для UsersView.xaml
/// </summary>
public partial class UsersView : MetroWindow
{
    public UsersView(UsersViewModel usersViewModel)
    {
        InitializeComponent();
        DataContext = usersViewModel;
    }
}