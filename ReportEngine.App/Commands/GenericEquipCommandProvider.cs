using System.Windows.Input;

namespace ReportEngine.App.Commands
{
    public class GenericEquipCommandProvider
    {
        public ICommand OpenCarbonPipeCommand { get; set; }
        public ICommand OpenHeaterPipeCommand { get; set; }
        public ICommand OpenStainlessPipeCommand { get; set; }
        public ICommand OpenCarbonArmatureCommand { get; set; }
        public ICommand OpenHeaterArmatureCommand { get; set; }
        public ICommand OpenStainlessArmatureCommand { get; set; }
        public ICommand OpenCarbonSocketsCommand { get; set; }
        public ICommand OpenStainlessSocketsCommand { get; set; }
        public ICommand OpenHeaterSocketsCommand { get; set; }
        public ICommand OpenDrainageCommand { get; set; }
        public ICommand OpenGenericWindowCommand { get; set; }
        public ICommand OpenFrameDetailsCommand { get; set; }
    }
}
