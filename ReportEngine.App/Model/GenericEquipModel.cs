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
        public ObservableCollection<T> BaseEquips { get; set; }
        public T SelectedBaseEquip { get; set; }
    }
}