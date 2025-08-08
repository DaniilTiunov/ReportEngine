using ReportEngine.App.Commands;
using ReportEngine.App.Display;
using ReportEngine.App.Model;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Frame;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;
using System.Collections.ObjectModel;
using System.Windows;
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
                FormedFrameModel.AllFrames = new ObservableCollection<FormedFrame>(await _formedFrameRepository.GetAllAsync());
                FormedFrameModel.FrameDetails = new ObservableCollection<FrameDetail>(await _frameDetailRepository.GetAllAsync());
                FormedFrameModel.FrameRolls = new ObservableCollection<FrameRoll>(await _frameRollRepository.GetAllAsync());
                FormedFrameModel.PillarEqiups = new ObservableCollection<PillarEqiup>(await _pillarEqiupRepository.GetAllAsync());
            });
        }

        public void InitializeCommands()
        {
            AddNewFrameCommand = new RelayCommand(OnAddNewFrameExecuted, CanAllCommandsExecute);
            SaveChangesCommand = new RelayCommand(OnSaveChangesExecuted, CanAllCommandsExecute);
            AddDetailsCommand = new RelayCommand(OnAddDetailsExecuted, CanAllCommandsExecute);
        }

        public ICommand AddNewFrameCommand { get; set; }
        public ICommand SaveChangesCommand { get; set; }
        public ICommand AddDetailsCommand { get; set; }
        public bool CanAllCommandsExecute(object p) => true;
        public async void OnAddNewFrameExecuted(object p)
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var newFrame = FormedFrameModel.CreateNewFrame();

                await _formedFrameRepository.AddAsync(newFrame);

                var addedFrame = await _formedFrameRepository.GetByIdAsync(newFrame.Id);
                FormedFrameModel.AllFrames.Add(addedFrame);

                FormedFrameModel.NewFrame = new FormedFrame();

                FormedFrameModel.SelectedFrame = addedFrame;

                MessageBoxHelper.ShowInfo($"Рама {addedFrame.Name} успешно создана!");
            });
        }
        public async void OnSaveChangesExecuted(object p)
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                if (FormedFrameModel.SelectedFrame != null)
                    await _formedFrameRepository.UpdateAsync(FormedFrameModel.SelectedFrame);

                MessageBoxHelper.ShowInfo("Изменения сохранены");
            });
        }
        public async void OnAddDetailsExecuted(object p)
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var selectedFrame = FormedFrameModel.SelectedFrame;
                var selectedDetail = FormedFrameModel.SelectedFrameDetail;
                if (selectedFrame != null && selectedDetail != null && !selectedFrame.FrameDetails.Contains(selectedDetail))
                {
                    if (!selectedFrame.FrameDetails.Any(d => d.Id == selectedDetail.Id))
                    {
                        selectedFrame.FrameDetails.Add(selectedDetail);
                        await _formedFrameRepository.UpdateAsync(selectedFrame);
                    }
                }
            });
        }
    }
}
