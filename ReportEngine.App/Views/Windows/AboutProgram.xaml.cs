using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;
using System.Windows;

namespace ReportEngine.App.Views.Windows;

/// <summary>
///     Логика взаимодействия для AboutProgram.xaml
/// </summary>
public partial class AboutProgram : Window
{
    public AboutProgram()
    {
        InitializeComponent(); // Устанавливаем DataContext
        DataContext = this;
    }

    public string Version => JsonHandler.GetCurrentVersion(DirectoryHelper.GetConfigPath());

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}