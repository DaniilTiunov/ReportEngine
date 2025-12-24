using System.Windows;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Utils
{
    /// <summary>
    /// Логика взаимодействия для RenumerateWindow.xaml
    /// </summary>
    public partial class RenumerateView : Window
    {

        public RenumerateView(RenumeratorViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
