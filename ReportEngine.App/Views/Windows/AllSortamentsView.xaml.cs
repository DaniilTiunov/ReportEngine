using ReportEngine.App.ViewModels;
using System.Windows;

namespace ReportEngine.App.Views.Windows;

public partial class AllSortamentsView : Window
{
    public AllSortamentsView(AllSortamentsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        Loaded += async (_, _) => await viewModel.LoadDataAsync();
    }
}