using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using System.Collections.ObjectModel;

namespace ReportEngine.App.Model
{
    /// <summary>
    /// Обобщенная модель для управления коллекцией оборудования и выбранным элементом оборудования.
    /// </summary>
    /// <typeparam name="T">Тип, реализующий интерфейс IBaseEquip.</typeparam>
    /// <typeparam name="TEquip">Тип оборудования, который является классом и имеет публичный конструктор без параметров.</typeparam>
    public class GenericEquipModel<T, TEquip> : BaseViewModel
        where T : IBaseEquip // Ограничение: T должен реализовывать интерфейс IBaseEquip
        where TEquip : class, new() // Ограничение: TEquip должен быть классом и иметь публичный конструктор без параметров
    {
        private ObservableCollection<T> _baseEquips; // Коллекция оборудования
        private T _selectedBaseEquip; // Выбранный элемент оборудования

        /// <summary>
        /// Получает или задает коллекцию оборудования.
        /// </summary>
        public ObservableCollection<T> BaseEquips
        {
            get => _baseEquips;
            set => Set(ref _baseEquips, value); // Используем метод Set для установки значения и уведомления об изменении
        }

        /// <summary>
        /// Получает или задает выбранный элемент оборудования.
        /// </summary>
        public T SelectedBaseEquip
        {
            get => _selectedBaseEquip;
            set => Set(ref _selectedBaseEquip, value); // Используем метод Set для установки значения и уведомления об изменении
        }
    }
}
