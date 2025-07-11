using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace ReportEngine.App.Services
{
    public class NavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private Window _currentWindow;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void ShowWindow<T>() where T : Window
        {
            _currentWindow = _serviceProvider.GetRequiredService<T>();
            _currentWindow.Show();
        }
        public void HideWindow()
        {
            _currentWindow?.Hide();
        }
        public void CloseWindow()
        {
            _currentWindow?.Close();
        }
    }
}
