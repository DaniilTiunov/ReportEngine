using ReportEngine.App.Commands;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Enums;
using ReportEngine.Domain.Repositories;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Config.DebugConsol;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels
{
    public class ProjectViewModel : BaseViewModel
    {
        private readonly ProjectInfoRepository _projectRepository;


        #region Приватные поля

        private ObservableCollection<ProjectInfo> _allProjects;

        private int _number;//№п/п

        private string? _description;//Обозначение КД

        private DateTime _creationDate; //Дата запроса

        private string? _company; //Заказчик

        private string? _object; //Объект

        private int _standCount; //Кол-во стендов

        private decimal _cost; //Стоимость

        private ProjectStatus _status; //Статус

        private DateTime _startDate; //Старта проекта

        private DateTime _outOfProduction; //Выход из производства

        private DateTime _endDate; //Окончание догвора

        private string? _orderCustomer; //Заказ покупателя

        private string? _requestProduction; //Заявка на производство

        private string? _markPlus; //Маркировка +

        private string? _markMinus; //Маркировка -

        private bool _isGalvanized; //Оцинковка 
        #endregion

        #region Публичные свойства
        public int CurrentProjectId;
        public int Number { get => _number; set => Set(ref _number, value); } //№п/п
        public string? Description { get => _description; set => Set(ref _description, value); } //Обозначение КД                               
        public DateTime CreationDate { get => _creationDate; set => Set(ref _creationDate, value); } //Дата запроса
        public string? Company { get => _company; set => Set(ref _company, value); } //Заказчик
        public string? Object { get => _object; set => Set(ref _object, value); } //Объект
        public int StandCount { get => _standCount; set => Set(ref _standCount, value); } //Кол-во стендов
        public decimal Cost { get => _cost; set => Set(ref _cost, value); } //Стоимость
        public ProjectStatus Status { get => _status; set => Set(ref _status, value); } //Статус
        public DateTime StartDate { get => _startDate; set => Set(ref _startDate, value); } //Старта проекта
        public DateTime OutOfProduction { get => _outOfProduction; set => Set(ref _outOfProduction, value); } //Выход из производства
        public DateTime EndDate { get => _endDate; set => Set(ref _endDate, value); } //Окончание догвора
        public string? OrderCustomer { get => _orderCustomer; set => Set(ref _orderCustomer, value); } //Заказ покупателя
        public string? RequestProduction { get => _requestProduction; set => Set(ref _requestProduction, value); } //Заявка на производство
        public string? MarkPlus { get => _markPlus; set => Set(ref _markPlus, value); } //Маркировка +
        public string? MarkMinus { get => _markMinus; set => Set(ref _markMinus, value); } //Маркировка -
        public bool IsGalvanized { get => _isGalvanized; set => Set(ref _isGalvanized, value); } //Оцинковка
        public ObservableCollection<ProjectInfo> AllProjects { get => _allProjects; set => Set(ref _allProjects, value); } 
        #endregion

        public ProjectViewModel(ProjectInfoRepository projectRepository)
        {
            CreateNewCardCommand = new RelayCommand(OnCreateNewCardCommandExecuted, CanCreateNewCardCommandExecute);
            AddNewStandCommand = new RelayCommand(OnAddNewStandCommandExecuted, CanAddNewStandCommandExecute);

            _projectRepository = projectRepository;

            CreationDate = DateTime.Now.Date;
            StartDate = DateTime.Now.Date;
            OutOfProduction = DateTime.Now.Date;
            EndDate = DateTime.Now.Date;
        }
        public ICommand CreateNewCardCommand { get; set; }
        public bool CanCreateNewCardCommandExecute(object e) => true;
        public async void OnCreateNewCardCommandExecuted(object e)
        {
            try
            {
                var newProjectCard = new ProjectInfo
                {
                    Number = Number,
                    Description = Description,
                    CreationDate = DateOnly.FromDateTime(CreationDate),
                    Company = Company,
                    Object = Object,
                    StandCount = StandCount,
                    Cost = Cost,
                    Status = Enum.GetValues(typeof(ProjectStatus)).Cast<ProjectStatus>().First(),
                    StartDate = DateOnly.FromDateTime(StartDate),
                    OutOfProduction = DateOnly.FromDateTime(OutOfProduction),
                    EndDate = DateOnly.FromDateTime(EndDate),
                    OrderCustomer = OrderCustomer,
                    RequestProduction = RequestProduction,
                    MarkMinus = MarkMinus,
                    MarkPlus = MarkPlus,
                    isGalvanized = IsGalvanized
                };


                await _projectRepository.AddAsync(newProjectCard);

                CurrentProjectId = newProjectCard.Id;

                MessageBox.Show("Карточка проекта успешно создана!");

            }
            catch(Exception ex){MessageBox.Show(ex.Message);} 
        }
        
        public ICommand AddNewStandCommand { get; set; }
        public bool CanAddNewStandCommandExecute(object e) => true;
        public async void OnAddNewStandCommandExecuted(object e)
        {
            try
            {
                if (CurrentProjectId == 0)
                {
                    MessageBox.Show("Сначала создайте проект");
                    return;
                }

                var newStand = new Stand
                {
                    ProjectInfoId = CurrentProjectId,
                    Number = 288,
                    KKSCode = "4",
                    Design = "4"
                };

                await _projectRepository.AddStandAsync(CurrentProjectId, newStand);
                MessageBox.Show("Стенд успешно добавлен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении стенда: {ex.Message}");
            }
        }

    }
}
