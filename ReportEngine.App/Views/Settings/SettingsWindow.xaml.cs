using System.Windows;
using System.Windows.Controls;

namespace ReportEngine.App.Views;

/// <summary>
///     Логика взаимодействия для SettingsWindow.xaml
/// </summary>
public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
    }


    private void SettingsCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        //переключалка выбранного окна
        switch (true)
        {
            case true:

            default:
                break;
        }
    }
}