using ReportEngine.App.Commands;
using ReportEngine.App.Display;
using ReportEngine.App.Model;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.Frame;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
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
                var frames = await _formedFrameRepository.GetAllAsync();
                var details = await _frameDetailRepository.GetAllAsync();
                var rolls = await _frameRollRepository.GetAllAsync();
                var eqiups = await _pillarEqiupRepository.GetAllAsync();

                FormedFrameModel.AllFrames.Clear();
                foreach (var f in frames) FormedFrameModel.AllFrames.Add(f);

                FormedFrameModel.FrameDetails.Clear();
                foreach (var d in details) FormedFrameModel.FrameDetails.Add(d);

                FormedFrameModel.FrameRolls.Clear();
                foreach (var r in rolls) FormedFrameModel.FrameRolls.Add(r);

                FormedFrameModel.PillarEqiups.Clear();
                foreach (var p in eqiups) FormedFrameModel.PillarEqiups.Add(p);
            });
        }

        public void InitializeCommands()
        {
            AddNewFrameCommand = new RelayCommand(OnAddNewFrameExecuted, CanAllCommandsExecute);
            SaveChangesCommand = new RelayCommand(OnSaveChangesExecuted, CanAllCommandsExecute);
            AddDetailsCommand = new RelayCommand(OnAddDetailsExecuted, CanAllCommandsExecute);
            DeleteFrameCommand = new RelayCommand(OnDeleteFrameExecuted, CanAllCommandsExecute);
        }

        public ICommand AddNewFrameCommand { get; set; }
        public ICommand SaveChangesCommand { get; set; }
        public ICommand AddDetailsCommand { get; set; }
        public ICommand DeleteFrameCommand { get; set; }
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

                MessageBoxHelper.ShowInfo($"{addedFrame.Name} успешно создана!");
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
                if (selectedFrame == null) return;

                object selected = p switch
                {
                    "FrameDetail" => FormedFrameModel.SelectedFrameDetail,
                    "FrameRoll" => FormedFrameModel.SelectedFrameRoll,
                    "PillarEqiup" => FormedFrameModel.SelectedPillarEqiup,
                    _ => null
                };

                if (selected is null) return;

                var freshFrame = await _formedFrameRepository.GetByIdAsync(selectedFrame.Id);

                bool alreadyExists = selected switch
                {
                    FrameDetail detail => freshFrame.FrameDetails.Any(d => d.Id == detail.Id),
                    FrameRoll roll => freshFrame.FrameRolls.Any(r => r.Id == roll.Id),
                    PillarEqiup eqiup => freshFrame.PillarEqiups.Any(e => e.Id == eqiup.Id),
                    _ => false
                };

                if (!alreadyExists)
                {
                    await _formedFrameRepository.AddComponentAsync(freshFrame.Id, (IBaseEquip)selected);
                    FormedFrameModel.SelectedFrame = await _formedFrameRepository.GetByIdAsync(freshFrame.Id);
                }
                else
                {
                    MessageBoxHelper.ShowInfo("Комплектующая уже добавлена к раме.");
                }
            });
        }
        public async void OnDeleteFrameExecuted(object p)
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var selectedFrame = FormedFrameModel.SelectedFrame;
                if (selectedFrame != null)
                {
                    await _formedFrameRepository.DeleteAsync(selectedFrame);
                    FormedFrameModel.AllFrames.Remove(selectedFrame);


                    MessageBoxHelper.ShowInfo($"{selectedFrame.Name} успешно удалена!");
                }
            });
        }
    }
}
