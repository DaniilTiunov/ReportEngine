using ReportEngine.App.ViewModels;
using System.Collections.ObjectModel;

namespace ReportEngine.App.Model.StandsModel
{
    public class StandFrameDrainageModel : BaseViewModel
    {
        private ObservableCollection<StandFrameModel> _allAvailableFrames = new();
        private ObservableCollection<StandDrainageModel> _allAvailableDrainages = new();
        private StandFrameModel _selectedFrame = new();
        private StandDrainageModel _selectedDrainage = new();

        public ObservableCollection<StandFrameModel> AllAvailableFrames
        {
            get => _allAvailableFrames;
            set => Set(ref _allAvailableFrames, value);
        }
        public StandFrameModel SelectedFrame
        {
            get => _selectedFrame;
            set => Set(ref _selectedFrame, value);
        }
        public ObservableCollection<StandDrainageModel> AllAvailableDrainages
        {
            get => _allAvailableDrainages;
            set => Set(ref _allAvailableDrainages, value);
        }
        public StandDrainageModel SelectedDrainage
        {
            get => _selectedDrainage;
            set => Set(ref _selectedDrainage, value);
        }
    }
}
