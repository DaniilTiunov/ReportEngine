using Microsoft.Extensions.DependencyInjection;
using ReportEngine.AtomicApp.Views;

namespace ReportEngine.AtomicApp
{
    static class AromicStartUp
    {
        [STAThread]
        public static void Main()
        {
            var host = AtomicHostFactory.BuildHost();

            var app = host.Services.GetService<App>();
            var mainWindow = host.Services.GetService<MainWindow>();


            mainWindow.Show();
            app.Run();
        }
    }
}
