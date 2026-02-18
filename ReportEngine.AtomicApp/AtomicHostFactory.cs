using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportEngine.AtomicApp.Views;

namespace ReportEngine.AtomicApp
{
    public static class AtomicHostFactory
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
