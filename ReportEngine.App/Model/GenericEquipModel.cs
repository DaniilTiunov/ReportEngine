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
    where T : class, IBaseEquip, new()
    {
        private ObservableCollection<T> _baseEquips = new();
        public ObservableCollection<T> BaseEquips
        {
            get => _baseEquips;
            set => Set(ref _baseEquips, value);
        }

        private T _selectedBaseEquip;
        public T SelectedBaseEquip
        {
            get => _selectedBaseEquip;
            set => Set(ref _selectedBaseEquip, value);
        }
    }
}