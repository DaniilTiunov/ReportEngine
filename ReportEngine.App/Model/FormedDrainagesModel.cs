using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;
using System.Collections.ObjectModel;

namespace ReportEngine.App.Model
{
    public class FormedDrainagesModel : BaseViewModel
    {
        private ObservableCollection<FormedDrainage> _allFormedDrainage = new();
        private ObservableCollection<FormedDrainage> _allFormedDrainagePurpose = new();


        private FormedDrainage _selectedFormedDrainage = new();
        public ObservableCollection<FormedDrainage> AllFormedDrainage
        {
            get => _allFormedDrainage;
            set => Set(ref _allFormedDrainage, value);
        }
        public FormedDrainage SelectedFormedDrainage
        {
            get => _selectedFormedDrainage;
            set => Set(ref _selectedFormedDrainage, value);
        }

        public FormedDrainage CreateNewFormedDrainages()
        {
            return new FormedDrainage
            {
                Name = SelectedFormedDrainage.Name,
                Material = SelectedFormedDrainage.Material,
                Quantity = SelectedFormedDrainage.Quantity
            };
        }
    }
}
