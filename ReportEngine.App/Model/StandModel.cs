using ReportEngine.App.ViewModels;
using System.Collections.ObjectModel;

namespace ReportEngine.App.Model
{
    public class StandModel : BaseViewModel
    {
        private int _id;
        private int _projectId;
        private string _kksCode;
        private string _design;
        private int _devices;
        private string _braceType;
        private float _width;
        private string _serialNumber;
        private float _weight;
        private decimal _standSummCost;
        private int _obvyazkaType;
        private int _nn;
        private string _materialLine;
        private string _armature;
        private string _treeScoket;
        private string _kmch;
        private string _firstSensorType;
        private string _firstSensorMarkPlus;
        private string _firsSensorKksCode;
        private string _firstSensorMarkMinus;
        private string _designeStand;
        private ObservableCollection<ObvyazkaModel> _obvyazki;
        private ObvyazkaModel? _selectedObvyazka;


        public ObservableCollection<ObvyazkaModel> Obvyazki 
        { 
            get => _obvyazki;
            set => Set(ref _obvyazki, value);
        }
        public ObvyazkaModel? SelectedObvyazka
        {
            get => _selectedObvyazka;
            set => Set(ref _selectedObvyazka, value);
        }

        public IEnumerable<string> BraceSensor { get; } = new List<string> { "На кронштейне", "Швеллер" };
        public IEnumerable<string> SensorType { get; } = new List<string> { "Датчик перепада давления", "Манометр", "Датчик абсолютного давления", "Манометр электрокомпактный" };
        public int Id { get => _id; set => Set(ref _id, value); }
        public int ProjectId { get => _projectId; set => Set(ref _projectId, value); }
        public string KKSCode { get => _kksCode; set => Set(ref _kksCode, value); }
        public string Design { get => _design; set => Set(ref _design, value); }
        public int Devices { get => _devices; set => Set(ref _devices, value); }
        public string BraceType { get => _braceType; set => Set(ref _braceType, value); }
        public float Width { get => _width; set => Set(ref _width, value); }
        public string SerialNumber { get => _serialNumber; set => Set(ref _serialNumber, value); }
        public float Weight { get => _weight; set => Set(ref _weight, value); }
        public decimal StandSummCost { get => _standSummCost; set => Set(ref _standSummCost, value); }
        public int ObvyazkaType { get => _obvyazkaType; set => Set(ref _obvyazkaType, value); }
        public int NN { get => _nn; set => Set(ref _nn, value); }
        public string MaterialLine { get => _materialLine; set => Set(ref _materialLine, value); }
        public string Armature { get => _armature; set => Set(ref _armature, value); }
        public string TreeSocket { get => _treeScoket; set => Set(ref _treeScoket, value); }
        public string KMCH { get => _kmch; set => Set(ref _kmch, value); }
        public string FirstSensorType { get => _firstSensorType; set => Set(ref _firstSensorType, value); }
        public string? FirstSensorKKSCounter { get => _firsSensorKksCode; set => Set(ref _firsSensorKksCode, value); }//ККС Контура
        public string? FirstSensorMarkPlus { get => _firstSensorMarkPlus; set => Set(ref _firstSensorMarkPlus, value); } //Марикровка +
        public string? FirstSensorMarkMinus { get => _firstSensorMarkMinus; set => Set(ref _firstSensorMarkMinus, value); } //Марикровка -
        public string? DesigneStand { get => _designeStand; set => Set(ref _designeStand, value); } //Описание

    }
}
