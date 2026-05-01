using Serilog;

namespace ReportEngine.App.Services.Logger;

public class UiLogger
{
    private readonly ILogger _logger;

    public UiLogger()
    {
        _logger = Log.ForContext("UiOnly", true);
    }

    public void Success(string message)
    {
        _logger.ForContext("UiOnly", "Success")
            .Information(message);
    }

    public void Info(string message)
    {
        _logger.ForContext("UiStyle", "Info")
            .Information(message);
    }

    public void Warning(string message)
    {
        _logger.ForContext("UiStyle", "Warning")
            .Warning(message);
    }

    public void Error(string message)
    {
        _logger.ForContext("UiStyle", "Error")
            .Error(message);
    }
}
