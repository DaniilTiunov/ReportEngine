using MahApps.Metro.Controls;
using ReportEngine.App.ViewModels.Contacts;

namespace ReportEngine.App.Views.Windows;

public partial class AuthWindow : MetroWindow
{
    private readonly AuthWindowViewModel _viewModel;

    public AuthWindow(AuthWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        _viewModel = viewModel;

        Loaded += async (_, __) => await InitializeDataAsync(viewModel);
    }

    private async Task InitializeDataAsync(AuthWindowViewModel viewModel)
    {
        await viewModel.LoadAllUsersAsync();
    }
}