using System.Windows.Input;
using Microsoft.Win32;
using ReportEngine.App.Commands;

namespace ReportEngine.App.ViewModels
{
    public class DockViewerViewModel : BaseViewModel
    {
        private string _currentFilePath;

        public DockViewerViewModel()
        {
            OpenCommand = new RelayCommand(Open);
            SaveCommand = new RelayCommand(Save);
        }

        public string CurrentFilePath
        {
            get => _currentFilePath;
            set => Set(ref _currentFilePath, value);
        }

        public ICommand OpenCommand { get; }
        public ICommand SaveCommand { get; }

        public event Action<string> OnFileOpenRequested;
        public event Action<string> OnFileSaveRequested;

        private void Open(object obj)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx"
            };

            if (dialog.ShowDialog() == true)
            {
                _currentFilePath = dialog.FileName;
                OnFileOpenRequested?.Invoke(_currentFilePath);
            }
        }

        private void Save(object obj)
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                var dialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx"
                };

                if (dialog.ShowDialog() == true)
                {
                    _currentFilePath = dialog.FileName;
                    OnFileSaveRequested?.Invoke(_currentFilePath);
                }
            }
            else
            {
                OnFileSaveRequested?.Invoke(_currentFilePath);
            }
        }
    }
}
