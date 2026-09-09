using System.Windows.Input;

namespace ReportEngine.App.AsyncCommands;

public interface IAsyncCommand : ICommand
{
    Task ExecuteAsync(object parameter);
}