using ReportEngine.App.AppHelpers;
using ReportEngine.App.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace ReportEngine.App.Views.Windows;

/// <summary>
///     Логика взаимодействия для GenericEquipView.xaml
/// </summary>
public partial class GenericEquipView : Window
{
    public GenericEquipView()
    {
        InitializeComponent();
    }

    private void SelectEquip_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        ExceptionHelper.SafeExecute(() =>
        {
            var type = DataContext.GetType();

            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(GenericEquipViewModel<>))
                return;

            dynamic viewModel = DataContext;
            viewModel.SelectCommand.Execute(null);
        });
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
            MaxRestoreButton_Click(sender, e);
        else
            DragMove();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaxRestoreButton_Click(object sender, RoutedEventArgs e)
    {
        var area = SystemParameters.WorkArea;
        if (Width != area.Width || Height != area.Height || Left != area.Left || Top != area.Top)
        {
            Left = area.Left;
            Top = area.Top;
            Width = area.Width;
            Height = area.Height;
        }
        else
        {
            Width = 1100;
            Height = 450;
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}