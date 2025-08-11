using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Drainage;
using ReportEngine.Domain.Entities.Frame;
using System.Collections.ObjectModel;

public class FormedDrainagesModel : BaseViewModel
{
    private ObservableCollection<Drainage> _drainageDetails = new();
    private ObservableCollection<FormedDrainage> _allFormedDrainage = new();
    private FormedDrainage _selectedFormedDrainage = new();
    private ObservableCollection<DrainagePurpose> _purposes = new();
    private DrainagePurpose _selectedPurpose = new();

    public ObservableCollection<FormedDrainage> AllFormedDrainage
    {
        get => _allFormedDrainage;
        set => Set(ref _allFormedDrainage, value);
    }
    public FormedDrainage SelectedFormedDrainage
    {
        get => _selectedFormedDrainage;
        set
        {
            Set(ref _selectedFormedDrainage, value);
            // При выборе дренажа обновляем коллекцию назначений
            Purposes = value?.Purposes != null
                ? new ObservableCollection<DrainagePurpose>(value.Purposes)
                : new ObservableCollection<DrainagePurpose>();
        }
    }
    public ObservableCollection<DrainagePurpose> Purposes
    {
        get => _purposes;
        set => Set(ref _purposes, value);
    }
    public DrainagePurpose SelectedPurpose
    {
        get => _selectedPurpose;
        set => Set(ref _selectedPurpose, value);
    }

    public FormedDrainage CreateNewFormedDrainage(string name)
    {
        return new FormedDrainage
        {
            Name = name
        };
    }

    public DrainagePurpose CreateNewPurpose(string purpose, string material, float? quantity)
    {
        return new DrainagePurpose
        {
            Purpose = purpose,
            Material = material,
            Quantity = quantity
        };
    }
}