using System.Windows.Controls;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Controls;

/// <summary>
///     Логика взаимодействия для ProjectCardView.xaml
/// </summary>
public partial class ProjectCardView : UserControl
{
    public ProjectCardView(ProjectViewModel projectViewModel)
    {
        InitializeComponent();
        DataContext = projectViewModel;
    }
}