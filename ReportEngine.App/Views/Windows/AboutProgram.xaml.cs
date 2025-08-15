using System.Windows;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;

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
}