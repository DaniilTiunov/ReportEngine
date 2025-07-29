using Microsoft.Extensions.DependencyInjection;
using ReportEngine.Domain.Entities.BaseEntities;
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
        public void ShowWindow<T>()
            where T : Window
        {
            _currentWindow = _serviceProvider.GetRequiredService<T>();
            _currentWindow.Show();
        }

        public void ShowGenericWindow<T>()
            where T : BaseEquip, new()
        {
            var factory = _serviceProvider.GetRequiredService<GenericEquipWindowFactory>();
            _currentWindow = factory.CreateWindow<T>();
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
        public void ShowContent<T>()
            where T : UserControl
        {
            if (_contentHost != null)
            {
                if (_currentContent is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                var content = _serviceProvider.GetRequiredService<T>();
                _contentHost.Content = content;
                _currentContent = content;
            }
        }
        public void ClearContent<T>()
            where T : UserControl
        {
            if (_currentContent is IDisposable disposable)
            {
                disposable.Dispose();
            }
            if (_contentHost != null)
            {
                _contentHost.Content = null;
                _currentContent = null;
            }
        }
        public void CloseContent()
        {
            if (_currentContent is IDisposable disposable)
            {
                disposable.Dispose();
            }
            if (_contentHost != null)
            {
                _contentHost.Content = null;
                _currentContent = null;
            }
        }
        #endregion
    }
}
