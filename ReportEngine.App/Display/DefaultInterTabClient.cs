using Dragablz;
using System.Windows;

namespace ReportEngine.App.Display;

public class DefaultInterTabClient : IInterTabClient
{
    public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
    {
        // Создаём новое окно-хост для «оторванной» вкладки
        var hostWindow = new Window
        {
            Title = "Смета КИП",
            Width = 1000,
            Height = 600,
            Owner = Application.Current?.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        // Создаём TabablzControl внутри нового окна
        var tabablz = new TabablzControl
        {
            ShowDefaultCloseButton = true,
        };

        // Создаём InterTabController и привязываем к тому же interTabClient
        var controller = new InterTabController
        {
            InterTabClient = interTabClient
        };

        tabablz.InterTabController = controller;

        hostWindow.Content = tabablz;

        // Возвращаем упаковку хоста
        return new NewHost(hostWindow, tabablz);
    }

    public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
    {
        // Когда TabablzControl пуст — разрешаем Dragablz закрыть окно автоматически.
        // Если нужно закрывать вручную, вернуть DoNothing и закрывать window самостоятельно.
        return TabEmptiedResponse.CloseWindowOrLayoutBranch;
    }

    // Простая реализация INewTabHost<Window>
    private class NewHost : INewTabHost<Window>
    {
        public NewHost(Window container, TabablzControl tabablzControl)
        {
            Container = container;
            TabablzControl = tabablzControl;
        }

        public Window Container { get; }

        public TabablzControl TabablzControl { get; }
    }
}