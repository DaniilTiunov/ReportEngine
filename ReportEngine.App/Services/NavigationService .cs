using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportEngine.Shared.Config.DebugConsol;
using System.Windows;
using System.Windows.Controls;

namespace ReportEngine.App.Services
{
    public class NavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private Window? _currentWindow;
        private ContentControl? _contentHost;
        private UserControl? _currentContent;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void InitializeContentHost(ContentControl contentHost)
        {
            _contentHost = contentHost ?? throw new ArgumentNullException(nameof(contentHost));
        }
        #region Методы открытия окон
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
            _currentWindow?.Hide();
        }
        #endregion

        #region Методы отображения контента
        public void ShowContent<T>() where T : UserControl
        {
            if (_contentHost != null)
            {
                _currentContent = _serviceProvider.GetRequiredService<T>();
                _contentHost.Content = _currentContent;
            }
        }

        public void HideContent()
        {
            if (_contentHost != null)
            {
                _contentHost.Content = null;
            }
        }

        public void ClearContent()
        {
            if (_contentHost != null)
            {
                _contentHost.Content = null;
            }
        }
        #endregion
    }
}
