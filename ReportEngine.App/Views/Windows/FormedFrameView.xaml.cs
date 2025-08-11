using ReportEngine.App.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace ReportEngine.App.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для FormedFrameView.xaml
    /// </summary>
    public partial class FormedFrameView : Window
    {
        private readonly FormedFrameViewModel _viewModel;
        public FormedFrameView(FormedFrameViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
        }
    }
}
