using System.Windows.Input;

namespace ReportEngine.App.Commands;

/// <summary>
///     Базовый абстрактный класс, реализующий интерфейс ICommand.
///     Предоставляет базовую функциональность для команд в приложении.
/// </summary>
public abstract class BaseCommand : ICommand
{
    /// <summary>
    ///     Событие, которое вызывается при изменении состояния команды.
    ///     Это событие используется WPF для определения, может ли команда быть выполнена.
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        /// <summary>
        /// Добавляет обработчик события.
        /// </summary>
        add => CommandManager.RequerySuggested += value;

        /// <summary>
        /// Удаляет обработчик события.
        /// </summary>
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>
    ///     Определяет, может ли команда быть выполнена.
    /// </summary>
    /// <param name="parameter">Параметр команды, который может использоваться для определения возможности выполнения.</param>
    /// <returns>Возвращает true, если команду можно выполнить; в противном случае — false.</returns>
    public abstract bool CanExecute(object? parameter);

    /// <summary>
    ///     Выполняет логику команды.
    /// </summary>
    /// <param name="parameter">Параметр команды, который может использоваться при выполнении команды.</param>
    public abstract void Execute(object? parameter);
}