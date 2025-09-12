using System.ComponentModel;
using ReportEngine.Domain.Entities.BaseEntities.Interface;

namespace ReportEngine.App.ModelWrappers;

public class DisplayedComponent : INotifyPropertyChanged
{
    private float _length;
    public IBaseEquip Component { get; set; }
    public int Count { get; set; }
    public float? CostComponent { get; set; }
    
    public string? Measure { get; set; }

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