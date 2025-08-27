using System.Collections.ObjectModel;
using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.Pipes;

namespace ReportEngine.App.Model;

public class AllSortamentsModel : BaseViewModel
{
    private ObservableCollection<HeaterPipe> _allHeaterPipes = new();
    private ObservableCollection<StainlessPipe> _allStainlessPipes = new();
    private ObservableCollection<CarbonPipe> _allCarbonPipes = new();
    private ObservableCollection<IBaseEquip> _currentSortaments = new();

    public ObservableCollection<HeaterPipe> AllHeaterPipes
    {
        get => _allHeaterPipes;
        set => Set(ref _allHeaterPipes, value);
    }

    public ObservableCollection<StainlessPipe> AllStainlessPipes
    {
        get => _allStainlessPipes;
        set => Set(ref _allStainlessPipes, value);
    }

    public ObservableCollection<CarbonPipe> AllCarbonPipes
    {
        get => _allCarbonPipes;
        set => Set(ref _allCarbonPipes, value);
    }
    
    public ObservableCollection<IBaseEquip> CurrentSortaments
    {
        get => _currentSortaments;
        set => Set(ref _currentSortaments, value);
    }
    
    public void SetCurrentSortaments<T>(ObservableCollection<T> collection) where T : IBaseEquip
    {
        CurrentSortaments = new ObservableCollection<IBaseEquip>(collection.Cast<IBaseEquip>());
    }
}