using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ReportEngine.App.Views.Windows.Dialog
{
    /// <summary>
    /// Логика взаимодействия для StandsSettingsView.xaml
    /// </summary>
    public partial class StandsSettingsView : Window
    {
        public StandsSettingsView()
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
