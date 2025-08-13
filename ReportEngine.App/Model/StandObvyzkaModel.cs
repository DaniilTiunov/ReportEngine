using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Model
{
    public class StandObvyazkaModel : BaseViewModel
    {
        public int ObvyazkaId { get; set; }
        public string ObvyazkaName { get; set; }
        public string MaterialLine { get; set; }
        public string TreeSocket { get; set; }
        public string KMCH { get; set; }

        public string? FirstSensorType { get; set; }
        public string? FirstSensorKKS { get; set; }
        public string? FirstSensorMarkPlus { get; set; }
        public string? FirstSensorMarkMinus { get; set; }

        public string? SecondSensorType { get; set; }
        public string? SecondSensorKKS { get; set; }
        public string? SecondSensorMarkPlus { get; set; }
        public string? SecondSensorMarkMinus { get; set; }

        public string? ThirdSensorType { get; set; }
        public string? ThirdSensorKKS { get; set; }
        public string? ThirdSensorMarkPlus { get; set; }
        public string? ThirdSensorMarkMinus { get; set; }
    }
}