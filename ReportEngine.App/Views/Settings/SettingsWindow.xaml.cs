using MahApps.Metro.Controls;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views;

/// <summary>
///     Логика взаимодействия для SettingsWindow.xaml
/// </summary>
public partial class SettingsWindow : MetroWindow
{
    private readonly SettingsViewModel _viewModel;

    public SettingsWindow(SettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;

        Loaded += (s, e) => _viewModel.LoadSettings();
    }
}