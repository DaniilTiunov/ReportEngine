using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ReportEngine.App.ViewModels
{
    /// <summary>
    /// Базовый абстрактный класс ViewModel, реализующий интерфейс INotifyPropertyChanged.
    /// Предоставляет базовую функциональность для уведомления об изменении свойств.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Событие, которое вызывается при изменении свойства.
        /// Используется для уведомления пользовательского интерфейса об изменениях данных.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Вызывает событие PropertyChanged для уведомления об изменении свойства.
        /// </summary>
        /// <param name="propertyName">Имя свойства, которое изменилось. Автоматически определяется компилятором.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Вызываем событие PropertyChanged, передавая имя изменившегося свойства
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Устанавливает значение поля и вызывает событие PropertyChanged, если значение изменилось.
        /// </summary>
        /// <typeparam name="T">Тип поля и значения.</typeparam>
        /// <param name="field">Ссылка на поле, которое нужно установить.</param>
        /// <param name="value">Новое значение для поля.</param>
        /// <param name="propertyName">Имя свойства, которое изменилось. Автоматически определяется компилятором.</param>
        /// <returns>Возвращает true, если значение было изменено; в противном случае — false.</returns>
        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            // Проверяем, равно ли текущее значение поля новому значению
            if (Equals(field, value))
                return false; // Если равно, возвращаем false, так как изменений нет

            // Устанавливаем новое значение поля
            field = value;

            // Вызываем метод OnPropertyChanged для уведомления об изменении свойства
            OnPropertyChanged(propertyName);

            // Возвращаем true, указывая, что значение было изменено
            return true;
        }
    }
}
