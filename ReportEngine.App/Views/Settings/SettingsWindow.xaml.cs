using System.Windows;
using System.Windows.Controls;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views;

/// <summary>
///     Логика взаимодействия для SettingsWindow.xaml
/// </summary>
public partial class SettingsWindow : Window
{
    public SettingsWindow(SettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        
        Loaded += (_, __) =>
        {
            InitializeSettings(viewModel);
        };
    }
    
    private void InitializeSettings(SettingsViewModel viewModel)
    {
        viewModel.LoadSettings();
        ShowPanel("GeneralSettings");
    }
    
    private void SettingsCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = SettingsCategories.SelectedItem as ListBoxItem;
        var tag = item?.Tag?.ToString() ?? "GeneralSettings";
        
        ShowPanel(tag);
    }
    private void ShowPanel(string tag)
    {
        ExceptionHelper.SafeExecute(() =>
        {
            if (GeneralSettingsPanel == null || ConnectionSettingsPanel == null)
                return;
            
            GeneralSettingsPanel.Visibility = Visibility.Collapsed;
            ConnectionSettingsPanel.Visibility = Visibility.Collapsed;
            DesignSettingsPanel.Visibility = Visibility.Collapsed;
            OtherSettingsPanel.Visibility = Visibility.Collapsed;
        
            switch (tag)
            {
                case "GeneralSettings":
                case "Основные":
                    GeneralSettingsPanel.Visibility = Visibility.Visible;
                    SettingsTitle.Text = "Основные настройки";
                    break;

                case "ConnectionSettings":
                case "Подключение":
                    ConnectionSettingsPanel.Visibility = Visibility.Visible;
                    SettingsTitle.Text = "Настройки подключения";
                    break;

                case "DesignSettings":
                    DesignSettingsPanel.Visibility = Visibility.Visible;
                    SettingsTitle.Text = "Оформление";
                    break;

                case "OtherSettings":
                    OtherSettingsPanel.Visibility = Visibility.Visible;
                    SettingsTitle.Text = "Другое";
                    break;

                default:
                    GeneralSettingsPanel.Visibility = Visibility.Visible;
                    SettingsTitle.Text = "Настройки";
                    break;
            }
        });
    }
}