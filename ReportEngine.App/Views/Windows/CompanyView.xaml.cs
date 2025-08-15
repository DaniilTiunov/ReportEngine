using System.ComponentModel;
using System.Windows;
using ReportEngine.App.ViewModels.Contacts;

namespace ReportEngine.App.Views.Windows;

/// <summary>
///     Логика взаимодействия для CompanyView.xaml
/// </summary>
public partial class CompanyView : Window
{
    public CompanyView(CompanyViewModel companyViewModel)
    {
        InitializeComponent();
        DataContext = companyViewModel;
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }
}