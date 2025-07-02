using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportEngine.App.Config.JsonHelpers;
using ReportEngine.Domain.Database.Context;

namespace ReportEngine.App
{
    public class StartUp
    {
        [STAThread]
        public static void Main()
        {
            JsonHandler jsonHandler = new JsonHandler(@"Config\appsettings.json");
            var connString = jsonHandler.GetConnectionString();


            var host = Host.CreateDefaultBuilder().
                ConfigureServices(services =>
                {
                    services.AddDbContext<ReAppContext>(options =>
                    {
                        options.UseNpgsql(connString);
                    });
                    services.AddSingleton<App>();
                    services.AddSingleton<MainWindow>();
                }).Build();

            var app = host.Services.GetService<App>();
            app?.Run();
        }
    }
}
