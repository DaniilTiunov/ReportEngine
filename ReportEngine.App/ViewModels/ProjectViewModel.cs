using ReportEngine.App.Commands;
using ReportEngine.App.Model;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Enums;
using ReportEngine.Domain.Repositories;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Config.DebugConsol;
using ReportEngine.Shared.Helpers;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels
{
    public class ProjectViewModel : BaseViewModel
    {
        private readonly IProjectInfoRepository _projectRepository;
        public StandModel CurrentStand { get; set; }
        public ProjectModel CurrentProject { get; set; }
        
 
        public ProjectViewModel(IProjectInfoRepository projectRepository)
        {          
            _projectRepository = projectRepository;

            InitializeModels();
            InitializeCommands();
            InitializeTime();
        }

        #region Методы
        public void InitializeModels()
        {
            CurrentProject = new ProjectModel();
            CurrentStand = new StandModel();
        }
        public void InitializeTime()
        {
            CurrentProject.CreationDate = DateTime.Now.Date;
            CurrentProject.StartDate = DateTime.Now.Date;
            CurrentProject.OutOfProduction = DateTime.Now.Date;
            CurrentProject.EndDate = DateTime.Now.Date;
        }
        public void InitializeCommands()
        {
            CreateNewCardCommand = new RelayCommand(OnCreateNewCardCommandExecuted, CanCreateNewCardCommandExecute);
            AddNewStandCommand = new RelayCommand(OnAddNewStandCommandExecuted, CanAddNewStandCommandExecute);
        }
        #endregion

        #region Команды
        public ICommand CreateNewCardCommand { get; set; }
        public bool CanCreateNewCardCommandExecute(object e) => true;
        public async void OnCreateNewCardCommandExecuted(object e)
        {
            try
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
                    isGalvanized = CurrentProject.IsGalvanized
                };


                await _projectRepository.AddAsync(newProjectCard);

                CurrentProject.CurrentProjectId = newProjectCard.Id;

                MessageBox.Show($"Проект создан! ID: {newProjectCard.Id}");

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public ICommand AddNewStandCommand { get; set; }
        public bool CanAddNewStandCommandExecute(object e) => true;
        public async void OnAddNewStandCommandExecuted(object e)
        {
            try
            {
                if (CurrentProject.CurrentProjectId == 0)
                {
                    MessageBox.Show("Сначала создайте проект");
                    return;
                }

                var newStand = new Stand
                {
                    ProjectInfoId = CurrentProject.CurrentProjectId,
                    Number = CurrentProject.CurrentProjectId,
                    KKSCode = "4",
                    Design = "4"
                };

                await _projectRepository.AddStandAsync(CurrentProject.CurrentProjectId, newStand);
                MessageBox.Show("Стенд успешно добавлен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении стенда: {ex.Message}");
            }
        } 
        #endregion

    }
}
