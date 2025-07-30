using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using System.Collections.ObjectModel;

namespace ReportEngine.App.Model
{
    public class GenericEquipModel<T> : BaseViewModel where T : IBaseEquip
    {
        private ObservableCollection<T> _baseEquips;
        private T _selectedBaseEquip;
        public ObservableCollection<T> BaseEquips
        {
            get => _baseEquips;
            set => Set(ref _baseEquips, value);
        }
        public T SelectedBaseEquip
        {
            get => _selectedBaseEquip;
            set => Set(ref _selectedBaseEquip, value);
        }
    }
}
