using System.Windows;
using System.Windows.Controls;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Controls;

/// <summary>
///     Логика взаимодействия для StandsContainerView.xaml
/// </summary>
public partial class StandsContainerView : UserControl
{
    private readonly ProjectViewModel _projectViewModel;

    public StandsContainerView(ProjectViewModel projectViewModel)
    {
        _projectViewModel = projectViewModel;

        InitializeComponent();
        DataContext = projectViewModel;
    }
}
