using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Frame;
using System.Collections.ObjectModel;

namespace ReportEngine.App.Model
{
    public class FormedFrameModel : BaseViewModel
    {
        private ObservableCollection<FrameDetail> _frameDetails  = new();
        private ObservableCollection<FrameRoll> _frameRolls  = new();
        private ObservableCollection<PillarEqiup> _pillarEqiups  = new();
        private FormedFrame _selectedFrame;
        private ObservableCollection<FormedFrame> _allFrames = new();
        public ObservableCollection<FormedFrame> AllFrames
        {
            get => _allFrames;
            set => Set(ref _allFrames, value);
        }

        public FormedFrame SelectedFrame
        {
            get => _selectedFrame;
            set => Set(ref _selectedFrame, value);
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

        public FrameDetail SelectedFrameDetail { get; set; }
        public FrameRoll SelectedFrameRoll { get; set; }
        public PillarEqiup SelectedPillarEqiup { get; set; }
    }
}
