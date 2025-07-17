using ReportEngine.App.Commands;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Enums;
using ReportEngine.Domain.Repositories.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels
{
    public class ProjectViewModel : BaseViewModel
    {
        private readonly IBaseRepository<ProjectInfo> _projectRepository;

        #region Приватные поля
        private ObservableCollection<ProjectInfo> _allProjects;

        private int _number;//№п/п

        private string? _description;//Обозначение КД

        private DateOnly _creationDate; //Дата запроса

        private string? _company; //Заказчик

        private string? _object; //Объект

        private int _standCount; //Кол-во стендов

        private float _cost; //Стоимость

        private ProjectStatus _status; //Статус

        private DateOnly _startDate; //Старта проекта

        private DateOnly _outOfProduction; //Выход из производства

        private DateOnly _endDate; //Окончание догвора

        private string? _orderCustomer; //Заказ покупателя

        private string? _requestProduction; //Заявка на производство

        private string? _markPlus; //Маркировка +

        private string? _markMinus; //Маркировка -

        private bool _isGalvanized; //Оцинковка 
        #endregion

        #region публичные свойства
        public int Number { get => _number; set => Set(ref _number, value); } //№п/п
        public string? Description { get => _description; set => Set(ref _description, value); } //Обозначение КД                               
        public DateOnly CreationDate { get => _creationDate; set => Set(ref _creationDate, value); } //Дата запроса
        public string? Company { get => _company; set => Set(ref _company, value); } //Заказчик
        public string? Object { get => _object; set => Set(ref _object, value); } //Объект
        public int StandCount { get => _standCount; set => Set(ref _standCount, value); } //Кол-во стендов
        public float Cost { get => _cost; set => Set(ref _cost, value); } //Стоимость
        public ProjectStatus Status { get => _status; set => Set(ref _status, value); } //Статус
        public DateOnly StartDate { get => _startDate; set => Set(ref _startDate, value); } //Старта проекта
        public DateOnly OutOfProduction { get => _outOfProduction; set => Set(ref _outOfProduction, value); } //Выход из производства
        public DateOnly EndDate { get => _endDate; set => Set(ref _endDate, value); } //Окончание догвора
        public string? OrderCustomer { get => _orderCustomer; set => Set(ref _orderCustomer, value); } //Заказ покупателя
        public string? RequestProduction { get => _requestProduction; set => Set(ref _requestProduction, value); } //Заявка на производство
        public string? MarkPlus { get => _markPlus; set => Set(ref _markPlus, value); } //Маркировка +
        public string? MarkMinus { get => _markMinus; set => Set(ref _markMinus, value); } //Маркировка -
        public bool IsGalvanized { get => _isGalvanized; set => Set(ref _isGalvanized, value); } //Оцинковка
        public ObservableCollection<ProjectInfo> AllProjects { get => _allProjects; set => Set(ref _allProjects, value); } 
        #endregion

        public ProjectViewModel(IBaseRepository<ProjectInfo> projectRepository)
        {
            CreateNewCardCommand = new RelayCommand(OnCreateNewCardCommandExecuted, CanCreateNewCardCommandExecute);

            _projectRepository = projectRepository;
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
                    CreationDate = CreationDate,
                    Company = Company,
                    Object = Object,
                    StandCount = StandCount,
                    Cost = Cost,
                    Status = Status,
                    StartDate = StartDate,
                    OutOfProduction = OutOfProduction,
                    EndDate = EndDate,
                    OrderCustomer = OrderCustomer,
                    RequestProduction = RequestProduction,
                    MarkMinus = MarkMinus,
                    MarkPlus = MarkPlus,
                    isGalvanized = IsGalvanized
                };

                await _projectRepository.AddAsync(newProjectCard);
            }
            catch(Exception ex){MessageBox.Show(ex.Message);} 

        }

        public ICommand DeleteCardCommand { get; set; }        
    }
}
