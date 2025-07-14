using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
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
            try
            {
                var _mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                var _currentWindow = _serviceProvider.GetRequiredService<T>();
                _currentWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void HideWindow()
        {
            _currentWindow?.Hide();
        }
        public void CloseWindow()
        {
            _currentWindow?.Hide();
        }
    }
}
