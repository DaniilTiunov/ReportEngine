using ReportEngine.App.ViewModels.FormedEquips;
using System.Windows;
using System.Windows.Input;

namespace ReportEngine.App.Views.Windows;

/// <summary>
///     Логика взаимодействия для FormedDrainagesView.xaml
/// </summary>
public partial class FormedDrainagesView : Window
{
    public FormedDrainagesView(FormedDrainagesViewModel formedDrainagesViewModel)
    {
        InitializeComponent();
        DataContext = formedDrainagesViewModel;

        Loaded += FormedDrainagesView_Loaded;
        StateChanged += FormedDrainagesWindow_StateChanges;
    }

    // Событие загрузки окна
    private void FormedDrainagesView_Loaded(object sender, RoutedEventArgs e)
    {
        FormedDrainagesView_StartUpState();
    }

    // Событие изменения состояния окна
    private void FormedDrainagesWindow_StateChanges(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Maximized)
            WindowState = WindowState.Normal;
    }

    // Устанавливает начальное положение и размеры окна по рабочей области
    private void FormedDrainagesView_StartUpState()
    {
        var area = SystemParameters.WorkArea;
        Left = area.Left;
        Top = area.Top;
        Width = area.Width;
        Height = area.Height;
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
            Width = 1280;
            Height = 800;
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}