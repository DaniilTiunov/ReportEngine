using System.Windows;

namespace ReportEngine.App
{
    public class App : Application
    {
        readonly MainWindow _mainWindow;

        public App(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            _mainWindow.Show();  // отображаем главное окно на экране
            base.OnStartup(e);
        }
    }
}
