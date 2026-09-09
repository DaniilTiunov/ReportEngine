using System.Windows;
using MahApps.Metro.Controls;
using ReportEngine.App.ViewModels.Utils;

namespace ReportEngine.App.Views.Utils;

/// <summary>
///     Логика взаимодействия для RenumerateWindow.xaml
/// </summary>
public partial class RenumerateView : MetroWindow
{
    private readonly RenumeratorViewModel _viewModel;

    public RenumerateView(RenumeratorViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (!_viewModel.ValidateData())
            return;

        _viewModel.OnApplyCommandExecuted(sender);
        Close();
    }
}