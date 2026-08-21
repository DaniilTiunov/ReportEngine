using System.Windows;
using System.Windows.Input;
using ReportEngine.App.Enums;

namespace ReportEngine.App.Views.Windows.Dialog;

/// <summary>
///     Логика взаимодействия для TechCardElecrticDialog.xaml
/// </summary>
public partial class TechCardElecrticDialog : Window
{
    public TechCardElecrticDialog()
    {
        InitializeComponent();
        SelectedOption = TechCardElecticDialogResult.Cancel;
    }

    public TechCardElecticDialogResult SelectedOption { get; private set; }


    private void WithButton_Click(object sender, RoutedEventArgs e)
    {
        SelectedOption = TechCardElecticDialogResult.WithElectric;
        DialogResult = true;

        Close();
    }

    private void WithoutButton_Click(object sender, RoutedEventArgs e)
    {
        SelectedOption = TechCardElecticDialogResult.WithoutElectric;
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        SelectedOption = TechCardElecticDialogResult.Cancel;
        DialogResult = false;
        Close();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        SelectedOption = TechCardElecticDialogResult.Cancel;
        DialogResult = false;
        Close();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}
