using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Frame;
using System.Collections.ObjectModel;

namespace ReportEngine.App.Model
{
    public class FormedFrameModel : BaseViewModel
    {
        private ObservableCollection<FormedFrame> _allFrames = new();
        public ObservableCollection<FormedFrame> AllFrames
        {
            get => _allFrames;
            set => Set(ref _allFrames, value);
        }

        private FormedFrame _selectedFrame;
        public FormedFrame SelectedFrame
        {
            get => _selectedFrame;
            set => Set(ref _selectedFrame, value);
        }

        public ObservableCollection<FrameDetail> FrameDetails { get; set; } = new();
        public ObservableCollection<FrameRoll> FrameRolls { get; set; } = new();
        public ObservableCollection<PillarEqiup> PillarEqiups { get; set; } = new();

        public FrameDetail SelectedFrameDetail { get; set; }
        public FrameRoll SelectedFrameRoll { get; set; }
        public PillarEqiup SelectedPillarEqiup { get; set; }
    }
}
