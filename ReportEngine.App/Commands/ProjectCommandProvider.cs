using System.Windows.Input;

namespace ReportEngine.App.Commands
{
    public class ProjectCommandProvider
    {
        public ICommand SelectFromDialogCommand { get; set; }
        public ICommand CreateNewCardCommand { get; set; }
        public ICommand AddNewStandCommand { get; set; }
        public ICommand SaveChangesCommand { get; set; }

    }
}
