using ReportEngine.App.Commands;
using ReportEngine.App.Display;
using ReportEngine.App.Model;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Enums;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels
{
    public class ProjectViewModel : BaseViewModel
    {
        private readonly IProjectInfoRepository _projectRepository;
        public StandModel CurrentStand { get; set; } = new();
        public ProjectModel CurrentProject { get; set; } = new();


        public ProjectViewModel(IProjectInfoRepository projectRepository)
        {
            _projectRepository = projectRepository;

            InitializeCommands();
            InitializeTime();
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
            CreateNewCardCommand = new RelayCommand(OnCreateNewCardCommandExecuted, CanAllCommandsExecute);
            AddNewStandCommand = new RelayCommand(OnAddNewStandCommandExecuted, CanAllCommandsExecute);
            SaveChangesCommand = new RelayCommand(OnSaveChangesCommandExecuted, CanAllCommandsExecute);
        }
        public void LoadProjectInfo(ProjectInfo projectInfo)
        {
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
            OnPropertyChanged(nameof(CurrentProject));
        }
        #endregion
        #region Команды
        public ICommand CreateNewCardCommand { get; set; }
        public bool CanAllCommandsExecute(object e) => true;
        public async void OnCreateNewCardCommandExecuted(object e)
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

                MessageBoxHelper.ShowInfo($"Новая карточка проекта успешно создана!\nId Преокта: {CurrentProject.CurrentProjectId}"); //Для отладки
            });
        }
        public ICommand AddNewStandCommand { get; set; }
        public async void OnAddNewStandCommandExecuted(object e)
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
                    KMCH = CurrentStand.KMCH
                };
                await _projectRepository.AddStandAsync(CurrentProject.CurrentProjectId, newStand);
                CurrentProject.Stands.Add(newStand);
                MessageBoxHelper.ShowInfo("Стенд успешно добавлен!");
            });
        }
        public ICommand SaveChangesCommand { get; set; }
        public async void OnSaveChangesCommandExecuted(object e)
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
        #endregion
    }
}
