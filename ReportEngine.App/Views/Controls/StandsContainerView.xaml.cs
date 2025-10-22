using ReportEngine.App.ViewModels;
using System.Windows;
using System.Windows.Controls;

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

        Loaded += StandsContainerView_Loaded;
    }

    private void StandsContainerView_Loaded(object sender, RoutedEventArgs e)
    {
        _projectViewModel.LoadContainersInfoAsync();
    }
}