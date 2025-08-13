using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Model.StandsModel
{
    public class StandObvyazkaModel : BaseViewModel
    {
        public int ObvyazkaId { get; set; }
        public string ObvyazkaName { get; set; }
        public string MaterialLine { get; set; }
        public string TreeSocket { get; set; }
        public string KMCH { get; set; }
        public string? Armature { get; set; }
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

        public static StandObvyazkaModel Create(
            int obvyazkaId,
            string obvyazkaName,
            string materialLine,
            string treeSocket,
            string kmch,
            string? firstSensorType = null,
            string? firstSensorKKS = null,
            string? firstSensorMarkPlus = null,
            string? firstSensorMarkMinus = null,
            string? secondSensorType = null,
            string? secondSensorKKS = null,
            string? secondSensorMarkPlus = null,
            string? secondSensorMarkMinus = null,
            string? thirdSensorType = null,
            string? thirdSensorKKS = null,
            string? thirdSensorMarkPlus = null,
            string? thirdSensorMarkMinus = null
        )
        {
            return new StandObvyazkaModel
            {
                ObvyazkaId = obvyazkaId,
                ObvyazkaName = obvyazkaName,
                MaterialLine = materialLine,
                TreeSocket = treeSocket,
                KMCH = kmch,
                FirstSensorType = firstSensorType,
                FirstSensorKKS = firstSensorKKS,
                FirstSensorMarkPlus = firstSensorMarkPlus,
                FirstSensorMarkMinus = firstSensorMarkMinus,
                SecondSensorType = secondSensorType,
                SecondSensorKKS = secondSensorKKS,
                SecondSensorMarkPlus = secondSensorMarkPlus,
                SecondSensorMarkMinus = secondSensorMarkMinus,
                ThirdSensorType = thirdSensorType,
                ThirdSensorKKS = thirdSensorKKS,
                ThirdSensorMarkPlus = thirdSensorMarkPlus,
                ThirdSensorMarkMinus = thirdSensorMarkMinus
            };
        }
    }
}