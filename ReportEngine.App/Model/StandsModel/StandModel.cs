using System.Collections.ObjectModel;
using ReportEngine.App.Model.FormedEquipsModels;
using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.Model.StandsModel;

public class StandModel : BaseViewModel
{
    private ObservableCollection<FormedAdditionalEquip> _additionalEquipsInStand = new();
    private ObservableCollection<AdditionalEquipPurpose> _allAdditionalEquipPurposesInStand = new();
    private ObservableCollection<FormedDrainage> _allAvailableDrainages = new();
    private ObservableCollection<FormedFrame> _allAvailableFrames = new();

    private ObservableCollection<DrainagePurpose> _allDrainagePurposesInStand = new();
    private ObservableCollection<ElectricalPurpose> _allElectricalPurposesInStand = new();
    private string _armature;
    private string _braceType;
    private string _comments;
    private string _design;
    private string _designeStand;
    private int _devices;
    private ObservableCollection<FormedDrainage> _drainagesInStand = new();


    private ObservableCollection<FormedElectricalComponent> _electricalComponentsInStand = new();
    private string _firsSensorKksCode;
    private string _firstSensorMarkMinus;
    private string _firstSensorMarkPlus;
    private string _firstSensorType;

    private FormedFrameModel _formedFrameComponents = new();


    private ObservableCollection<FormedFrame> _framesInStand = new();
    private int _id;
    private string _kksCode;
    private string _kmch;
    private string _materialLine;
    private FormedAdditionalEquip _newAdditionalEquip = new();

    private FormedDrainage _newDrainage = new();
    private FormedElectricalComponent _newElectricalComponent = new();
    private int _nn;
    private string _obvyazkaName;

    private ObservableCollection<StandObvyazkaModel> _obvyazki = new();
    private ObservableCollection<ObvyazkaInStand> _obvyazkiInStand = new();
    private int _projectId;
    private string? _secondSensorKksCode;
    private string? _secondSensorMarkMinus;
    private string? _secondSensorMarkPlus;
    private string _secondSensorType;
    private FormedAdditionalEquip _selectedAdditionalEquip;
    private FormedDrainage _selectedDrainage;
    private FormedElectricalComponent _selectedElectricalComponent;
    private FormedFrame _selectedFrame;
    private StandObvyazkaModel _selectedObvyazka;

    private ObvyazkaInStand _selectedObvyazkaInStand = new();
    private string _serialNumber;
    private decimal _standSummCost;
    private string? _thirdSensorKksCode;
    private string? _thirdSensorMarkMinus;
    private string? _thirdSensorMarkPlus;
    private string _thirdSensorType;
    private string _treeScoket;
    private float _weight;
    private float _width;

    public StandModel()
    {
        InitializeDefaultPurposes();
    }

    public ObservableCollection<StandObvyazkaModel> Obvyazki
    {
        get => _obvyazki;
        set => Set(ref _obvyazki, value);
    }

    public StandObvyazkaModel SelectedObvyazka
    {
        get => _selectedObvyazka;
        set => Set(ref _selectedObvyazka, value);
    }

    public IEnumerable<string> BraceSensor { get; } = new List<string> { "На кронштейне", "Швеллер" };

    public IEnumerable<string> SensorType { get; } = new List<string>
        { "Датчик перепада давления", "Манометр", "Датчик абсолютного давления", "Манометр электрокомпактный" };

    public int Id
    {
        get => _id;
        set => Set(ref _id, value);
    }

    public int ProjectId
    {
        get => _projectId;
        set => Set(ref _projectId, value);
    }

    public string KKSCode
    {
        get => _kksCode;
        set => Set(ref _kksCode, value);
    }

    public string Design
    {
        get => _design;
        set => Set(ref _design, value);
    }

    public int Devices
    {
        get => _devices;
        set => Set(ref _devices, value);
    }

    public string Comments
    {
        get => _comments;
        set => Set(ref _comments, value);
    }

    public string BraceType
    {
        get => _braceType;
        set => Set(ref _braceType, value);
    }

    public float Width
    {
        get => _width;
        set => Set(ref _width, value);
    }

    public string SerialNumber
    {
        get => _serialNumber;
        set => Set(ref _serialNumber, value);
    }

    public float Weight
    {
        get => _weight;
        set => Set(ref _weight, value);
    }

    public decimal StandSummCost
    {
        get => _standSummCost;
        set => Set(ref _standSummCost, value);
    }

    public string ObvyazkaName
    {
        get => _obvyazkaName;
        set => Set(ref _obvyazkaName, value);
    }

    public int NN
    {
        get => _nn;
        set => Set(ref _nn, value);
    }

    public string MaterialLine
    {
        get => _materialLine;
        set => Set(ref _materialLine, value);
    }

    public string Armature
    {
        get => _armature;
        set => Set(ref _armature, value);
    }

    public string TreeSocket
    {
        get => _treeScoket;
        set => Set(ref _treeScoket, value);
    }

    public string KMCH
    {
        get => _kmch;
        set => Set(ref _kmch, value);
    }

    public string FirstSensorType
    {
        get => _firstSensorType;
        set => Set(ref _firstSensorType, value);
    }

    public string? FirstSensorKKSCounter
    {
        get => _firsSensorKksCode;
        set => Set(ref _firsSensorKksCode, value);
    } //ККС Контура

    public string? FirstSensorMarkPlus
    {
        get => _firstSensorMarkPlus;
        set => Set(ref _firstSensorMarkPlus, value);
    } //Марикровка +

    public string? FirstSensorMarkMinus
    {
        get => _firstSensorMarkMinus;
        set => Set(ref _firstSensorMarkMinus, value);
    } //Марикровка -

    public string SecondSensorType
    {
        get => _secondSensorType;
        set => Set(ref _secondSensorType, value);
    }

    public string? SecondSensorKKSCounter
    {
        get => _secondSensorKksCode;
        set => Set(ref _secondSensorKksCode, value);
    } //ККС Контура

    public string? SecondSensorMarkPlus
    {
        get => _secondSensorMarkPlus;
        set => Set(ref _secondSensorMarkPlus, value);
    } //Марикровка +

    public string? SecondSensorMarkMinus
    {
        get => _secondSensorMarkMinus;
        set => Set(ref _secondSensorMarkMinus, value);
    } //Марикровка -

    public string ThirdSensorType
    {
        get => _thirdSensorType;
        set => Set(ref _thirdSensorType, value);
    }

    public string? ThirdSensorKKS
    {
        get => _thirdSensorKksCode;
        set => Set(ref _thirdSensorKksCode, value);
    } //ККС Контура

    public string? ThirdSensorMarkPlus
    {
        get => _thirdSensorMarkPlus;
        set => Set(ref _thirdSensorMarkPlus, value);
    } //Марикровка +

    public string? ThirdSensorMarkMinus
    {
        get => _thirdSensorMarkMinus;
        set => Set(ref _thirdSensorMarkMinus, value);
    } //Марикровка -

    public string? DesigneStand
    {
        get => _designeStand;
        set => Set(ref _designeStand, value);
    } //Описание

    public ObservableCollection<FormedFrame> AllAvailableFrames
    {
        get => _allAvailableFrames;
        set => Set(ref _allAvailableFrames, value);
    }

    public ObservableCollection<FormedFrame> FramesInStand
    {
        get => _framesInStand;
        set => Set(ref _framesInStand, value);
    }

    public ObservableCollection<FormedDrainage> AllAvailableDrainages
    {
        get => _allAvailableDrainages;
        set => Set(ref _allAvailableDrainages, value);
    }

    public ObservableCollection<FormedDrainage> DrainagesInStand
    {
        get => _drainagesInStand;
        set => Set(ref _drainagesInStand, value);
    }

    public ObservableCollection<ObvyazkaInStand> ObvyazkiInStand
    {
        get => _obvyazkiInStand;
        set => Set(ref _obvyazkiInStand, value);
    }

    public FormedDrainage SelectedDrainage
    {
        get => _selectedDrainage;
        set => Set(ref _selectedDrainage, value);
    }

    public FormedFrame SelectedFrame
    {
        get => _selectedFrame;
        set => Set(ref _selectedFrame, value);
    }

    public ObvyazkaInStand SelectedObvyazkaInStand
    {
        get => _selectedObvyazkaInStand;
        set => Set(ref _selectedObvyazkaInStand, value);
    }

    public FormedDrainage NewDrainage
    {
        get => _newDrainage;
        set => Set(ref _newDrainage, value);
    }

    public ObservableCollection<FormedElectricalComponent> ElectricalComponentsInStand
    {
        get => _electricalComponentsInStand;
        set => Set(ref _electricalComponentsInStand, value);
    }

    public ObservableCollection<FormedAdditionalEquip> AdditionalEquipsInStand
    {
        get => _additionalEquipsInStand;
        set => Set(ref _additionalEquipsInStand, value);
    }

    public FormedElectricalComponent SelectedElectricalComponent
    {
        get => _selectedElectricalComponent;
        set => Set(ref _selectedElectricalComponent, value);
    }

    public FormedAdditionalEquip SelectedAdditionalEquip
    {
        get => _selectedAdditionalEquip;
        set => Set(ref _selectedAdditionalEquip, value);
    }

    public FormedElectricalComponent NewElectricalComponent
    {
        get => _newElectricalComponent;
        set => Set(ref _newElectricalComponent, value);
    }

    public FormedAdditionalEquip NewAdditionalEquip
    {
        get => _newAdditionalEquip;
        set => Set(ref _newAdditionalEquip, value);
    }

    public ObservableCollection<DrainagePurpose> AllDrainagePurposesInStand
    {
        get => _allDrainagePurposesInStand;
        set => Set(ref _allDrainagePurposesInStand, value);
    }

    public ObservableCollection<ElectricalPurpose> AllElectricalPurposesInStand
    {
        get => _allElectricalPurposesInStand;
        set => Set(ref _allElectricalPurposesInStand, value);
    }

    public ObservableCollection<AdditionalEquipPurpose> AllAdditionalEquipPurposesInStand
    {
        get => _allAdditionalEquipPurposesInStand;
        set => Set(ref _allAdditionalEquipPurposesInStand, value);
    }
    
    public void InitializeDefaultPurposes()
    {
        NewDrainage = new FormedDrainage
        {
            Purposes = new ObservableCollection<DrainagePurpose>
            {
                new() { Purpose = "Основная труба" },
                new() { Purpose = "Патрубок" },
                new() { Purpose = "Заглушка основной трубы" },
                new() { Purpose = "Кронштейн дренажа" },
                new() { Purpose = "Клапан" }
            }
        };

        NewAdditionalEquip = new FormedAdditionalEquip
        {
            Purposes = new ObservableCollection<AdditionalEquipPurpose>
            {
                new() { Purpose = "Клеммная коробка" },
                new() { Purpose = "Кабельные вводы" },
                new() { Purpose = "Сигнальный кабель" },
                new() { Purpose = "Металлорукав" },
                new() { Purpose = "Кабель 6мм" },
                new() { Purpose = "Кабель 4мм" },
                new() { Purpose = "Кронштейн коробки" }
            }
        };

        NewElectricalComponent = new FormedElectricalComponent
        {
            Purposes = new ObservableCollection<ElectricalPurpose>
            {
                new() { Purpose = "Шильдик" },
                new() { Purpose = "Швеллер" },
                new() { Purpose = "Хомуты" },
                new() { Purpose = "Табличка" },
                new() { Purpose = "Кронштейны перепадников" }
            }
        };
    }
}