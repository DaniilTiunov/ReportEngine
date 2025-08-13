using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Enums;
using ReportEngine.Shared.Helpers;
using System.Collections.ObjectModel;

namespace ReportEngine.App.Model
{
    public class ProjectModel : BaseViewModel
    {
        #region Приватные поля
        private int _number;//№п/п

        private string? _description;//Обозначение КД

        private DateTime _creationDate; //Дата запроса

        private string? _company; //Заказчик

        private string? _object; //Объект

        private int _standCount; //Кол-во стендов

        private decimal _cost; //Стоимость

        private string _status; //Статус

        private DateTime _startDate; //Старта проекта

        private DateTime _outOfProduction; //Выход из производства

        private DateTime _endDate; //Окончание догвора

        private string? _orderCustomer; //Заказ покупателя

        private string? _requestProduction; //Заявка на производство

        private string? _markPlus; //Маркировка +

        private string? _markMinus; //Маркировка -

        private bool _isGalvanized; //Оцинковка
                                    //
        private float _humanCost; //Трудозатраты

        private string? _manager;

        private StandModel? _stand;
        #endregion

        #region Публичные свойства
        public IEnumerable<string> Statuses { get; set; } = new List<string>() { "ТКП", "Завершен" };

        public int CurrentProjectId;
        public ObservableCollection<StandModel> Stands { get; set; } = new();
        public StandModel? SelectedStand { get => _stand; set => Set(ref _stand, value); } //Выбранный стенд; 


        public int Number { get => _number; set => Set(ref _number, value); } //№п/п
        public string? Description { get => _description; set => Set(ref _description, value); } //Обозначение КД                               
        public DateTime CreationDate { get => _creationDate; set => Set(ref _creationDate, value); } //Дата запроса
        public string? Company { get => _company; set => Set(ref _company, value); } //Заказчик
        public string? Object { get => _object; set => Set(ref _object, value); } //Объект
        public int StandCount { get => _standCount; set => Set(ref _standCount, value); } //Кол-во стендов
        public decimal Cost { get => _cost; set => Set(ref _cost, value); } //Стоимость
        public float HumanCost { get => _humanCost; set => Set(ref _humanCost, value); } //Трудозатраты
        public string Manager { get => _manager; set => Set(ref _manager, value); } // Руководитель
        public string Status { get => _status; set => Set(ref _status, value); } //Статус
        public DateTime StartDate { get => _startDate; set => Set(ref _startDate, value); } //Старта проекта
        public DateTime OutOfProduction { get => _outOfProduction; set => Set(ref _outOfProduction, value); } //Выход из производства
        public DateTime EndDate { get => _endDate; set => Set(ref _endDate, value); } //Окончание догвора
        public string? OrderCustomer { get => _orderCustomer; set => Set(ref _orderCustomer, value); } //Заказ покупателя
        public string? RequestProduction { get => _requestProduction; set => Set(ref _requestProduction, value); } //Заявка на производство
        public string? MarkPlus { get => _markPlus; set => Set(ref _markPlus, value); } //Маркировка +
        public string? MarkMinus { get => _markMinus; set => Set(ref _markMinus, value); } //Маркировка -
        public bool IsGalvanized { get => _isGalvanized; set => Set(ref _isGalvanized, value); } //Оцинковка
        #endregion

        #region Методы

        public ProjectInfo CreateNewProjectCard()
        {
            return new ProjectInfo
            {
                Id = this.CurrentProjectId,
                Number = this.Number,
                Description = this.Description,
                CreationDate = DateOnly.FromDateTime(this.CreationDate),
                Company = this.Company,
                Object = this.Object,
                StandCount = this.StandCount,
                Cost = this.Cost,
                HumanCost = this.HumanCost,
                Manager = this.Manager,
                Status = ComboBoxHelper.ComboBoxChangedValue<ProjectStatus>(this.Status),
                StartDate = DateOnly.FromDateTime(this.StartDate),
                OutOfProduction = DateOnly.FromDateTime(this.OutOfProduction),
                EndDate = DateOnly.FromDateTime(this.EndDate),
                OrderCustomer = this.OrderCustomer,
                RequestProduction = this.RequestProduction,
                MarkMinus = this.MarkMinus,
                MarkPlus = this.MarkPlus,
                IsGalvanized = this.IsGalvanized
            };
        }
        #endregion
    }
}
