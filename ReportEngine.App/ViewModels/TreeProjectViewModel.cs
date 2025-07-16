using ReportEngine.App.Services;

namespace ReportEngine.App.ViewModels
{
    public class TreeProjectViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly NavigationService _navigation;
        
        public TreeProjectViewModel(NavigationService navigation, IServiceProvider serviceProvider)
        {
            _navigation = navigation;
            _serviceProvider = serviceProvider;
        }
    }
}
