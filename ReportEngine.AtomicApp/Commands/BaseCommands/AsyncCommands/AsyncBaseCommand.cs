using System.Windows.Input;

namespace ReportEngine.AtomicApp.Commands.BaseCommands.AsyncCommands;

public abstract class AsyncBaseCommand : IAsyncCommand
{
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public abstract bool CanExecute(object parameter);

    public abstract Task ExecuteAsync(object parameter);

    public async void Execute(object parameter)
    {
        await ExecuteAsync(parameter);
    }

    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}
