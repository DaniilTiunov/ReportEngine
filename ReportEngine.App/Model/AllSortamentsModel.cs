using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using System.Collections.ObjectModel;

namespace ReportEngine.App.Model;

public class AllSortamentsModel : BaseViewModel
{
    public Dictionary<string, ObservableCollection<IBaseEquip>> EquipGroups { get; } = new();

    public void SetEquipGroup<T>(string groupName, IEnumerable<T> items)
        where T : IBaseEquip
    {
        EquipGroups[groupName] = new ObservableCollection<IBaseEquip>(items.Cast<IBaseEquip>());
        OnPropertyChanged(nameof(EquipGroups));
    }

}