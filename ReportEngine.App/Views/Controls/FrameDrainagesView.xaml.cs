using ReportEngine.App.ViewModels;
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
    }
}
