using Serilog;

namespace ReportEngine.App.Config.Logger
{
    public class LoggerConfig
    {
        public static ILogger InitializeLogger()
        {
#if DEBUG
            string logPath = @"C:\Work\Prjs\ReportEngine\ReportEngine.App\logs\log.txt";
#else
            string logPath = DirectoryHelper.GetLogsPath();
#endif
            Log.Logger = new LoggerConfiguration() // Конфигурация Serilog
                .Enrich.FromLogContext()
                .WriteTo.File( //Пишем в файл
                        path: logPath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .MinimumLevel.Debug()
                .WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();

            return Log.Logger;
        }
    }
}
