using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportEngine.Launcher.Views;

namespace ReportEngine.Launcher
{
    public class HostFactoryLauncher
    {
        public static IHost BuildHost()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<App>();
                    services.AddSingleton<MainWindow>();
                })
                .Build();
        }
    }
}
