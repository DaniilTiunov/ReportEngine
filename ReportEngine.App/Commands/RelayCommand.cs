namespace ReportEngine.App.Commands;

/// <summary>
///     Класс RelayCommand, наследующийся от BaseCommand.
///     Предоставляет реализацию команды с использованием делегата для выполнения логики команды.
/// </summary>
public class RelayCommand : BaseCommand
{
    private readonly Func<object, bool> _canExecute; // Делегат для определения возможности выполнения команды
    private readonly Action<object> _execute; // Делегат для выполнения логики команды

    /// <summary>
    ///     Инициализирует новый экземпляр класса RelayCommand.
    /// </summary>
    /// <param name="execute">Делегат, который будет выполнен при вызове команды.</param>
    /// <param name="canExecute">
    ///     Делегат, определяющий, может ли команда быть выполнена. Если null, команда всегда может быть
    ///     выполнена.
    /// </param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если делегат execute равен null.</exception>
    public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
    {
        _canExecute = canExecute; // Устанавливаем делегат для определения возможности выполнения команды
        _execute = execute ??
                   throw new ArgumentNullException(
                       nameof(execute)); // Устанавливаем делегат для выполнения команды и проверяем его на null
    }

    /// <summary>
    ///     Определяет, может ли команда быть выполнена.
    /// </summary>
    /// <param name="parameter">Параметр команды, который может использоваться для определения возможности выполнения.</param>
    /// <returns>Возвращает true, если команду можно выполнить; в противном случае — false.</returns>
    public override bool CanExecute(object? parameter)
    {
        // Если делегат _canExecute не установлен, команда всегда может быть выполнена
        return _canExecute?.Invoke(parameter) ?? true;
    }

    /// <summary>
    ///     Выполняет логику команды.
    /// </summary>
    /// <param name="parameter">Параметр команды, который может использоваться при выполнении команды.</param>
    public override void Execute(object? parameter)
    {
        // Выполняем делегат _execute с переданным параметром
        _execute(parameter);
    }
}