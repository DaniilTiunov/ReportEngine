using System.Windows.Input;

namespace ReportEngine.AtomicApp.Commands.Providers;

public class ProjectCommandProvider
{
    public ICommand GetProjectAsyncCommand { get; set; }

    public ICommand AddNewProjectAsyncCommand { get; set; }
}
