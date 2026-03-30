using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace ReportEngine.AtomicApp.Services;

public class NavigationService
{
    private readonly IServiceProvider _serviceProvider; // Провайдер сервисов для получения необходимых зависимостей
    private ContentControl? _contentHost; // Контейнер для отображения пользовательского контента
    private UserControl? _currentContent; // Текущий отображаемый пользовательский элемент управления
    private Window? _currentWindow; // Текущее открытое окно

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void InitializeContentHost(ContentControl contentHost)
    {
        _contentHost = contentHost ?? throw new ArgumentNullException(nameof(contentHost));
    }

    public void ShowContent<T>()
        where T : UserControl
    {
        if (_contentHost != null)
        {
            // Освобождаем ресурсы текущего контента, если он реализует IDisposable
            if (_currentContent is IDisposable disposable) disposable.Dispose();

            // Получаем новый контент из провайдера сервисов и отображаем его
            var content = _serviceProvider.GetRequiredService<T>();
            _contentHost.Content = content;
            _currentContent = content;
        }
    }
}
