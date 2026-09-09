using System.Windows;
using MahApps.Metro.Controls;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Settings.CalculationParameters;

/// <summary>
///     Логика взаимодействия для CalculationParametersWindow.xaml
/// </summary>
public partial class CalculationParametersWindow : MetroWindow
{
    private bool _allowEdit;

    public CalculationParametersWindow(CalculationParametersViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}