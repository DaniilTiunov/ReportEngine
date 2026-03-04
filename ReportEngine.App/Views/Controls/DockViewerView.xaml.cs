using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using ReportEngine.App.ViewModels;
using ReportEngine.App.Views.Windows.Dialog;

namespace ReportEngine.App.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для DockViewerView.xaml
    /// </summary>
    public partial class DockViewerView : UserControl
    {
        private DockViewerViewModel _viewModel;

        public DockViewerView(DockViewerViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;

            DataContext = _viewModel;

            _viewModel.OnFileOpenRequested += OpenFile;
            _viewModel.OnFileSaveRequested += SaveFile;
        }

        private void OpenFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                MainSreadSheet.Open(stream);
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
