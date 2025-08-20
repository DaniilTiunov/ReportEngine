using System.Windows;
using System.Windows.Controls;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Controls;

public partial class ProjectPreview : UserControl
{
    public ProjectPreview(ProjectViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        
        InitializeData(viewModel);
    }
    private async void InitializeData(ProjectViewModel viewModel)
    {
        await viewModel.LoadObvyazkiAsync();
        await viewModel.LoadStandsDataAsync();
        await viewModel.LoadPurposesInStandsAsync();
    }
}