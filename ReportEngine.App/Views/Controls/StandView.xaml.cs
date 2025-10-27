using ReportEngine.App.ViewModels;
using System.Windows.Controls;

namespace ReportEngine.App.Views.Controls;

/// <summary>
///     Логика взаимодействия для StandView.xaml
/// </summary>
public partial class StandView : UserControl
{
    public StandView(ProjectViewModel projectViewModel)
    {
        InitializeComponent();
        DataContext = projectViewModel;
    }
}