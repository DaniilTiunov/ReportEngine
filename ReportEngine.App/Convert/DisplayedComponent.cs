using ReportEngine.Domain.Entities.BaseEntities.Interface;
using System.ComponentModel;

namespace ReportEngine.App.Convert
{
    public class DisplayedComponent : INotifyPropertyChanged
    {
        public IBaseEquip Component { get; set; }
        public int Count { get; set; }
        private float _length;
        public float Length
        {
            get => _length;
            set
            {
                if (_length != value)
                {
                    _length = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Length)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
