using ReportEngine.App.ViewModels;
using System.Windows;

namespace ReportEngine.App.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для FormedDrainagesView.xaml
    /// </summary>
    public partial class FormedDrainagesView : Window
    {
        public FormedDrainagesView(FormedDrainagesViewModel formedDrainagesViewModel)
        {
            InitializeComponent();
            DataContext = formedDrainagesViewModel;
        }
    }
}
