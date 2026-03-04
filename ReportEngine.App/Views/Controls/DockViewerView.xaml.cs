using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace ReportEngine.App.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для DockViewerView.xaml
    /// </summary>
    public partial class DockViewerView : UserControl
    {
        private string _currentFilePath;

        public DockViewerView()
        {
            InitializeComponent();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx"
            };

            if (dialog.ShowDialog() == true)
            {
                _currentFilePath = dialog.FileName;
                OpenFile(_currentFilePath);
            }
        }

        private void OpenFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                MainSreadSheet.Open(stream);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                SaveAs();
            }
            else
            {
                SaveFile(_currentFilePath);
            }
        }

        private void SaveAs()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx"
            };

            if (dialog.ShowDialog() == true)
            {
                _currentFilePath = dialog.FileName;
                SaveFile(_currentFilePath);
            }
        }

        private void SaveFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                MainSreadSheet.SaveAs(stream);
            }
        }
    }
}
