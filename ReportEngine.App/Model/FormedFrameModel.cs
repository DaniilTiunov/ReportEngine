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
        private ObservableCollection<FormedFrame> _allFrames = new();
        private FormedFrame _selectedFrame = new();
        private FrameDetail _selectedFrameDetail = new();
        private FormedFrame _newFrame = new();

        public string FrameDetailsNames =>
        FrameDetails != null && FrameDetails.Any()
            ? string.Join(", ", FrameDetails.Select(d => d.Name))
            : "Нет деталей";

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

        public FrameDetail SelectedFrameDetail 
        { 
            get => _selectedFrameDetail; 
            set => Set(ref _selectedFrameDetail, value); 
        }
        public FrameRoll SelectedFrameRoll { get; set; }
        public PillarEqiup SelectedPillarEqiup { get; set; }

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
    }
}
