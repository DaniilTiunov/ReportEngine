namespace ReportEngine.App.Commands;

/// <summary>
///     Класс RelayCommand, наследующийся от BaseCommand.
///     Предоставляет реализацию команды с использованием делегата для выполнения логики команды.
/// </summary>
public class RelayCommand : BaseCommand
{
    private readonly Func<object, bool> _canExecute; // Делегат для определения возможности выполнения команды
    private readonly Action<object> _execute; // Делегат для выполнения логики команды

    public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
    {
        _canExecute = canExecute; // Устанавливаем делегат для определения возможности выполнения команды
        _execute = execute ??
                   throw new ArgumentNullException(
                       nameof(execute)); // Устанавливаем делегат для выполнения команды и проверяем его на null
    }

    public override bool CanExecute(object? parameter)
    {
        // Если делегат _canExecute не установлен, команда всегда может быть выполнена
        return _canExecute?.Invoke(parameter) ?? true;
    }

    public override void Execute(object? parameter)
    {
        // Выполняем делегат _execute с переданным параметром
        _execute(parameter);
    }
}