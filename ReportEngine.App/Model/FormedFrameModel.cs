using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.Frame;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace ReportEngine.App.Model
{
    public class ComponentWithCount
    {
        public IBaseEquip Component { get; set; }
        public int Count { get; set; }
    }

    public class FormedFrameModel : BaseViewModel
    {
        private ObservableCollection<FrameDetail> _frameDetails = new();
        private ObservableCollection<FrameRoll> _frameRolls = new();
        private ObservableCollection<PillarEqiup> _pillarEqiups = new();
        private ObservableCollection<FormedFrame> _allFrames = new();
        private ObservableCollection<IBaseEquip> _allComponents = new();

        private FormedFrame _selectedFrame = new();
        private FrameDetail _selectedFrameDetail = new();
        private FrameRoll _selectedFrameRoll = new();
        private PillarEqiup _selectedPillarEqiup = new();
        private IBaseEquip _selectedEquip;
        private FormedFrame _newFrame = new();

        
        public ObservableCollection<FormedFrame> AllFrames
        {
            get => _allFrames;
            set => Set(ref _allFrames, value);
        }

        public FormedFrame NewFrame
        {
            get => _newFrame;
            set => Set(ref _newFrame, value);
        }

        public FormedFrame SelectedFrame
        {
            get => _selectedFrame;
            set
            {
                Set(ref _selectedFrame, value);

            }
        }

        public ObservableCollection<FrameDetail> FrameDetails
        {
            get => _frameDetails;
            set => Set(ref _frameDetails, value);
        }

        public ObservableCollection<FrameRoll> FrameRolls
        {
            get => _frameRolls;
            set => Set(ref _frameRolls, value);
        }
        public ObservableCollection<PillarEqiup> PillarEqiups
        {
            get => _pillarEqiups;
            set => Set(ref _pillarEqiups, value);
        }
        public ObservableCollection<IBaseEquip> AllComponents
        {
            get => _allComponents;
            set => Set(ref _allComponents, value);
        }
        public FrameDetail SelectedFrameDetail
        {
            get => _selectedFrameDetail;
            set => Set(ref _selectedFrameDetail, value);
        }
        public FrameRoll SelectedFrameRoll
        {
            get => _selectedFrameRoll;
            set => Set(ref _selectedFrameRoll, value);
        }
        public PillarEqiup SelectedPillarEqiup
        {
            get => _selectedPillarEqiup;
            set => Set(ref _selectedPillarEqiup, value);
        }
        public IBaseEquip SelectedComponent
        {
            get => _selectedEquip;
            set => Set(ref _selectedEquip, value);
        }
        public FormedFrameModel()
        {
            SelectedFrame = new FormedFrame();
        }
        public FormedFrame CreateNewFrame()
        {

            return new FormedFrame
            {
                Name = NewFrame.Name,
                Weight = NewFrame.Weight,
                FrameType = NewFrame.FrameType,
                Depth = NewFrame.Depth,
                Width = NewFrame.Width,
                Height = NewFrame.Height,
                Designe = NewFrame.Designe,
            };
        }

        // Словари для хранения количества каждой комплектующей по Id
        public Dictionary<int, int> FrameDetailCounts { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> FrameRollCounts { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> PillarEqiupCounts { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> ComponentCounts { get; set; } = new Dictionary<int, int>(); // Общий словарь для всех типов

        public void NotifyCountsChanged()
        {
            OnPropertyChanged(nameof(FrameDetailCounts));
            OnPropertyChanged(nameof(FrameRollCounts));
            OnPropertyChanged(nameof(PillarEqiupCounts));

        }
    }
}
