using ReportEngine.App.Commands;
using ReportEngine.App.Display;
using ReportEngine.App.Model;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
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

        public ICommand AddNewFrameCommand { get; set; }
        public ICommand SaveChangesCommand { get; set; }
        public ICommand AddDetailsCommand { get; set; }
        public ICommand DeleteFrameCommand { get; set; }
        public ICommand RemoveComponentCommand { get; set; }
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

                // Добавляем компонент в базу и коллекцию рамы
                await _formedFrameRepository.AddComponentAsync(selectedFrame.Id, (IBaseEquip)selected);

                // Обновляем словарь количества
                switch (selected)
                {
                    case FrameDetail detail:
                        if (FormedFrameModel.FrameDetailCounts.ContainsKey(detail.Id))
                            FormedFrameModel.FrameDetailCounts[detail.Id]++;
                        else
                            FormedFrameModel.FrameDetailCounts[detail.Id] = 1;
                        break;
                    case FrameRoll roll:
                        if (FormedFrameModel.FrameRollCounts.ContainsKey(roll.Id))
                            FormedFrameModel.FrameRollCounts[roll.Id]++;
                        else
                            FormedFrameModel.FrameRollCounts[roll.Id] = 1;
                        break;
                    case PillarEqiup eqiup:
                        if (FormedFrameModel.PillarEqiupCounts.ContainsKey(eqiup.Id))
                            FormedFrameModel.PillarEqiupCounts[eqiup.Id]++;
                        else
                            FormedFrameModel.PillarEqiupCounts[eqiup.Id] = 1;
                        break;
                    default:
                        return;
                }

                // Перезапрашиваем раму с комплектующими
                FormedFrameModel.SelectedFrame = await _formedFrameRepository.GetByIdAsync(selectedFrame.Id);
                FormedFrameModel.NotifyCountsChanged();
                OnPropertyChanged(nameof(FormedFrameModel.AllSelectedComponents));
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
        public async void OnRemoveComponentExecuted(object p)
        {
            if (p is not IBaseEquip component || FormedFrameModel.SelectedFrame == null)
                return;

            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var frame = await _formedFrameRepository.GetByIdAsync(FormedFrameModel.SelectedFrame.Id);
                if (frame == null) return;

                bool removed = false;
                switch (component)
                {
                    case FrameDetail detail:
                        if (FormedFrameModel.FrameDetailCounts.ContainsKey(detail.Id))
                        {
                            FormedFrameModel.FrameDetailCounts[detail.Id]--;
                            if (FormedFrameModel.FrameDetailCounts[detail.Id] <= 0)
                            {
                                removed = frame.FrameDetails.Remove(frame.FrameDetails.FirstOrDefault(d => d.Id == detail.Id));
                                FormedFrameModel.FrameDetailCounts.Remove(detail.Id);
                            }
                            else
                            {
                                removed = true;
                            }
                        }
                        break;
                    case FrameRoll roll:
                        if (FormedFrameModel.FrameRollCounts.ContainsKey(roll.Id))
                        {
                            FormedFrameModel.FrameRollCounts[roll.Id]--;
                            if (FormedFrameModel.FrameRollCounts[roll.Id] <= 0)
                            {
                                removed = frame.FrameRolls.Remove(frame.FrameRolls.FirstOrDefault(r => r.Id == roll.Id));
                                FormedFrameModel.FrameRollCounts.Remove(roll.Id);
                            }
                            else
                            {
                                removed = true;
                            }
                        }
                        break;
                    case PillarEqiup eqiup:
                        if (FormedFrameModel.PillarEqiupCounts.ContainsKey(eqiup.Id))
                        {
                            FormedFrameModel.PillarEqiupCounts[eqiup.Id]--;
                            if (FormedFrameModel.PillarEqiupCounts[eqiup.Id] <= 0)
                            {
                                removed = frame.PillarEqiups.Remove(frame.PillarEqiups.FirstOrDefault(e => e.Id == eqiup.Id));
                                FormedFrameModel.PillarEqiupCounts.Remove(eqiup.Id);
                            }
                            else
                            {
                                removed = true;
                            }
                        }
                        break;
                    default:
                        return;
                }
                if (removed)
                {
                    await _formedFrameRepository.UpdateAsync(frame);
                    FormedFrameModel.SelectedFrame = await _formedFrameRepository.GetByIdAsync(frame.Id);
                    FormedFrameModel.NotifyCountsChanged();

                }
                else
                {
                    MessageBoxHelper.ShowInfo("Комплектующая не найдена в раме.");
                }
                OnPropertyChanged(nameof(FormedFrameModel.AllSelectedComponents));
            });
        }
    }
}
