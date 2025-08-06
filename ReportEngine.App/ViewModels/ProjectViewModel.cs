using ReportEngine.App.Commands;
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
        public StandModel CurrentStand { get; set; } = new();
        public ProjectModel CurrentProject { get; set; } = new();
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
            CurrentProject.CreationDate = DateTime.Now.Date;
            CurrentProject.StartDate = DateTime.Now.Date;
            CurrentProject.OutOfProduction = DateTime.Now.Date;
            CurrentProject.EndDate = DateTime.Now.Date;
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

                CurrentProject.CurrentProjectId = projectInfo.Id;
                CurrentProject.Number = projectInfo.Number;
                CurrentProject.Description = projectInfo.Description;
                CurrentProject.CreationDate = projectInfo.CreationDate.ToDateTime(TimeOnly.MinValue);
                CurrentProject.Company = projectInfo.Company;
                CurrentProject.Object = projectInfo.Object;
                CurrentProject.StandCount = projectInfo.StandCount;
                CurrentProject.Cost = projectInfo.Cost;
                CurrentProject.Status = projectInfo.Status.ToString();
                CurrentProject.StartDate = projectInfo.StartDate.ToDateTime(TimeOnly.MinValue);
                CurrentProject.OutOfProduction = projectInfo.OutOfProduction.ToDateTime(TimeOnly.MinValue);
                CurrentProject.EndDate = projectInfo.EndDate.ToDateTime(TimeOnly.MinValue);
                CurrentProject.OrderCustomer = projectInfo.OrderCustomer;
                CurrentProject.RequestProduction = projectInfo.RequestProduction;
                CurrentProject.MarkMinus = projectInfo.MarkMinus;
                CurrentProject.MarkPlus = projectInfo.MarkPlus;
                CurrentProject.IsGalvanized = projectInfo.IsGalvanized;

                CurrentProject.Stands.Clear();
                if (projectInfo.Stands != null)
                {
                    foreach (var stand in projectInfo.Stands)
                        CurrentProject.Stands.Add(stand);
                }

                CurrentStand = new StandModel();
                OnPropertyChanged(nameof(CurrentStand));
                OnPropertyChanged(nameof(CurrentProject));
            });
        }
        #endregion
        #region Команды
        public void OnSelectMaterialFromDialogCommandExecuted<T>(object e)
            where T : class, IBaseEquip, new()
        {
            ExceptionHelper.SafeExecute(() =>
            {
                var materialLine = _dialogService.ShowEquipDialog<T>();
                if (materialLine != null)
                    CurrentProject.SelectedStand.MaterialLine = materialLine.Name;
            });
        }

        public void OnSelectArmatureFromDialogCommandExecuted<T>(object e)
            where T : class, IBaseEquip, new()
        {
            ExceptionHelper.SafeExecute(() =>
            {
                var armature = _dialogService.ShowEquipDialog<T>();
                if (armature != null)
                    CurrentProject.SelectedStand.Armature = armature.Name;
            });
        }
        public void OnSelectTreeSocketFromDialogCommandExecuted<T>(object e)
            where T : class, IBaseEquip, new()
        {
            ExceptionHelper.SafeExecute(() =>
            {
                var socket = _dialogService.ShowEquipDialog<T>();
                if (socket != null)
                    CurrentProject.SelectedStand.TreeScoket = socket.Name;
            });
        }
        public void OnSelectCompanyCommandExecuted(object e)
        {
            ExceptionHelper.SafeExecute(() => _dialogService.());
        }

        public bool CanAllCommandsExecute(object e) => true;
        public async void OnCreateNewCardCommandExecuted(object e) // Создание новой карточки проекта
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var newProjectCard = new ProjectInfo
                {
                    Number = CurrentProject.Number,
                    Description = CurrentProject.Description,
                    CreationDate = DateOnly.FromDateTime(CurrentProject.CreationDate),
                    Company = CurrentProject.Company,
                    Object = CurrentProject.Object,
                    StandCount = CurrentProject.StandCount,
                    Cost = CurrentProject.Cost,
                    HumanCost = CurrentProject.HumanCost,
                    Manager = CurrentProject.Manager,
                    Status = ComboBoxHelper.ComboBoxChangedValue<ProjectStatus>(CurrentProject.Status),
                    StartDate = DateOnly.FromDateTime(CurrentProject.StartDate),
                    OutOfProduction = DateOnly.FromDateTime(CurrentProject.OutOfProduction),
                    EndDate = DateOnly.FromDateTime(CurrentProject.EndDate),
                    OrderCustomer = CurrentProject.OrderCustomer,
                    RequestProduction = CurrentProject.RequestProduction,
                    MarkMinus = CurrentProject.MarkMinus,
                    MarkPlus = CurrentProject.MarkPlus,
                    IsGalvanized = CurrentProject.IsGalvanized
                };
                await _projectRepository.AddAsync(newProjectCard);

                CurrentProject.CurrentProjectId = newProjectCard.Id;
                CurrentProject.Stands.Clear();
                CurrentStand = new StandModel();
                OnPropertyChanged(nameof(CurrentStand));

                MessageBoxHelper.ShowInfo($"Новая карточка проекта успешно создана!\nId Преокта: {CurrentProject.CurrentProjectId}"); //Для отладки
            });
        }
        public async void OnAddNewStandCommandExecuted(object e) // Добавление нового стенда с привязкой к проекту
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                if (CurrentProject.CurrentProjectId == 0)
                {
                    MessageBoxHelper.ShowInfo("Сначала создайте проект");
                    return;
                }
                var newStand = new Stand
                {
                    ProjectInfoId = CurrentProject.CurrentProjectId,
                    Number = CurrentProject.Number,
                    KKSCode = CurrentStand.KKSCode,
                    Design = CurrentStand.Design,
                    BraceType = CurrentStand.BraceType,
                    Devices = CurrentStand.Devices,
                    Width = CurrentStand.Width,
                    SerialNumber = CurrentStand.SerialNumber,
                    Weight = CurrentStand.Weight,
                    StandSummCost = CurrentStand.StandSummCost,
                    ObvyazkaType = CurrentStand.ObvyazkaType,
                    NN = CurrentStand.NN,
                    MaterialLine = CurrentStand.MaterialLine,
                    Armature = CurrentStand.Armature,
                    TreeScoket = CurrentStand.TreeScoket,
                    KMCH = CurrentStand.KMCH,
                    FirstSensorType = CurrentStand.FirstSensorType
                };

                await _projectRepository.AddStandAsync(CurrentProject.CurrentProjectId, newStand);
                CurrentProject.Stands.Add(newStand);
                CurrentProject.SelectedStand = newStand;
                MessageBoxHelper.ShowInfo("Стенд успешно добавлен!");
            });
        }
        public async void OnSaveChangesCommandExecuted(object e) // Сохранение изменений для карточки преокта
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                if (CurrentProject.CurrentProjectId == 0)
                {
                    MessageBoxHelper.ShowInfo("Сначала создайте проект");
                    return;
                }
                var projectInfo = new ProjectInfo
                {
                    Id = CurrentProject.CurrentProjectId,
                    Number = CurrentProject.Number,
                    Description = CurrentProject.Description,
                    CreationDate = DateOnly.FromDateTime(CurrentProject.CreationDate),
                    Company = CurrentProject.Company,
                    Object = CurrentProject.Object,
                    StandCount = CurrentProject.StandCount,
                    Cost = CurrentProject.Cost,
                    HumanCost = CurrentProject.HumanCost,
                    Manager = CurrentProject.Manager,
                    Status = ComboBoxHelper.ComboBoxChangedValue<ProjectStatus>(CurrentProject.Status),
                    StartDate = DateOnly.FromDateTime(CurrentProject.StartDate),
                    OutOfProduction = DateOnly.FromDateTime(CurrentProject.OutOfProduction),
                    EndDate = DateOnly.FromDateTime(CurrentProject.EndDate),
                    OrderCustomer = CurrentProject.OrderCustomer,
                    RequestProduction = CurrentProject.RequestProduction,
                    MarkMinus = CurrentProject.MarkMinus,
                    MarkPlus = CurrentProject.MarkPlus,
                    IsGalvanized = CurrentProject.IsGalvanized
                };
                await _projectRepository.UpdateAsync(projectInfo);
                MessageBoxHelper.ShowInfo("Изменения успешно сохранены!");
            });
        }

        public async void OnSaveObvCommandExecuted(object e) // Сохранение изменений для обвязки
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                if (CurrentProject.SelectedStand == null)
                    return;

                await _projectRepository.UpdateStandAsync(CurrentProject.SelectedStand);
                MessageBoxHelper.ShowInfo("Изменения обвязки успешно сохранены!");
            });
        }
        #endregion
    }
}
