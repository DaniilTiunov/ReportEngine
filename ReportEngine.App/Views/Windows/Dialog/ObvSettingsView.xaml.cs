using System.Windows;
using System.Windows.Input;

namespace ReportEngine.App.Views.Windows.Dialog
{
    /// <summary>
    /// Логика взаимодействия для ObvSettingsView.xaml
    /// </summary>
    public partial class ObvSettingsView : Window
    {
        public ObvSettingsView()
        {
            InitializeComponent();
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
}
