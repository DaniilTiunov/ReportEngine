using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ReportEngine.App.Services.Core;
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
        
        if (SessionService.CurrentUser != null)
        {
            var user = viewModel.CurrentUser.AllUsers
                .FirstOrDefault(u => u.Id == SessionService.CurrentUser.Id);
            if (user != null)
                viewModel.CurrentUser.SelectedUser = user;
        }
    }
    
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    
    private void AdminPasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is AuthWindowViewModel vm)
        {
            vm.InputMegaSecretPassword = ((PasswordBox)sender).Password;
        }
    }
}