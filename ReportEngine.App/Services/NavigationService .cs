using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace ReportEngine.App.Services
{
    public class NavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private Window _currentWindow;
        private UserControl _contentHost;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void ShowWindow<T>() where T : Window
        {
            _currentWindow = _serviceProvider.GetRequiredService<T>();
            _currentWindow.Show();
        }
        public void ShowContent<T>() where T : UserControl
        {
            var content = _serviceProvider.GetRequiredService<T>;
            _contentHost.Content = content;
        }
        public void HideWindow()
        {
            _currentWindow.Hide();
        }
        public void CloseWindow()
        {
            _currentWindow?.Hide();
        }
    }
}
