using ReportEngine.App.Commands;
using ReportEngine.App.Display;
using ReportEngine.App.Model;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Entities.Frame;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;
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
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
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
            RemoveComponentCommand = new RelayCommand(OnRemoveComponentExecuted, CanAllCommandsExecute);
        }
        #region Команды
        public ICommand AddNewFrameCommand { get; set; }
        public ICommand SaveChangesCommand { get; set; }
        public ICommand AddDetailsCommand { get; set; }
        public ICommand DeleteFrameCommand { get; set; }
        public ICommand RemoveComponentCommand { get; set; }
        public bool CanAllCommandsExecute(object p) => true;
        public async void OnAddNewFrameExecuted(object p)
        {
            await ExceptionHelper.SafeExecuteAsync(CreateNewFrameAsync);
        }
        public async void OnSaveChangesExecuted(object p)
        {
            await ExceptionHelper.SafeExecuteAsync(SaveChangesAsync);
        }
        public async void OnDeleteFrameExecuted(object p)
        {
            await ExceptionHelper.SafeExecuteAsync(DeleteFrameAsync);
        }
        public async void OnAddDetailsExecuted(object p)
        {
            await ExceptionHelper.SafeExecuteAsync(AddDetailsToFrame);
        }
        public async void OnRemoveComponentExecuted(object p)
        {
            await ExceptionHelper.SafeExecuteAsync(RemoveDetailsFromFrameAsync);
        }
        #endregion
        #region Методы
        private async Task CreateNewFrameAsync()
        {
            var newFrame = FormedFrameModel.CreateNewFrame();

            await _formedFrameRepository.AddAsync(newFrame);

            var addedFrame = await _formedFrameRepository.GetByIdAsync(newFrame.Id);
            FormedFrameModel.AllFrames.Add(addedFrame);

            FormedFrameModel.NewFrame = new FormedFrame();

            FormedFrameModel.SelectedFrame = addedFrame;

            MessageBoxHelper.ShowInfo($"{addedFrame.Name} успешно создана!");
        }
        private async Task SaveChangesAsync()
        {
            if (FormedFrameModel.SelectedFrame != null)
                await _formedFrameRepository.UpdateAsync(FormedFrameModel.SelectedFrame);

            MessageBoxHelper.ShowInfo("Изменения сохранены");
        }

        private async Task DeleteFrameAsync()
        {
            var selectedFrame = FormedFrameModel.SelectedFrame;
            if (selectedFrame != null)
            {
                await _formedFrameRepository.DeleteAsync(selectedFrame);
                FormedFrameModel.AllFrames.Remove(selectedFrame);


                MessageBoxHelper.ShowInfo($"{selectedFrame.Name} успешно удалена!");
            }
        }
        private async Task AddDetailsToFrame()
        {
            var frame = FormedFrameModel.SelectedFrame;
            var component = FormedFrameModel.SelectedComponentForAdd;
            var length = float.Parse(FormedFrameModel.ComponentLength);

            if (frame == null || component == null) return;

            // Проверяем, нужно ли передавать длину
            var isMeter = (component is BaseFrame baseFrame) && baseFrame.Measure == "м";
            await _formedFrameRepository.AddComponentAsync(frame.Id, component, isMeter ? length : null);

            var updatedFrame = await _formedFrameRepository.GetByIdAsync(frame.Id);
            var idx = FormedFrameModel.AllFrames.IndexOf(FormedFrameModel.AllFrames.FirstOrDefault(f => f.Id == updatedFrame.Id));
            if (idx >= 0)
                FormedFrameModel.AllFrames[idx] = updatedFrame;

            FormedFrameModel.SelectedFrame = updatedFrame;
            FormedFrameModel.UpdateDisplayedComponents();

            // Сбросить длину после добавления
            if (isMeter)
                FormedFrameModel.ComponentLength = null;
        }
        private async Task RemoveDetailsFromFrameAsync()
        {
            var frame = FormedFrameModel.SelectedFrame;
            var frameComponent = FormedFrameModel.SelectedComponentInFrame;
            if (frame == null || frameComponent == null) return;

            await _formedFrameRepository.RemoveComponentAsync(frame.Id, frameComponent.Component);

            var updatedFrame = await _formedFrameRepository.GetByIdAsync(frame.Id);
            var idx = FormedFrameModel.AllFrames.IndexOf(FormedFrameModel.AllFrames.FirstOrDefault(f => f.Id == updatedFrame.Id));
            if (idx >= 0)
                FormedFrameModel.AllFrames[idx] = updatedFrame;

            FormedFrameModel.SelectedFrame = updatedFrame;
            FormedFrameModel.UpdateDisplayedComponents();
        }
        #endregion
    }
}
