using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ReportEngine.AtomicApp.ViewModels.Abstracts
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Вызываем событие PropertyChanged, передавая имя изменившегося свойства
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
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
