namespace ReportEngine.App.AsyncCommands;

public class AsyncRelayCommand : AsyncBaseCommand
{
    private readonly Func<object, bool> _canExecute;
    private readonly Action<object> _execute;
    
    public override bool CanExecute(object parameter)
    {
        throw new NotImplementedException();
    }

    public override Task ExecuteAsync(object parameter)
    {
        throw new NotImplementedException();
    }
}