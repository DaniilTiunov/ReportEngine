using System.Collections.ObjectModel;
using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.Model;

public class ObvyazkaModel : BaseViewModel
{
    private ObservableCollection<Obvyazka> _obvyazki = new();
    private Obvyazka _selectedObvyazka = new();


    public ObservableCollection<Obvyazka> Obvyazki
    {
        get => _obvyazki;
        set => Set(ref _obvyazki, value);
    }

    public Obvyazka SelectedObvyazka
    {
        get => _selectedObvyazka;
        set => Set(ref _selectedObvyazka, value);
    }
}