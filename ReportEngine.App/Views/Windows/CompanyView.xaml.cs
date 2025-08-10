using ReportEngine.App.ViewModels;
using System.Windows;

namespace ReportEngine.App.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для CompanyView.xaml
    /// </summary>
    public partial class CompanyView : Window
    {
        public CompanyView(CompanyViewModel companyViewModel)
        {
            InitializeComponent();
            DataContext = companyViewModel;

            companyViewModel.OnLoadAllCompaniesExecuted(null);
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
