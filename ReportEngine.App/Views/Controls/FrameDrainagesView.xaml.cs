using ReportEngine.App.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ReportEngine.App.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для FrameDrainagesView.xaml
    /// </summary>
    public partial class FrameDrainagesView : UserControl
    {
        public FrameDrainagesView(ProjectViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void FrameDrainagesView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProjectViewModel viewModel)
            {
                viewModel.LoadDataAsync().GetAwaiter().GetResult();
            }
        }
    }
}
