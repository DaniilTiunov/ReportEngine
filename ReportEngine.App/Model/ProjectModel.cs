using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Enums;
using ReportEngine.Shared.Helpers;
using System.Collections.ObjectModel;

namespace ReportEngine.App.Model;

public class ProjectModel : BaseViewModel
{
    #region Методы

    public ProjectInfo CreateNewProjectCard()
    {
        return new ProjectInfo
        {
            Id = CurrentProjectId,
            Number = Number,
            Description = Description,
            CreationDate = DateOnly.FromDateTime(CreationDate),
            Company = Company,
            Object = Object,
            StandCount = StandCount,
            Cost = Cost,
            HumanCost = HumanCost,
            Manager = Manager,
            Status = ComboBoxHelper.ComboBoxChangedValue<ProjectStatus>(Status),
            StartDate = DateOnly.FromDateTime(StartDate),
            OutOfProduction = DateOnly.FromDateTime(OutOfProduction),
            EndDate = DateOnly.FromDateTime(EndDate),
            OrderCustomer = OrderCustomer,
            RequestProduction = RequestProduction,
            MarkMinus = MarkMinus,
            MarkPlus = MarkPlus,
            IsGalvanized = IsGalvanized
        };
    }

    #endregion

    #region Приватные поля

    private int _number; //№п/п

    private string? _description; //Обозначение КД

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
    
    private ObservableCollection<ContainerStand> _containerStandsInSelectedBatch = new();
    private ObservableCollection<Stand> _standsInSelectedContainer = new();
    

    //
    private float _humanCost; //Трудозатраты

    private string? _manager;

    private StandModel? _stand;


    private ObservableCollection<ContainerStand> _containerStandsInProject = new();
    private ObservableCollection<ContainerBatch> _containerBathInProject = new();

    private ContainerStand? _containerStand;
    private ContainerBatch? _containerBatch;

    private ContainerStand? _selectedContainerStand;
    private ContainerBatch? _selectedContainerBatch;

    #endregion

    #region Публичные свойства
    public IEnumerable<string> Statuses { get; set; } =
        new List<string> { "Расчёт", "ТКП", "Производство", "Завершен" };

    public int CurrentProjectId;
    private ObservableCollection<StandModel> _standsInContainer;

    public ObservableCollection<StandModel> Stands { get; set; } = new();

    public StandModel? SelectedStand
    {
        get => _stand;
        set => Set(ref _stand, value);
    } //Выбранный стенд; 


    public int Number
    {
        get => _number;
        set => Set(ref _number, value);
    } //№п/п

    public string? Description
    {
        get => _description;
        set => Set(ref _description, value);
    } //Обозначение КД

    public DateTime CreationDate
    {
        get => _creationDate;
        set => Set(ref _creationDate, value);
    } //Дата запроса

    public string? Company
    {
        get => _company;
        set => Set(ref _company, value);
    } //Заказчик

    public string? Object
    {
        get => _object;
        set => Set(ref _object, value);
    } //Объект

    public int StandCount
    {
        get => _standCount;
        set => Set(ref _standCount, value);
    } //Кол-во стендов

    public decimal Cost
    {
        get => _cost;
        set => Set(ref _cost, value);
    } //Стоимость

    public float HumanCost
    {
        get => _humanCost;
        set => Set(ref _humanCost, value);
    } //Трудозатраты

    public string Manager
    {
        get => _manager;
        set => Set(ref _manager, value);
    } // Руководитель

    public string Status
    {
        get => _status;
        set => Set(ref _status, value);
    } //Статус

    public DateTime StartDate
    {
        get => _startDate;
        set => Set(ref _startDate, value);
    } //Старта проекта

    public DateTime OutOfProduction
    {
        get => _outOfProduction;
        set => Set(ref _outOfProduction, value);
    } //Выход из производства

    public DateTime EndDate
    {
        get => _endDate;
        set => Set(ref _endDate, value);
    } //Окончание догвора

    public string? OrderCustomer
    {
        get => _orderCustomer;
        set => Set(ref _orderCustomer, value);
    } //Заказ покупателя

    public string? RequestProduction
    {
        get => _requestProduction;
        set => Set(ref _requestProduction, value);
    } //Заявка на производство

    public string? MarkPlus
    {
        get => _markPlus;
        set => Set(ref _markPlus, value);
    } //Маркировка +

    public string? MarkMinus
    {
        get => _markMinus;
        set => Set(ref _markMinus, value);
    } //Маркировка -

    public bool IsGalvanized
    {
        get => _isGalvanized;
        set => Set(ref _isGalvanized, value);
    } //Оцинковка

    // Упаковка
    public ObservableCollection<ContainerStand> ContainerStandsInProject
    { 
        get => _containerStandsInProject; 
        set => Set(ref _containerStandsInProject, value);
    }// Упаковки
    public ObservableCollection<ContainerBatch> ContainerBatchesInProject
    { 
        get => _containerBathInProject; 
        set => Set(ref _containerBathInProject, value);
    }// Партии

    public ObservableCollection<StandModel> StandsInContainer
    {
        get => _standsInContainer;
        set => Set(ref _standsInContainer, value);
    }
    public ContainerStand? ContainerStand 
    { 
        get => _containerStand; 
        set => Set(ref _containerStand, value); 
    }// Упаковка
    public ContainerBatch? ContainerBatch 
    {
        get => _containerBatch;
        set => Set(ref _containerBatch, value);
    }// Партия
    public ContainerStand? SelectedContainerStand 
    { 
        get => _selectedContainerStand; 
        set
        {
            if (Set(ref _selectedContainerStand, value))
            {
                StandsInSelectedContainer = value != null
                    ? new ObservableCollection<Stand>(value.Stands)
                    : new ObservableCollection<Stand>();
                OnPropertyChanged(nameof(StandsInSelectedContainer));
            }
        }
    }// Выбранная упаковка
    public ContainerBatch? SelectedContainerBatch 
    { 
        get => _selectedContainerBatch;
        set
        {
            if (Set(ref _selectedContainerBatch, value))
            {
                ContainerStandsInSelectedBatch = value != null
                    ? new ObservableCollection<ContainerStand>(value.Containers)
                    : new ObservableCollection<ContainerStand>();
                OnPropertyChanged(nameof(ContainerStandsInSelectedBatch));
                // Обновить StandsInSelectedContainer тоже, если нужно
                if (SelectedContainerStand != null)
                    StandsInSelectedContainer = new ObservableCollection<Stand>(SelectedContainerStand.Stands);
                else
                    StandsInSelectedContainer = new ObservableCollection<Stand>();
            }
        }
    }// Выбранная партия
    
    public ObservableCollection<ContainerStand> ContainerStandsInSelectedBatch
    {
        get => _containerStandsInSelectedBatch;
        set => Set(ref _containerStandsInSelectedBatch, value);
    }

    public ObservableCollection<Stand> StandsInSelectedContainer
    {
        get => _standsInSelectedContainer;
        set => Set(ref _standsInSelectedContainer, value);
    }
    
    private Stand? _selectedStandInContainer;
    private StandModel? _selectedStandInProject;

    public Stand? SelectedStandInContainer
    {
        get => _selectedStandInContainer;
        set => Set(ref _selectedStandInContainer, value);
    }

    public StandModel? SelectedStandInProject
    {
        get => _selectedStandInProject;
        set => Set(ref _selectedStandInProject, value);
    }

    #endregion
}