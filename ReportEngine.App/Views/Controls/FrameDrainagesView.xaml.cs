using System.Windows.Controls;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Controls;

/// <summary>
///     Логика взаимодействия для FrameDrainagesView.xaml
/// </summary>
public partial class FrameDrainagesView : UserControl
{
    public FrameDrainagesView(ProjectViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        InitializeData(viewModel);
    }

    private async void InitializeData(ProjectViewModel viewModel)
    {
        await viewModel.LoadAllAvaileDataAsync();
        await viewModel.LoadStandsDataAsync();
        await viewModel.LoadPurposesInStandsAsync();
    }
}