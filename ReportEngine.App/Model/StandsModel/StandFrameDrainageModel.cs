using System.Collections.ObjectModel;
using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.Model.StandsModel
{
    public class StandFrameDrainageModel : BaseViewModel
    {
        // Коллекции для справочника (доступных для выбора)
        private ObservableCollection<FormedFrame> _allAvailableFrames = new();
        private ObservableCollection<StandDrainageModel> _allAvailableDrainages = new();

        // Текущие выбранные элементы из справочника
        private FormedFrame _selectedFrame;
        private StandDrainageModel _selectedDrainage;

        // Коллекции для хранения выбранных (добавленных к стенду)
        private ObservableCollection<StandFrameModel> _frames = new();
        private ObservableCollection<StandDrainageModel> _drainages = new();
         
        // Текущие выбранные элементы из добавленных к стенду
        private StandFrameModel _selectedStandFrame;
        private StandDrainageModel _selectedStandDrainage;

        // Свойства для коллекций доступных элементов (справочник)
        public ObservableCollection<FormedFrame> AllAvailableFrames
        {
            get => _allAvailableFrames;
            set => Set(ref _allAvailableFrames, value);
        }
        public ObservableCollection<StandDrainageModel> AllAvailableDrainages
        {
            get => _allAvailableDrainages;
            set => Set(ref _allAvailableDrainages, value);
        }

        // Свойства для текущих выбранных элементов из справочника
        public FormedFrame SelectedFrame
        {
            get => _selectedFrame;
            set => Set(ref _selectedFrame, value);
        }
        public StandDrainageModel SelectedDrainage
        {
            get => _selectedDrainage;
            set => Set(ref _selectedDrainage, value);
        }

        // Свойства для коллекций элементов, добавленных к стенду
        public ObservableCollection<StandFrameModel> Frames
        {
            get => _frames;
            set => Set(ref _frames, value);
        }
        public ObservableCollection<StandDrainageModel> Drainages
        {
            get => _drainages;
            set => Set(ref _drainages, value);
        }

        // Свойства для текущих выбранных элементов из добавленных к стенду
        public StandFrameModel SelectedStandFrame
        {
            get => _selectedStandFrame;
            set => Set(ref _selectedStandFrame, value);
        }
        public StandDrainageModel SelectedStandDrainage
        {
            get => _selectedStandDrainage;
            set => Set(ref _selectedStandDrainage, value);
        }

        // Свойство для создания нового дренажа вручную
        public StandDrainageModel NewDrainage { get; set; } = new();
    }
}