using Microsoft.Extensions.DependencyInjection;
using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using System.Windows;
using System.Windows.Controls;

namespace ReportEngine.App.Services
{
    /// <summary>
    /// Сервис навигации для управления отображением окон и пользовательских элементов управления.
    /// </summary>
    public class NavigationService
    {
        private readonly IServiceProvider _serviceProvider; // Провайдер сервисов для получения необходимых зависимостей
        private Window? _currentWindow; // Текущее открытое окно
        private ContentControl? _contentHost; // Контейнер для отображения пользовательского контента
        private UserControl? _currentContent; // Текущий отображаемый пользовательский элемент управления

        /// <summary>
        /// Инициализирует новый экземпляр класса NavigationService.
        /// </summary>
        /// <param name="serviceProvider">Провайдер сервисов для получения необходимых зависимостей.</param>
        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Инициализирует контейнер для отображения пользовательского контента.
        /// </summary>
        /// <param name="contentHost">Контейнер для отображения пользовательского контента.</param>
        /// <exception cref="ArgumentNullException">Выбрасывается, если contentHost равен null.</exception>
        public void InitializeContentHost(ContentControl contentHost)
        {
            _contentHost = contentHost ?? throw new ArgumentNullException(nameof(contentHost));
        }

        #region Методы открытия окон

        /// <summary>
        /// Отображает окно указанного типа.
        /// </summary>
        /// <typeparam name="T">Тип окна, который нужно отобразить.</typeparam>
        public void ShowWindow<T>() where T : Window
        {
            _currentWindow = _serviceProvider.GetRequiredService<T>(); // Получаем экземпляр окна из провайдера сервисов
            _currentWindow.Show(); // Отображаем окно
        }

        /// <summary>
        /// Отображает обобщенное окно для указанных типов оборудования.
        /// </summary>
        /// <typeparam name="T">Тип, реализующий интерфейс IBaseEquip.</typeparam>
        /// <typeparam name="TEquip">Тип оборудования, для которого создается окно.</typeparam>
        public void ShowGenericWindow<T, TEquip>()
            where T : IBaseEquip
            where TEquip : class, new()
        {
            var factory = _serviceProvider.GetRequiredService<GenericEquipWindowFactory>(); // Получаем фабрику окон
            _currentWindow = factory.CreateWindow<T, TEquip>(); // Создаем окно с помощью фабрики
            _currentWindow.Show(); // Отображаем окно
        }

        /// <summary>
        /// Скрывает текущее окно.
        /// </summary>
        public void HideWindow()
        {
            _currentWindow?.Hide(); // Скрываем текущее окно, если оно существует
        }

        /// <summary>
        /// Закрывает текущее окно.
        /// </summary>
        public void CloseWindow()
        {
            _currentWindow?.Close(); // Закрываем текущее окно, если оно существует
        }

        #endregion

        #region Методы отображения контента

        /// <summary>
        /// Отображает пользовательский элемент управления указанного типа.
        /// </summary>
        /// <typeparam name="T">Тип пользовательского элемента управления, который нужно отобразить.</typeparam>
        public void ShowContent<T>() where T : UserControl
        {
            if (_contentHost != null)
            {
                // Освобождаем ресурсы текущего контента, если он реализует IDisposable
                if (_currentContent is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                // Получаем новый контент из провайдера сервисов и отображаем его
                var content = _serviceProvider.GetRequiredService<T>();
                _contentHost.Content = content;
                _currentContent = content;
            }
        }

        /// <summary>
        /// Очищает текущий контент.
        /// </summary>
        public void ClearContent<T>() where T : UserControl
        {
            // Освобождаем ресурсы текущего контента, если он реализует IDisposable
            if (_currentContent is IDisposable disposable)
            {
                disposable.Dispose();
            }

            // Очищаем контейнер контента, если он существует
            if (_contentHost != null)
            {
                _contentHost.Content = null;
                _currentContent = null;
            }
        }

        /// <summary>
        /// Закрывает текущий контент.
        /// </summary>
        public void CloseContent()
        {
            // Освобождаем ресурсы текущего контента, если он реализует IDisposable
            if (_currentContent is IDisposable disposable)
            {
                disposable.Dispose();
            }

            // Очищаем контейнер контента, если он существует
            if (_contentHost != null)
            {
                _contentHost.Content = null;
                _currentContent = null;
            }
        }

        #endregion
    }
}
