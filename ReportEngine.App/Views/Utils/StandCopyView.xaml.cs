using System.Windows;
using ReportEngine.App.ViewModels.Utils;

namespace ReportEngine.App.Views.Utils;

/// <summary>
///     Логика взаимодействия для StandCopyView.xaml
/// </summary>
public partial class StandCopyView : Window
{
    private readonly StandCopyViewModel _viewModel;

    public StandCopyView(StandCopyViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (!_viewModel.ValidateData())
            return;

        _viewModel.OnApplyCommandExecuted(sender);
        Close();
    }
}
