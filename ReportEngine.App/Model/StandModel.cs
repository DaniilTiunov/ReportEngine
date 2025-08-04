using ReportEngine.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.App.Model
{
    public class StandModel : BaseViewModel
    {
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


        public IEnumerable<string> BraceSensor { get; } = new List<string> { "На кронштейне", "Швеллер" };
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
        public string TreeScoket { get => _treeScoket; set => Set(ref _treeScoket, value); }
        public string KMCH { get => _kmch; set => Set(ref _kmch, value); }
       
    }
}
