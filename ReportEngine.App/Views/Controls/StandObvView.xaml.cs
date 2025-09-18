using ReportEngine.App.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ReportEngine.App.Views.Controls;

/// <summary>
///     Логика взаимодействия для StandObvView.xaml
/// </summary>
public partial class StandObvView : UserControl
{
    public StandObvView(ProjectViewModel projectViewModel)
    {
        InitializeComponent();
        DataContext = projectViewModel;
        
        Loaded += async (_, __) => await InitializeDataAsync(projectViewModel);
    }
    
    private async Task InitializeDataAsync(ProjectViewModel viewModel)
    {
        await viewModel.LoadStandsDataAsync();
        await viewModel.LoadObvyazkiAsync();
        await viewModel.LoadAllAvaileDataAsync();
    }
}