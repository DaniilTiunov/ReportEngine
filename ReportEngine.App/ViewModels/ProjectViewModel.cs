using ReportEngine.App.Commands;
using ReportEngine.App.Display;
using ReportEngine.App.Model;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.ModelWrappers;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Armautre;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.ElectricSockets;
using ReportEngine.Domain.Entities.Pipes;
using ReportEngine.Domain.Enums;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;
using System.Collections.ObjectModel;

namespace ReportEngine.App.ViewModels
{
    public class ProjectViewModel : BaseViewModel
    {
        private readonly IProjectInfoRepository _projectRepository;
        private readonly IFrameRepository _formedFrameRepository;
        private readonly INotificationService _notificationService;
        private readonly IFormedDrainagesRepository _formedDrainagesRepository;
        private readonly IDialogService _dialogService;
        public StandModel CurrentStandModel { get; set; } = new();
        public ProjectModel CurrentProjectModel { get; set; } = new();
        public ProjectCommandProvider ProjectCommandProvider { get; set; } = new();


        public ProjectViewModel(IProjectInfoRepository projectRepository,
            IDialogService dialogService,
            INotificationService notificationService,
            IFrameRepository formedFrameRepository,
            IFormedDrainagesRepository formedDrainagesRepository)
        {
            _projectRepository = projectRepository;
            _dialogService = dialogService;
            _formedFrameRepository = formedFrameRepository;
            _formedDrainagesRepository = formedDrainagesRepository;
            _notificationService = notificationService;

            InitializeCommands();
            InitializeTime();
            InitializeGenericCommands();
        }

        #region Инициализация
        public void InitializeTime()
        {
            CurrentProjectModel.CreationDate = DateTime.Now.Date;
            CurrentProjectModel.StartDate = DateTime.Now.Date;
            CurrentProjectModel.OutOfProduction = DateTime.Now.Date;
            CurrentProjectModel.EndDate = DateTime.Now.Date;
        }
        public void InitializeCommands()
        {
            ProjectCommandProvider.CreateNewCardCommand = new RelayCommand(OnCreateNewCardCommandExecuted, CanAllCommandsExecute);
            ProjectCommandProvider.AddNewStandCommand = new RelayCommand(OnAddNewStandCommandExecuted, CanAllCommandsExecute);
            ProjectCommandProvider.SaveChangesCommand = new RelayCommand(OnSaveChangesCommandExecuted, CanAllCommandsExecute);

            ProjectCommandProvider.AddFrameToStandCommand = new RelayCommand(OnAddFrameToStandExecuted, CanAllCommandsExecute);
            ProjectCommandProvider.AddDrainageToStandCommand = new RelayCommand(OnAddDrainageToStandExecuted, CanAllCommandsExecute);
            ProjectCommandProvider.AddCustomDrainageToStandCommand = new RelayCommand(OnAddCustomDrainageToStandExecuted, CanAllCommandsExecute);
        }
        public void InitializeGenericCommands()
        {
            ProjectCommandProvider.SelectMaterialLineDialogCommand = new RelayCommand(OnSelectMaterialFromDialogCommandExecuted<HeaterPipe>, CanAllCommandsExecute);
            ProjectCommandProvider.SelectArmatureDialogCommand = new RelayCommand(OnSelectArmatureFromDialogCommandExecuted<HeaterArmature>, CanAllCommandsExecute);
            ProjectCommandProvider.SelectKMCHDialogCommand = new RelayCommand(OnSelectTreeSocketFromDialogCommandExecuted<HeaterSocket>, CanAllCommandsExecute);
            ProjectCommandProvider.SaveObvCommand = new RelayCommand(OnSaveObvCommandExecuted, CanAllCommandsExecute);
        }
        #endregion

        #region Команды
        public bool CanAllCommandsExecute(object e) => true;
        public void OnSelectMaterialFromDialogCommandExecuted<T>(object e) //Выбор материала из диалога
            where T : class, IBaseEquip, new()
        {
            SelectEquipment<T>(name => CurrentProjectModel.SelectedStand.MaterialLine = name);
        }

        public void OnSelectArmatureFromDialogCommandExecuted<T>(object e) //Выбор материала из диалога
            where T : class, IBaseEquip, new()
        {
            SelectEquipment<T>(name => CurrentProjectModel.SelectedStand.Armature = name);
        }
        public void OnSelectTreeSocketFromDialogCommandExecuted<T>(object e) //Выбор материала из диалога
            where T : class, IBaseEquip, new()
        {
            SelectEquipment<T>(name => CurrentProjectModel.SelectedStand.TreeSocket = name);
        }
        public async void OnCreateNewCardCommandExecuted(object e) // Создание новой карточки проекта
        {
            await ExceptionHelper.SafeExecuteAsync(CreateNewProjectCardAsync);
        }
        public async void OnAddNewStandCommandExecuted(object e) // Добавление нового стенда с привязкой к проекту
        {
            await ExceptionHelper.SafeExecuteAsync(AddNewStandToProjectAsync);
        }
        public async void OnSaveChangesCommandExecuted(object e) // Сохранение изменений для карточки проекта
        {
            await ExceptionHelper.SafeExecuteAsync(SaveProjectChangesAsync);
        }

        public async void OnSaveObvCommandExecuted(object e) // Сохранение изменений для обвязки
        {
            await ExceptionHelper.SafeExecuteAsync(AddObvAsync);
        }
        #endregion

        #region Методы
        public void ResetProject() // Сброс проекта для создания нового
        {
            CurrentProjectModel = new ProjectModel();
            CurrentStandModel = new StandModel();
            InitializeTime();
            OnPropertyChanged(nameof(CurrentProjectModel));
            OnPropertyChanged(nameof(CurrentStandModel));
        }
        public void LoadFrames()
        {
            ExceptionHelper.SafeExecute(async () =>
            {
                var frames = await _formedFrameRepository.GetAllAsync();
                var drainages = await _formedDrainagesRepository.GetAllWithPurposesAsync();
                var framesInStand = await _projectRepository.GetAllFramesInStandAsync(CurrentStandModel.Id);
                var drainagesInStand = await _projectRepository.GetAllDrainagesInStandAsync(CurrentStandModel.Id);
                
                CurrentStandModel.FramesInStand = new ObservableCollection<FormedFrame>(framesInStand);
                CurrentStandModel.DrainagesInStand = new ObservableCollection<FormedDrainage>(drainagesInStand);
                CurrentStandModel.AllAvailableFrames = new ObservableCollection<FormedFrame>(frames);
                CurrentStandModel.AllAvailableDrainages = new ObservableCollection<FormedDrainage>(drainages);
            });
        }
        public async Task LoadProjectInfoAsync(int projectId) // Загрузка карточки проекта для редактирования
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var projectInfo = await _projectRepository.GetStandsByIdAsync(projectId);
                if (projectInfo == null)
                    return;

                CurrentProjectModel.CurrentProjectId = projectInfo.Id;
                CurrentProjectModel.Number = projectInfo.Number;
                CurrentProjectModel.Description = projectInfo.Description;
                CurrentProjectModel.CreationDate = projectInfo.CreationDate.ToDateTime(TimeOnly.MinValue);
                CurrentProjectModel.Company = projectInfo.Company;
                CurrentProjectModel.Object = projectInfo.Object;
                CurrentProjectModel.StandCount = projectInfo.StandCount;
                CurrentProjectModel.Cost = projectInfo.Cost;
                CurrentProjectModel.Status = projectInfo.Status.ToString();
                CurrentProjectModel.StartDate = projectInfo.StartDate.ToDateTime(TimeOnly.MinValue);
                CurrentProjectModel.OutOfProduction = projectInfo.OutOfProduction.ToDateTime(TimeOnly.MinValue);
                CurrentProjectModel.EndDate = projectInfo.EndDate.ToDateTime(TimeOnly.MinValue);
                CurrentProjectModel.OrderCustomer = projectInfo.OrderCustomer;
                CurrentProjectModel.RequestProduction = projectInfo.RequestProduction;
                CurrentProjectModel.MarkMinus = projectInfo.MarkMinus;
                CurrentProjectModel.MarkPlus = projectInfo.MarkPlus;
                CurrentProjectModel.IsGalvanized = projectInfo.IsGalvanized;

                CurrentProjectModel.Stands.Clear();
                if (projectInfo.Stands != null)
                {
                    foreach (var stand in projectInfo.Stands)
                    {
                        var standModel = StandDataConverter.ConvertToStandModel(stand);

                        standModel.FramesInStand = new ObservableCollection<FormedFrame>(stand.FormedFrames ?? new List<FormedFrame>());
                        CurrentProjectModel.Stands.Add(standModel);
                    }
                }
                CurrentProjectModel.SelectedStand = CurrentProjectModel.Stands.FirstOrDefault();

                if (CurrentProjectModel.SelectedStand != null)
                    CurrentStandModel = CurrentProjectModel.SelectedStand;
                else
                    CurrentStandModel = new StandModel();

                OnPropertyChanged(nameof(CurrentStandModel));
            });
        }

        private async Task AddObvAsync()
        {
            if (CurrentProjectModel.SelectedStand == null)
                return;

            var newObvyazka = new StandObvyazkaModel
            {
                ObvyazkaId = CurrentProjectModel.SelectedStand.ObvyazkaType,
                ObvyazkaName = "Обвязка" + CurrentProjectModel.SelectedStand.ObvyazkaType,
                MaterialLine = CurrentProjectModel.SelectedStand.MaterialLine,
                Armature = CurrentProjectModel.SelectedStand.Armature,
                TreeSocket = CurrentProjectModel.SelectedStand.TreeSocket,
                KMCH = CurrentProjectModel.SelectedStand.KMCH,
                FirstSensorType = CurrentProjectModel.SelectedStand.FirstSensorType,
                FirstSensorKKS = CurrentProjectModel.SelectedStand.FirstSensorKKSCounter,
                FirstSensorMarkPlus = CurrentProjectModel.SelectedStand.FirstSensorMarkPlus,
                FirstSensorMarkMinus = CurrentProjectModel.SelectedStand.FirstSensorMarkMinus,

            };

            CurrentStandModel.Obvyazki.Add(newObvyazka);

            var entity = new ObvyazkaInStand
            {
                StandId = CurrentProjectModel.SelectedStand.Id,
                ObvyazkaId = newObvyazka.ObvyazkaId,
                MaterialLine = newObvyazka.MaterialLine,
                TreeSocket = newObvyazka.TreeSocket,
                KMCH = newObvyazka.KMCH,
                FirstSensorType = newObvyazka.FirstSensorType,
                FirstSensorKKS = newObvyazka.FirstSensorKKS,
                FirstSensorMarkPlus = newObvyazka.FirstSensorMarkPlus,
                FirstSensorMarkMinus = newObvyazka.FirstSensorMarkMinus,
            };
            await _projectRepository.AddStandObvyazkaAsync(CurrentProjectModel.SelectedStand.Id, entity);

            _notificationService.ShowInfo("Обвязка успешно добавлена!");
        }
        private async Task SaveProjectChangesAsync()
        {
            if (CurrentProjectModel.CurrentProjectId == 0)
            {
                _notificationService.ShowInfo("Сначала создайте проект");
                return;
            }
            var projectInfo = new ProjectInfo
            {
                Id = CurrentProjectModel.CurrentProjectId,
                Number = CurrentProjectModel.Number,
                Description = CurrentProjectModel.Description,
                CreationDate = DateOnly.FromDateTime(CurrentProjectModel.CreationDate),
                Company = CurrentProjectModel.Company,
                Object = CurrentProjectModel.Object,
                StandCount = CurrentProjectModel.StandCount,
                Cost = CurrentProjectModel.Cost,
                HumanCost = CurrentProjectModel.HumanCost,
                Manager = CurrentProjectModel.Manager,
                Status = ComboBoxHelper.ComboBoxChangedValue<ProjectStatus>(CurrentProjectModel.Status),
                StartDate = DateOnly.FromDateTime(CurrentProjectModel.StartDate),
                OutOfProduction = DateOnly.FromDateTime(CurrentProjectModel.OutOfProduction),
                EndDate = DateOnly.FromDateTime(CurrentProjectModel.EndDate),
                OrderCustomer = CurrentProjectModel.OrderCustomer,
                RequestProduction = CurrentProjectModel.RequestProduction,
                MarkMinus = CurrentProjectModel.MarkMinus,
                MarkPlus = CurrentProjectModel.MarkPlus,
                IsGalvanized = CurrentProjectModel.IsGalvanized
            };
            await _projectRepository.UpdateAsync(projectInfo);
            _notificationService.ShowInfo("Изменения успешно сохранены!");
        }
        private async Task AddNewStandToProjectAsync()
        {
            if (CurrentProjectModel.CurrentProjectId == 0)
            {
                _notificationService.ShowInfo("Сначала создайте проект");
                return;
            }
            var newStandModel = new StandModel
            {
                KKSCode = CurrentStandModel.KKSCode,
                Design = CurrentStandModel.Design,
                BraceType = CurrentStandModel.BraceType,
                Devices = CurrentStandModel.Devices,
                Width = CurrentStandModel.Width,
                SerialNumber = CurrentStandModel.SerialNumber,
                Weight = CurrentStandModel.Weight,
                StandSummCost = CurrentStandModel.StandSummCost,
                ObvyazkaType = CurrentStandModel.ObvyazkaType,
                NN = CurrentStandModel.NN,
                MaterialLine = CurrentStandModel.MaterialLine,
                Armature = CurrentStandModel.Armature,
                TreeSocket = CurrentStandModel.TreeSocket,
                KMCH = CurrentStandModel.KMCH,
                FirstSensorType = CurrentStandModel.FirstSensorType,
                ProjectId = CurrentProjectModel.CurrentProjectId
            };
            var newStandEntity = StandDataConverter.ConvertToStandEntity(newStandModel);
            var addedStandEntity = await _projectRepository.AddStandAsync(CurrentProjectModel.CurrentProjectId, newStandEntity);

            newStandModel.Id = addedStandEntity.Id;
            newStandModel.ProjectId = addedStandEntity.ProjectInfoId;

            CurrentProjectModel.Stands.Add(newStandModel);
            CurrentProjectModel.SelectedStand = newStandModel;

            _notificationService.ShowInfo("Стенд успешно добавлен!");
        }
        private async Task CreateNewProjectCardAsync()
        {
            var newProjectCard = CurrentProjectModel.CreateNewProjectCard();

            await _projectRepository.AddAsync(newProjectCard);

            CurrentProjectModel.CurrentProjectId = newProjectCard.Id;
            CurrentProjectModel.Stands.Clear();
            CurrentStandModel = new StandModel();

            _notificationService.ShowInfo($"Новая карточка проекта успешно создана!\nId Проекта: {CurrentProjectModel.CurrentProjectId}"); //Для отладки
        }
        private void SelectEquipment<T>(Action<string> setProperty)
            where T : class, IBaseEquip, new()
        {
            ExceptionHelper.SafeExecute(() =>
            {
                var equipment = _dialogService.ShowEquipDialog<T>();
                if (equipment != null && CurrentProjectModel.SelectedStand != null)
                {
                    setProperty(equipment.Name);
                }
            });
        }

        public async void OnAddFrameToStandExecuted(object p)
        {
            try
            {
                var frame = CurrentStandModel.SelectedFrame;
                if (frame != null)
                {
                    CurrentStandModel.FramesInStand.Add(frame);
                    await _projectRepository.AddFrameToStandAsync(CurrentStandModel.Id, frame);
                    OnPropertyChanged(nameof(CurrentStandModel.FramesInStand));
                    _notificationService.ShowInfo("Рама добавлена");
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowError(ex.Message);
            }
        }

        public async void OnAddDrainageToStandExecuted(object p)
        {
            try
            {
                var drainage = CurrentStandModel.SelectedDrainage;
                if (drainage != null)
                {
                    CurrentStandModel.DrainagesInStand.Add(drainage);
                    await _projectRepository.AddDrainageToStandAsync(CurrentStandModel.Id, drainage);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowError(ex.Message);
            }
        }

        public async void OnAddCustomDrainageToStandExecuted(object p)
        {
            try
            {
                var customDrainage = CurrentStandModel.NewDrainage;
                if (!string.IsNullOrWhiteSpace(customDrainage.Name))
                {
                    CurrentStandModel.AllAvailableDrainages.Add(customDrainage);
                    var entity = new FormedDrainage
                    {
                        Name = customDrainage.Name,
                        Purposes = customDrainage.Purposes.Select(p => new DrainagePurpose
                        {
                            Purpose = p.Purpose,
                            Material = p.Material,
                            Quantity = p.Quantity
                        }).ToList()
                    };
                    await _projectRepository.AddDrainageToStandAsync(CurrentStandModel.Id, entity);
                    MessageBoxHelper.ShowInfo("Пользовательский дренаж успешно добавлен!");
                    await _formedDrainagesRepository.AddAsync(entity);
                    CurrentStandModel.NewDrainage = new FormedDrainage(); // Сбросить
                    
                    OnPropertyChanged(nameof(CurrentStandModel.AllAvailableDrainages));
                    OnPropertyChanged(nameof(CurrentStandModel.DrainagesInStand));
                }
            }
            catch (Exception e)
            {
                _notificationService.ShowError(e.Message);
            }
        }
        #endregion
    }
}
