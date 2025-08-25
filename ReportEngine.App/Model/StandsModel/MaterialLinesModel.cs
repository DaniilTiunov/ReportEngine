using System.Collections.ObjectModel;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Model.StandsModel;

public class MaterialLinesModel : BaseViewModel
{
    public ObservableCollection<string> MaterialLineTypes { get; } = new() { "Жаропрочные", "Нержавеющие", "Углеродистые" };
    private string _selectedMaterialLine = "Жаропрочные";
    public string SelectedMaterialLine
    {
        get => _selectedMaterialLine;
        set => Set(ref _selectedMaterialLine, value);
    }
    
    public ObservableCollection<string> AramuteresTypes { get; } = new() { "Жаропрочные", "Нержавеющие", "Углеродистые" };
    private string _selectedAramuteres= "Жаропрочные";
    public string SelectedAramuteres
    {
        get => _selectedAramuteres;
        set => Set(ref _selectedAramuteres, value);
    }
    
    public ObservableCollection<string> TreeSocketTypes{ get; } = new() { "Жаропрочные", "Нержавеющие", "Углеродистые" };
    private string _selectedSocketTypes = "Жаропрочные";
    public string SelectedSocketTypes
    {
        get => _selectedSocketTypes;
        set => Set(ref _selectedSocketTypes, value);
    }
    
    public ObservableCollection<string> KMCHTypes { get; } = new() { "Жаропрочные", "Нержавеющие", "Углеродистые" };
    private string _selectedKMCHType = "Жаропрочные";
    public string SelectedKMCHType
    {
        get => _selectedKMCHType;
        set => Set(ref _selectedKMCHType, value);
    }
}