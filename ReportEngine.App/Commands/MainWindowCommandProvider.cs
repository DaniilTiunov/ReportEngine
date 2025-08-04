using System.Windows.Input;

namespace ReportEngine.App.Commands
{
    public class MainWindowCommandProvider
    {
        public ICommand OpenMainWindowCommand { get; set; }
        public ICommand OpenAllObvyazkiCommand { get; set; }
        public ICommand OpenTreeViewCommand { get; set; }
        public ICommand CloseAppCommand { get; set; }
        public ICommand OpenAllUsersCommand { get; set; }
        public ICommand ChekDbConnectionCommand { get; set; }
        public ICommand ShowAllProjectsCommand { get; set; }
        public ICommand DeleteSelectedProjectCommand { get; set; }
        public ICommand EditProjectCommand { get; set; }

    }
}
