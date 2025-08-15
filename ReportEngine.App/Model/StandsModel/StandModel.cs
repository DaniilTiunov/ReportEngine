using System.Collections.ObjectModel;
using ReportEngine.App.Model.FormedEquipsModels;
using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.Model.StandsModel;

public class StandModel : BaseViewModel
{
    private ObservableCollection<FormedDrainage> _allAvailableDrainages = new();
    private ObservableCollection<FormedFrame> _allAvailableFrames = new();
    private string _armature;
    private string _braceType;
    private string _design;
    private string _designeStand;
    private int _devices;
    private ObservableCollection<FormedDrainage> _drainagesInStand = new();
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

    private FormedDrainage _newDrainage = new();
    private int _nn;
    private int _obvyazkaType;

    private ObservableCollection<StandObvyazkaModel> _obvyazki = new();
    private ObservableCollection<ObvyazkaInStand> _obvyazkiInStand = new();
    private int _projectId;
    private FormedDrainage _selectedDrainage;
    private FormedFrame _selectedFrame;
    private StandObvyazkaModel _selectedObvyazka;
    private string _serialNumber;
    private decimal _standSummCost;
    private string _treeScoket;
    private float _weight;
    private float _width;

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

    public int ObvyazkaType
    {
        get => _obvyazkaType;
        set => Set(ref _obvyazkaType, value);
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

    public FormedDrainage NewDrainage
    {
        get => _newDrainage;
        set => Set(ref _newDrainage, value);
    }

    public FormedFrameModel FormedFrameComponents
    {
        get => _formedFrameComponents;
        set => Set(ref _formedFrameComponents, value);
    }
}