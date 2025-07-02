using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportEngine.Domain.Database.Context;

namespace ReportEngine.App
{
    public class StartUp
    {
        [STAThread]
        public static void Main()
        {
            var host = Host.CreateDefaultBuilder().
                ConfigureServices(services =>
                {
                    services.AddDbContext<ReAppContext>();
                    services.AddSingleton<App>();
                    services.AddSingleton<MainWindow>();
                }).Build();


            var app = host.Services.GetService<App>();

            app?.Run();
        }
    }
}
