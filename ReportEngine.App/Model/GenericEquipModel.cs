using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities.BaseEntities;
using System.Collections.ObjectModel;

namespace ReportEngine.App.Model
{
    public class GenericEquipModel : BaseViewModel
    {
        private ObservableCollection<BaseEquip> _baseEquips;
        private BaseEquip _selectedBaseEquip;

        public ObservableCollection<BaseEquip> baseEquips
        {
            get => _baseEquips;
            set => Set(ref _baseEquips, value);
        }
        public BaseEquip SelectedBaseEquip
        {
            get => _selectedBaseEquip;
            set => Set(ref _selectedBaseEquip, value);
        }
    }
}
