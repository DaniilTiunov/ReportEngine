using ReportEngine.App.Commands;
using ReportEngine.App.Model;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Frame;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels
{
    public class FormedFrameViewModel : BaseViewModel
    {
        private readonly IFrameRepository _formedFrameRepository;
        private readonly IGenericBaseRepository<FrameDetail, FrameDetail> _frameDetailRepository;
        private readonly IGenericBaseRepository<FrameRoll, FrameRoll> _frameRollRepository;
        private readonly IGenericBaseRepository<PillarEqiup, PillarEqiup> _pillarEqiupRepository;

        public FormedFrameModel FormedFrameModel { get; } = new();

        public FormedFrameViewModel(
            IFrameRepository formedFrameRepository,
            IGenericBaseRepository<FrameDetail, FrameDetail> frameDetailRepository,
            IGenericBaseRepository<FrameRoll, FrameRoll> frameRollRepository,
            IGenericBaseRepository<PillarEqiup, PillarEqiup> pillarEqiupRepository)
        {
            _frameDetailRepository = frameDetailRepository;
            _frameRollRepository = frameRollRepository;
            _pillarEqiupRepository = pillarEqiupRepository;
            _formedFrameRepository = formedFrameRepository;

            LoadDetailsData();
            InitializeCommands();
        }

        public async void LoadDetailsData()
        {
            await ExceptionHelper.SafeExecuteAsync( async () => {
                FormedFrameModel.FrameDetails = new ObservableCollection<FrameDetail>(await _frameDetailRepository.GetAllAsync());
                FormedFrameModel.FrameRolls = new ObservableCollection<FrameRoll>(await _frameRollRepository.GetAllAsync());
                FormedFrameModel.PillarEqiups = new ObservableCollection<PillarEqiup>(await _pillarEqiupRepository.GetAllAsync());
            });
        }

        public void InitializeCommands() => AddNewFrameCommand = new RelayCommand(OnAddNewFrameExecuted, CanAllCommandsExecute);

        public ICommand AddNewFrameCommand { get; set; }
        public bool CanAllCommandsExecute(object p) => true;
        public async void OnAddNewFrameExecuted(object p)
        {
            await ExceptionHelper.SafeExecuteAsync(async () => 
            {
                var newFrame = new FormedFrame
                {
                    Name = FormedFrameModel.SelectedFrame.Name,
                    Weight = FormedFrameModel.SelectedFrame.Weight,
                    Depth = FormedFrameModel.SelectedFrame.Depth,
                    Width = FormedFrameModel.SelectedFrame.Width,
                    Height = FormedFrameModel.SelectedFrame.Height,
                    Designe = FormedFrameModel.SelectedFrame.Designe
                };

                await _formedFrameRepository.AddAsync(newFrame);

                FormedFrameModel.AllFrames.Add(newFrame);

            });
        }
    }
}
