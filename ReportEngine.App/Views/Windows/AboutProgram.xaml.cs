using System.Windows;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;

namespace ReportEngine.App.Views.Windows;

/// <summary>
///     Логика взаимодействия для PathSettings.xaml
/// </summary>
public partial class AboutProgram : Window
{
    public AboutProgram()
    {
        InitializeComponent(); // Устанавливаем DataContext
        DataContext = this;
    }

    

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
