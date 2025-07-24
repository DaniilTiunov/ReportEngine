using ReportEngine.App.ViewModels;
using System.Windows.Controls;

namespace ReportEngine.App.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для ProjectCardView.xaml
    /// </summary>
    public partial class ProjectCardView : UserControl
    {
        public ProjectCardView(ProjectViewModel projectViewModel)
        {
            InitializeComponent();
            DataContext = projectViewModel;
        }
    }
}
