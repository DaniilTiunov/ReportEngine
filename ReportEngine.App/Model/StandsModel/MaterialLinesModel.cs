using ReportEngine.App.ViewModels;
using System.Collections.ObjectModel;

namespace ReportEngine.App.Model.StandsModel;

public class MaterialLinesModel : BaseViewModel
{
    private string _selectedAramuteres = "Углеродистые";
    private string _selectedKMCHType = "Углеродистые";
    private string _selectedMaterialLine = "Углеродистые";
    private string _selectedSocketTypes = "Углеродистые";

    public ObservableCollection<string> MaterialLineTypes { get; } =
        new() { "Жаропрочные", "Нержавеющие", "Углеродистые" };

    public string SelectedMaterialLine
    {
        get => _selectedMaterialLine;
        set => Set(ref _selectedMaterialLine, value);
    }

    public ObservableCollection<string> AramuteresTypes { get; } =
        new() { "Жаропрочные", "Нержавеющие", "Углеродистые" };

    public string SelectedAramuteres
    {
        get => _selectedAramuteres;
        set => Set(ref _selectedAramuteres, value);
    }

    public ObservableCollection<string> TreeSocketTypes { get; } =
        new() { "Жаропрочные", "Нержавеющие", "Углеродистые" };

    public string SelectedSocketTypes
    {
        get => _selectedSocketTypes;
        set => Set(ref _selectedSocketTypes, value);
    }

    public ObservableCollection<string> KMCHTypes { get; } = new() { "Жаропрочные", "Нержавеющие", "Углеродистые" };

    public string SelectedKMCHType
    {
        get => _selectedKMCHType;
        set => Set(ref _selectedKMCHType, value);
    }
}
