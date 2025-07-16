using ReportEngine.App.ViewModels;
using System.Windows.Controls;

namespace ReportEngine.App.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для TreeProjectView.xaml
    /// </summary>
    public partial class TreeProjectView : UserControl
    {
        public TreeProjectView(TreeProjectViewModel treeProjectViewModel)
        {
            InitializeComponent();
            DataContext = treeProjectViewModel;
        }
    }
}

