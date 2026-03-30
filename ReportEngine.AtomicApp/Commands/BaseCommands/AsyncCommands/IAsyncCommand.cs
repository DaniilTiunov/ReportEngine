using System.Windows.Input;

namespace ReportEngine.AtomicApp.Commands.BaseCommands.AsyncCommands;

public interface IAsyncCommand : ICommand
{
    Task ExecuteAsync(object parameter);
}
