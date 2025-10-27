using ReportEngine.Domain.Entities;
using System.Collections.ObjectModel;

namespace ReportEngine.App.Model.StandsModel;

public class StandDrainageModel
{
    public StandDrainageModel()
    {
    }

    public StandDrainageModel(FormedDrainage drainage)
    {
        Id = drainage.Id;
        Name = drainage.Name;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public float Cost { get; set; }
    public string Measure { get; set; }
    public ObservableCollection<DrainagePurposeModel> Purposes { get; set; } = new();

    public class DrainagePurposeModel
    {
        public string Purpose { get; set; }
        public string Material { get; set; }
        public float Quantity { get; set; }
    }
}