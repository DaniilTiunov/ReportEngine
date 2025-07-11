using System.Windows.Input;

namespace ReportEngine.App.Commands
{
    public abstract class BaseCommand : ICommand // Класс, реализующий интерфейс ICommand
    {
        public event EventHandler? CanExecuteChanged // Событие CanExecuteChanged вызывается при изменении состояния команд
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        public abstract bool CanExecute(object? parameter); 
        public abstract void Execute(object? parameter);
    }
}
