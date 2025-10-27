using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace ReportEngine.App.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для UpdateInfoView.xaml
    /// </summary>
    public partial class UpdateInfoView : Window
    {
        public List<UpdateInfo> Updates { get; set; }

        public UpdateInfoView()
        {
            InitializeComponent();
            LoadUpdateHistory();
            DataContext = this;
        }

        private void LoadUpdateHistory()
        {
            var filePath = DirectoryHelper.GetUpdateInfoPath();

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);

                var updates = JsonSerializer.Deserialize<List<UpdateInfo>>(json);

                UpdatesList.ItemsSource = updates;
            }
            else
            {
                Updates = new List<UpdateInfo>();
            }
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
                Width = 1000;
                Height = 600;
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
