using ReportEngine.App.Commands;
using ReportEngine.App.ModelWrappers;
using ReportEngine.App.Display;
using ReportEngine.App.Model;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Armautre;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.ElectricSockets;
using ReportEngine.Domain.Entities.Pipes;
using ReportEngine.Domain.Enums;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;

namespace ReportEngine.App.ViewModels
{
    public class ProjectViewModel : BaseViewModel
    {
        private readonly IProjectInfoRepository _projectRepository;
        private readonly IDialogService _dialogService;
        public StandModel CurrentStandModel { get; set; } = new();
        public ProjectModel CurrentProjectModel { get; set; } = new();
        public ProjectCommandProvider ProjectCommandProvider { get; set; } = new();

        public ProjectViewModel(IProjectInfoRepository projectRepository, IDialogService dialogService)
        {
            _projectRepository = projectRepository;
            _dialogService = dialogService;

            InitializeCommands();
            InitializeTime();
            InitializeGenericCommands();
        }
        #region Методы
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
        }
        public void InitializeGenericCommands()
        {
            ProjectCommandProvider.SelectMaterialLineDialogCommand = new RelayCommand(OnSelectMaterialFromDialogCommandExecuted<HeaterPipe>, CanAllCommandsExecute);
            ProjectCommandProvider.SelectArmatureDialogCommand = new RelayCommand(OnSelectArmatureFromDialogCommandExecuted<HeaterArmature>, CanAllCommandsExecute);
            ProjectCommandProvider.SelectKMCHDialogCommand = new RelayCommand(OnSelectTreeSocketFromDialogCommandExecuted<HeaterSocket>, CanAllCommandsExecute);
            ProjectCommandProvider.SaveObvCommand = new RelayCommand(OnSaveObvCommandExecuted, CanAllCommandsExecute);
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
                        CurrentProjectModel.Stands.Add(StandDataConverter.ConvertToStandModel(stand));
                }
                CurrentProjectModel.SelectedStand = CurrentProjectModel.Stands.FirstOrDefault();

                CurrentStandModel = new StandModel();
            });
        }
        #endregion
        #region Команды
        public bool CanAllCommandsExecute(object e) => true;
        public void OnSelectMaterialFromDialogCommandExecuted<T>(object e) //Выбор материала из диалога
            where T : class, IBaseEquip, new()
        {
            SelectEquipment<T>(name =>  CurrentProjectModel.SelectedStand.MaterialLine = name);
        }

        public void OnSelectArmatureFromDialogCommandExecuted<T>(object e) //Выбор материала из диалога
            where T : class, IBaseEquip, new()
        {
            SelectEquipment<T>(name =>  CurrentProjectModel.SelectedStand.Armature = name);
        }
        public void OnSelectTreeSocketFromDialogCommandExecuted<T>(object e) //Выбор материала из диалога
            where T : class, IBaseEquip, new()
        {
            SelectEquipment<T>(name =>  CurrentProjectModel.SelectedStand.TreeSocket = name);
        }
        public async void OnCreateNewCardCommandExecuted(object e) // Создание новой карточки проекта
        {
            await ExceptionHelper.SafeExecuteAsync(CreateNewProjectCardAsync);
        }
        public async void OnAddNewStandCommandExecuted(object e) // Добавление нового стенда с привязкой к проекту
        {
            await ExceptionHelper.SafeExecuteAsync(AddNewStandToProjectAsync);
        }
        public async void OnSaveChangesCommandExecuted(object e) // Сохранение изменений для карточки преокта
        {
            await ExceptionHelper.SafeExecuteAsync(SaveProjectChangesAsync);
        }

        public async void OnSaveObvCommandExecuted(object e) // Сохранение изменений для обвязки
        {
            await ExceptionHelper.SafeExecuteAsync(AddObvAsync);
        }

        public void ResetProject() // Сброс проекта для создания нового
        {
            CurrentProjectModel = new ProjectModel();
            CurrentStandModel = new StandModel();
            InitializeTime();
            OnPropertyChanged(nameof(CurrentProjectModel));
            OnPropertyChanged(nameof(CurrentStandModel));
        }

        private async Task AddObvAsync()
        {
            if (CurrentProjectModel.SelectedStand == null)
                return;



            MessageBoxHelper.ShowInfo("Изменения обвязки успешно сохранены!");
        }
        private async Task SaveProjectChangesAsync()
        {
            if (CurrentProjectModel.CurrentProjectId == 0)
            {
                MessageBoxHelper.ShowInfo("Сначала создайте проект");
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
            MessageBoxHelper.ShowInfo("Изменения успешно сохранены!");
        }
        private async Task AddNewStandToProjectAsync()
        {
            if (CurrentProjectModel.CurrentProjectId == 0)
            {
                MessageBoxHelper.ShowInfo("Сначала создайте проект");
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

            MessageBoxHelper.ShowInfo("Стенд успешно добавлен!");
        }
        private async Task CreateNewProjectCardAsync()
        {
            var newProjectCard = CurrentProjectModel.CreateNewProjectCard();

            await _projectRepository.AddAsync(newProjectCard);

            CurrentProjectModel.CurrentProjectId = newProjectCard.Id;
            CurrentProjectModel.Stands.Clear();
            CurrentStandModel = new StandModel();

            MessageBoxHelper.ShowInfo($"Новая карточка проекта успешно создана!\nId Преокта: {CurrentProjectModel.CurrentProjectId}"); //Для отладки
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
        #endregion        
    }
}
