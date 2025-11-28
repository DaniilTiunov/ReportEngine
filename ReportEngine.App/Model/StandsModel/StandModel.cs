using ReportEngine.App.Model.CalculationModels;
using ReportEngine.App.Model.FormedEquipsModels;
using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;
using System.Collections.ObjectModel;

namespace ReportEngine.App.Model.StandsModel;

/// <summary>
///     Модель представления стенда (UI-модель) используемая в WPF для биндинга.
///     Содержит поля и свойства, отражающие данные стенда и связанные коллекции для отображения в интерфейсе.
/// </summary>
public class StandModel : BaseViewModel
{
    // Коллекция дополнительных комплектующих, находящихся в стенде
    private ObservableCollection<FormedAdditionalEquip> _additionalEquipsInStand = new();

    // Коллекция всех целей для дополнительных комплектующих в стенде
    private ObservableCollection<AdditionalEquipPurpose> _allAdditionalEquipPurposesInStand = new();

    // Коллекция всех доступных типов дренажей
    private ObservableCollection<FormedDrainage> _allAvailableDrainages = new();

    // Коллекция всех доступных рам
    private ObservableCollection<FormedFrame> _allAvailableFrames = new();

    // Коллекция всех целей дренажей, собранная из дренажей в стенде
    private ObservableCollection<DrainagePurpose> _allDrainagePurposesInStand = new();

    // Коллекция всех целей электрических компонентов, собранная из компонентов в стенде
    private ObservableCollection<ElectricalPurpose> _allElectricalPurposesInStand = new();

    // Арматура стенда (текст)
    private string _armature;
    private string? _armatureCostPerUnit;
    private float? _armatureCount;
    private string? _armatureMeasure;

    // Тип крепления датчика
    private string _braceType;

    // Комментарии к стенду
    private string _comments;

    // Обозначение/дизайн стенда
    private string _design;

    // Описание стенда
    private string _designeStand;

    // Количество приборов
    private int _devices;

    // Коллекция дренажей, находящихся в стенде
    private ObservableCollection<FormedDrainage> _drainagesInStand = new();

    // Коллекция электрических компонентов, находящихся в стенде
    private ObservableCollection<FormedElectricalComponent> _electricalComponentsInStand = new();

    // KKS код первого датчика (внутреннее поле)
    private string _firsSensorKksCode;
    private string? _firstSensorDescription;

    // Маркировка + для первого датчика
    private string _firstSensorMarkMinus;

    // Маркировка - для первого датчика
    private string _firstSensorMarkPlus;

    // Тип первого датчика
    private string _firstSensorType;

    // Модель компонентов рамы (вспомогательная модель)
    private FormedFrameModel _formedFrameComponents = new();

    // Коллекция рам, добавленных в стенд
    private ObservableCollection<FormedFrame> _framesInStand = new();

    // Идентификатор стенда
    private int _id;

    // Бинарные данные изображения чертежа стенда
    private byte[]? _imageData;

    // MIME тип или краткое описание типа изображения (например "image/png")
    private string? _imageType;

    // KKS-код стенда
    private string _kksCode;

    // КМЧ
    private string _kmch;
    private string? _kmchCostPerUnit;
    private float? _kmchCount;
    private string? _kmchMeasure;


    // Материал линии
    private string _materialLine;
    private string _materialLineCostPerUnit;
    private float? _materialLineCount;
    private string? _materialLineMeasure;

    // Объект для создания нового дополнительного комплектующего
    private FormedAdditionalEquip _newAdditionalEquip = new();

    // Объект для создания нового дренажа
    private FormedDrainage _newDrainage = new();

    // Объект для создания нового электрического компонента
    private FormedElectricalComponent _newElectricalComponent = new();

    // Номер по порядку (NN)
    private int _nn;

    // Имя/тип обвязки
    private string _obvyazkaName;

    // Коллекция обвязок (UI)
    private ObservableCollection<StandObvyazkaModel> _obvyazki = new();

    // Коллекция обвязок, привязанных к стенду
    private ObservableCollection<ObvyazkaInStand> _obvyazkiInStand = new();

    // Идентификатор проекта, которому принадлежит стенд
    private int _projectId;
    private string? _secondSensorDescription;

    // Второй датчик: KKS
    private string? _secondSensorKksCode;

    // Второй датчик: маркировка -
    private string? _secondSensorMarkMinus;

    // Второй датчик: маркировка +
    private string? _secondSensorMarkPlus;

    // Тип второго датчика
    private string _secondSensorType;

    // Выбранное дополнительное комплектующее
    private AdditionalEquipPurpose _selectedAdditionalEquip;

    // Выбранный дренаж
    private FormedDrainage _selectedDrainage;
    private DrainagePurpose _selectedDrainagePurpose;

    // Выбранный электрический компонент
    private ElectricalPurpose _selectedElectricalComponent;

    // Выбранная рама
    private FormedFrame _selectedFrame;

    // Выбранная обвязка (UI модель)
    private StandObvyazkaModel _selectedObvyazka;

    // Выбранная обвязка в стенде
    private ObvyazkaInStand _selectedObvyazkaInStand = new();

    // Серийный номер стенда
    private string _serialNumber;

    // Суммарная стоимость стенда
    private decimal _standSummCost;
    private string? _thirdSensorDescription;

    // Третий датчик: KKS
    private string? _thirdSensorKksCode;

    // Третий датчик: маркировка -
    private string? _thirdSensorMarkMinus;

    // Третий датчик: маркировка +
    private string? _thirdSensorMarkPlus;

    // Тип третьего датчика
    private string _thirdSensorType;

    // Тройник/разветвитель
    private string _treeScoket;
    private string? _treeSocketMaterialCostPerUnit;
    private float? _treeSocketMaterialCount;
    private string? _treeSocketMaterialMeasure;

    // Масса стенда
    private float _weight;

    // Ширина стенда
    private float _width;
    private int? _materialLineExportDays;
    private int? _armatureExportDays;
    private int? _kMCHExportDays;
    private int? _treeSocketExportDays;
    private int _number;

    public StandModel()
    {

    }

    // Коллекция обвязок для отображения
    public ObservableCollection<StandObvyazkaModel> Obvyazki
    {
        get => _obvyazki;
        set => Set(ref _obvyazki, value);
    }

    // Выбранная обвязка
    public StandObvyazkaModel SelectedObvyazka
    {
        get => _selectedObvyazka;
        set => Set(ref _selectedObvyazka, value);
    }

    // Варианты расположения датчика на кронштейне
    public IEnumerable<string> BraceSensor { get; } = new List<string> { "На кронштейне", "Швеллер" };

    // Доступные типы датчиков
    public IEnumerable<string> SensorType { get; } = new List<string>
        { "Датчик перепада давления", "Манометр", "Датчик абсолютного давления", "Манометр электрокомпактный" };

    // Идентификатор стенда
    public int Id
    {
        get => _id;
        set => Set(ref _id, value);
    }

    // Идентификатор проекта
    public int ProjectId
    {
        get => _projectId;
        set => Set(ref _projectId, value);
    }

    // KKS-код стенда
    public string KKSCode
    {
        get => _kksCode;
        set => Set(ref _kksCode, value);
    }

    // Обозначение/дизайн стенда
    public string Design
    {
        get => _design;
        set => Set(ref _design, value);
    }

    // Количество приборов на стенде
    public int Devices
    {
        get => _devices;
        set => Set(ref _devices, value);
    }

    // Комментарии к стенду
    public string Comments
    {
        get => _comments;
        set => Set(ref _comments, value);
    }

    // Тип крепления датчика
    public string BraceType
    {
        get => _braceType;
        set => Set(ref _braceType, value);
    }

    // Ширина стенда
    public float Width
    {
        get => _width;
        set => Set(ref _width, value);
    }

    // Серийный номер
    public string SerialNumber
    {
        get => _serialNumber;
        set => Set(ref _serialNumber, value);
    }

    public int Number
    {
        get => _number;
        set => Set(ref _number, value);
    }

    // Масса стенда
    public float Weight
    {
        get => _weight;
        set => Set(ref _weight, value);
    }

    // Суммарная стоимость стенда
    public decimal StandSummCost
    {
        get => _standSummCost;
        set => Set(ref _standSummCost, value);
    }

    // Имя или тип обвязки
    public string ObvyazkaName
    {
        get => _obvyazkaName;
        set => Set(ref _obvyazkaName, value);
    }

    // Порядковый номер NN
    public int NN
    {
        get => _nn;
        set => Set(ref _nn, value);
    }

    // Материал линии
    public string MaterialLine
    {
        get => _materialLine;
        set => Set(ref _materialLine, value);
    }

    public float? MaterialLineCount
    {
        get => _materialLineCount;
        set => Set(ref _materialLineCount, value);
    }

    public string? MaterialLineMeasure
    {
        get => _materialLineMeasure;
        set => Set(ref _materialLineMeasure, value);
    }

    public int? MaterialLineExportDays
    {
        get => _materialLineExportDays;
        set => Set(ref _materialLineExportDays, value);
    }

    public string? MaterialLineCostPerUnit
    {
        get => _materialLineCostPerUnit;
        set => Set(ref _materialLineCostPerUnit, value);
    }

    public string? ArmatureCostPerUnit
    {
        get => _armatureCostPerUnit;
        set => Set(ref _armatureCostPerUnit, value);
    }

    public int? ArmatureExportDays
    {
        get => _armatureExportDays;
        set => Set(ref _armatureExportDays, value);
    }

    public string? KMCHCostPerUnit
    {
        get => _kmchCostPerUnit;
        set => Set(ref _kmchCostPerUnit, value);
    }
    public int? KMCHExportDays
    {
        get => _kMCHExportDays;
        set => Set(ref _kMCHExportDays, value);
    }

    public string? TreeSocketMaterialCostPerUnit
    {
        get => _treeSocketMaterialCostPerUnit;
        set => Set(ref _treeSocketMaterialCostPerUnit, value);
    }

    public int? TreeSocketExportDays
    {
        get => _treeSocketExportDays;
        set => Set(ref _treeSocketExportDays, value);
    }

    // Арматура
    public string Armature
    {
        get => _armature;
        set => Set(ref _armature, value);
    }

    public float? ArmatureCount
    {
        get => _armatureCount;
        set => Set(ref _armatureCount, value);
    }

    public string? ArmatureMeasure
    {
        get => _armatureMeasure;
        set => Set(ref _armatureMeasure, value);
    }

    // Информация о тройнике/разветвителе
    public string TreeSocket
    {
        get => _treeScoket;
        set => Set(ref _treeScoket, value);
    }

    public float? TreeSocketMaterialCount
    {
        get => _treeSocketMaterialCount;
        set => Set(ref _treeSocketMaterialCount, value);
    }

    public string? TreeSocketMaterialMeasure
    {
        get => _treeSocketMaterialMeasure;
        set => Set(ref _treeSocketMaterialMeasure, value);
    }

    // КМЧ
    public string KMCH
    {
        get => _kmch;
        set => Set(ref _kmch, value);
    }

    public float? KMCHCount
    {
        get => _kmchCount;
        set => Set(ref _kmchCount, value);
    }

    public string? KMCHMeasure
    {
        get => _kmchMeasure;
        set => Set(ref _kmchMeasure, value);
    }

    // Тип первого датчика
    public string FirstSensorType
    {
        get => _firstSensorType;
        set => Set(ref _firstSensorType, value);
    }

    // KKS первого датчика (контур)
    public string? FirstSensorKKS
    {
        get => _firsSensorKksCode;
        set => Set(ref _firsSensorKksCode, value);
    } //ККС Контура

    // Маркировка + первого датчика
    public string? FirstSensorMarkPlus
    {
        get => _firstSensorMarkPlus;
        set => Set(ref _firstSensorMarkPlus, value);
    } //Марикровка +

    // Маркировка - первого датчика
    public string? FirstSensorMarkMinus
    {
        get => _firstSensorMarkMinus;
        set => Set(ref _firstSensorMarkMinus, value);
    } //Марикровка -

    public string? FirstSensorDescription
    {
        get => _firstSensorDescription;
        set => Set(ref _firstSensorDescription, value);
    }

    // Тип второго датчика
    public string SecondSensorType
    {
        get => _secondSensorType;
        set => Set(ref _secondSensorType, value);
    }

    // KKS второго датчика
    public string? SecondSensorKKS
    {
        get => _secondSensorKksCode;
        set => Set(ref _secondSensorKksCode, value);
    } //ККС Контура

    // Маркировка + второго датчика
    public string? SecondSensorMarkPlus
    {
        get => _secondSensorMarkPlus;
        set => Set(ref _secondSensorMarkPlus, value);
    } //Марикровка +

    // Маркировка - второго датчика
    public string? SecondSensorMarkMinus
    {
        get => _secondSensorMarkMinus;
        set => Set(ref _secondSensorMarkMinus, value);
    } //Марикровка -

    public string? SecondSensorDescription
    {
        get => _secondSensorDescription;
        set => Set(ref _secondSensorDescription, value);
    }

    // Тип третьего датчика
    public string ThirdSensorType
    {
        get => _thirdSensorType;
        set => Set(ref _thirdSensorType, value);
    }

    // KKS третьего датчика
    public string? ThirdSensorKKS
    {
        get => _thirdSensorKksCode;
        set => Set(ref _thirdSensorKksCode, value);
    }

    // Маркировка + третьего датчика
    public string? ThirdSensorMarkPlus
    {
        get => _thirdSensorMarkPlus;
        set => Set(ref _thirdSensorMarkPlus, value);
    } //Марикровка +

    // Маркировка - третьего датчика
    public string? ThirdSensorMarkMinus
    {
        get => _thirdSensorMarkMinus;
        set => Set(ref _thirdSensorMarkMinus, value);
    } //Марикровка -

    public string? ThirdSensorDescription
    {
        get => _thirdSensorDescription;
        set => Set(ref _thirdSensorDescription, value);
    }

    // Описание стенда
    public string? DesigneStand
    {
        get => _designeStand;
        set => Set(ref _designeStand, value);
    } //Описание

    // Бинарные данные изображения чертежа стенда
    public byte[]? ImageData
    {
        get => _imageData;
        set => Set(ref _imageData, value);
    }

    // MIME-тип изображения (например "image/png")
    public string? ImageType
    {
        get => _imageType;
        set => Set(ref _imageType, value);
    }

    // Коллекция всех доступных рам
    public ObservableCollection<FormedFrame> AllAvailableFrames
    {
        get => _allAvailableFrames;
        set => Set(ref _allAvailableFrames, value);
    }

    // Рамы, находящиеся в стенде
    public ObservableCollection<FormedFrame> FramesInStand
    {
        get => _framesInStand;
        set => Set(ref _framesInStand, value);
    }

    // Коллекция всех доступных дренажей
    public ObservableCollection<FormedDrainage> AllAvailableDrainages
    {
        get => _allAvailableDrainages;
        set => Set(ref _allAvailableDrainages, value);
    }

    // Дренажи, находящиеся в стенде
    public ObservableCollection<FormedDrainage> DrainagesInStand
    {
        get => _drainagesInStand;
        set => Set(ref _drainagesInStand, value);
    }

    // Обвязки, привязанные к стенду
    public ObservableCollection<ObvyazkaInStand> ObvyazkiInStand
    {
        get => _obvyazkiInStand;
        set => Set(ref _obvyazkiInStand, value);
    }

    // Выбранный дренаж
    public FormedDrainage SelectedDrainage
    {
        get => _selectedDrainage;
        set => Set(ref _selectedDrainage, value);
    }

    public DrainagePurpose SelectedDrainagePurpose
    {
        get => _selectedDrainagePurpose;
        set => Set(ref _selectedDrainagePurpose, value);
    }

    // Выбранная рама
    public FormedFrame SelectedFrame
    {
        get => _selectedFrame;
        set => Set(ref _selectedFrame, value);
    }

    // Выбранная обвязка в стенде
    public ObvyazkaInStand SelectedObvyazkaInStand
    {
        get => _selectedObvyazkaInStand;
        set => Set(ref _selectedObvyazkaInStand, value);
    }

    // Новый дренаж для добавления
    public FormedDrainage NewDrainage
    {
        get => _newDrainage;
        set => Set(ref _newDrainage, value);
    }

    // Электрические компоненты в стенде
    public ObservableCollection<FormedElectricalComponent> ElectricalComponentsInStand
    {
        get => _electricalComponentsInStand;
        set => Set(ref _electricalComponentsInStand, value);
    }

    // Дополнительные комплектующие в стенде
    public ObservableCollection<FormedAdditionalEquip> AdditionalEquipsInStand
    {
        get => _additionalEquipsInStand;
        set => Set(ref _additionalEquipsInStand, value);
    }

    // Выбранный электрический компонент
    public ElectricalPurpose SelectedElectricalComponent
    {
        get => _selectedElectricalComponent;
        set => Set(ref _selectedElectricalComponent, value);
    }

    // Выбранное дополнительное комплектующее
    public AdditionalEquipPurpose SelectedAdditionalEquip
    {
        get => _selectedAdditionalEquip;
        set => Set(ref _selectedAdditionalEquip, value);
    }

    // Новый электрический компонент
    public FormedElectricalComponent NewElectricalComponent
    {
        get => _newElectricalComponent;
        set => Set(ref _newElectricalComponent, value);
    }

    // Новое дополнительное комплектующее
    public FormedAdditionalEquip NewAdditionalEquip
    {
        get => _newAdditionalEquip;
        set => Set(ref _newAdditionalEquip, value);
    }

    // Все цели дренажей в стенде
    public ObservableCollection<DrainagePurpose> AllDrainagePurposesInStand
    {
        get => _allDrainagePurposesInStand;
        set => Set(ref _allDrainagePurposesInStand, value);
    }

    // Все цели электрических компонентов в стенде
    public ObservableCollection<ElectricalPurpose> AllElectricalPurposesInStand
    {
        get => _allElectricalPurposesInStand;
        set => Set(ref _allElectricalPurposesInStand, value);
    }

    // Все цели дополнительных комплектующих в стенде
    public ObservableCollection<AdditionalEquipPurpose> AllAdditionalEquipPurposesInStand
    {
        get => _allAdditionalEquipPurposesInStand;
        set => Set(ref _allAdditionalEquipPurposesInStand, value);
    }

    public StandSettingsModel DefaultStandSettings { get; set; } = new();

    public void InitializeDrainagePurposes()
    {
        NewDrainage = new FormedDrainage
        {
            Purposes = new ObservableCollection<DrainagePurpose>
            {
                new() { Purpose = "Основная труба" },
                new() { Purpose = "Патрубок", Quantity = (float)0.2},
                new() { Purpose = "Заглушка основной трубы", Quantity = 2 },
                new() { Purpose = "Кронштейн дренажа" },
                new() { Purpose = "Клапан" }
            }
        };
    }

    public void InitializeAdditionalEquip()
    {
        NewAdditionalEquip = new FormedAdditionalEquip
        {
            Purposes = new ObservableCollection<AdditionalEquipPurpose>
            {
                new() { Purpose = "Шильдик", Material = DefaultStandSettings.NamePlate },
                new() { Purpose = "Швеллер", Material = DefaultStandSettings.SteelChannel },
                new() { Purpose = "Хомуты" },
                new() { Purpose = "Табличка", Material = DefaultStandSettings.NameTable },
                new() { Purpose = "Кронштейны перепадников" }
            }
        };
    }

    public void InitializeElectricalComponent()
    {
        NewElectricalComponent = new FormedElectricalComponent
        {
            Purposes = new ObservableCollection<ElectricalPurpose>
            {
                new() { Purpose = "Клеммная коробка" },
                new() { Purpose = "Кабельные вводы" },
                new() { Purpose = "Сигнальный кабель", Material = DefaultStandSettings.SignalCable, Quantity = 2 },
                new() { Purpose = "Металлорукав" , Quantity = 2},
                new() { Purpose = "Кабель 6мм", Material = DefaultStandSettings.CabelSixMM },
                new() { Purpose = "Кабель 4мм", Material = DefaultStandSettings.CabelFourMM, Quantity = 2 },
                new() { Purpose = "Кронштейн коробки" }
            }
        };
    }

    public async Task InitializeDefaultPurposes()
    {
        await DefaultStandSettings.LoadStandsSettingsDataAsync();

        InitializeElectricalComponent();
        InitializeAdditionalEquip();
        InitializeDrainagePurposes();
    }
}
