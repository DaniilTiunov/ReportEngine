namespace ReportEngine.AtomicApp.Commands.BaseCommands.AsyncCommands
{
    public class AsyncRelayCommand : AsyncBaseCommand
    {
        private readonly Func<object, bool> _canExecute;
        private readonly Func<object, Task> _executeAsync;

        public AsyncRelayCommand(Func<object, Task> executeAsync, Func<object, bool>? canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            try
            {
                await _executeAsync(parameter);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
