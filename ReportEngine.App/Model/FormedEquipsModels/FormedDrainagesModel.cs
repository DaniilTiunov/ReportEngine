using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Drainage;
using System.Collections.ObjectModel;


namespace ReportEngine.App.Model.FormedEquipsModels
{
    public class FormedDrainagesModel : BaseViewModel
    {
        private ObservableCollection<Drainage> _drainageDetails = new();
        private ObservableCollection<FormedDrainage> _allFormedDrainage = new();
        private ObservableCollection<DrainagePurpose> _purposes = new();

        private FormedDrainage _selectedFormedDrainage = new();
        private FormedDrainage _newFormedDrainage = new();
        private DrainagePurpose _selectedPurpose = new();
        private DrainagePurpose _newPurpose = new();
        private Drainage _selectedDrainageDetail = new();

        // Для секции "Добавление нового дренажа"
        public FormedDrainage NewFormedDrainage
        {
            get => _newFormedDrainage;
            set => Set(ref _newFormedDrainage, value);
        }

        // Для секции "Редактирование выбранного дренажа"
        public FormedDrainage SelectedFormedDrainage
        {
            get => _selectedFormedDrainage;
            set
            {
                Set(ref _selectedFormedDrainage, value);
                Purposes = value?.Purposes != null
                    ? new ObservableCollection<DrainagePurpose>(value.Purposes)
                    : new ObservableCollection<DrainagePurpose>();
            }
        }

        public ObservableCollection<FormedDrainage> AllFormedDrainage
        {
            get => _allFormedDrainage;
            set => Set(ref _allFormedDrainage, value);
        }
        public ObservableCollection<Drainage> DrainageDetails
        {
            get => _drainageDetails;
            set => Set(ref _drainageDetails, value);
        }
        public ObservableCollection<DrainagePurpose> Purposes
        {
            get => _purposes;
            set => Set(ref _purposes, value);
        }
        public DrainagePurpose SelectedPurpose
        {
            get => _selectedPurpose;
            set => Set(ref _selectedPurpose, value);
        }
        public DrainagePurpose NewPurpose
        {
            get => _newPurpose;
            set => Set(ref _newPurpose, value);
        }
        public Drainage SelectedDrainageDetail
        {
            get => _selectedDrainageDetail;
            set => Set(ref _selectedDrainageDetail, value);
        }

        public FormedDrainagesModel()
        {
            NewFormedDrainage = new FormedDrainage();
            NewPurpose = new DrainagePurpose();
        }

        public FormedDrainage CreateNewFormedDrainage()
        {
            return new FormedDrainage
            {
                Name = NewFormedDrainage.Name
            };
        }

        public DrainagePurpose CreateNewPurpose()
        {
            return new DrainagePurpose
            {
                Purpose = string.IsNullOrWhiteSpace(NewPurpose.Purpose) ? "Назначение" : NewPurpose.Purpose,
                Material = NewPurpose.Material,
                Quantity = NewPurpose.Quantity
            };
        }
        public void RefreshPurposes()
        {
            if (SelectedFormedDrainage?.Purposes != null)
                Purposes = new ObservableCollection<DrainagePurpose>(SelectedFormedDrainage.Purposes);
            else
                Purposes = new ObservableCollection<DrainagePurpose>();
        }
    }
}
