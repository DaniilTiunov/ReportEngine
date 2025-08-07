using ReportEngine.App.ViewModels;
using System.Windows;

namespace ReportEngine.App.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для FormedFrameView.xaml
    /// </summary>
    public partial class FormedFrameView : Window
    {
        public FormedFrameView(FormedFrameViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
