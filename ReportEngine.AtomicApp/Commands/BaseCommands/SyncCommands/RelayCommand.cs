namespace ReportEngine.AtomicApp.Commands.BaseCommands.SyncCommands
{
    public class RelayCommand : BaseCommand
    {
        private readonly Func<object, bool> _canExecute;
        private readonly Action<object> _execute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _canExecute = canExecute;
            _execute = execute ??
                       throw new ArgumentNullException(
                           nameof(execute));
        }

        public override bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public override void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
