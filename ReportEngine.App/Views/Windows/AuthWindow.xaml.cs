using System.Windows;
using ReportEngine.App.ViewModels.Contacts;

namespace ReportEngine.App.Views.Windows;

public partial class AuthWindow : Window
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
    
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}