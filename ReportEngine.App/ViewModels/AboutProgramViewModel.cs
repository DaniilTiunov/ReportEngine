using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;
using System.ComponentModel;

namespace ReportEngine.App.ViewModels
{
    public class AboutProgramViewModel : INotifyPropertyChanged
    {
        private string _version;
        public string Version
        {
            get => _version;
            set
            {
                _version = value;
                OnPropertyChanged(nameof(Version));
            }
        }

        public AboutProgramViewModel()
        {
            Version = JsonHandler.GetCurrentVersion(DirectoryHelper.GetConfigPath());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}