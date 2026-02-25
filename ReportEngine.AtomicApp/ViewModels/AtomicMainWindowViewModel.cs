using ReportEngine.AtomicApp.Services;
using ReportEngine.AtomicApp.ViewModels.Abstracts;

namespace ReportEngine.AtomicApp.ViewModels
{
    public class AtomicMainWindowViewModel : BaseViewModel
    {
        private readonly NavigationService _navigationService;
        private readonly IServiceProvider _serviceProvider;

        public AtomicMainWindowViewModel(
            NavigationService navigationService,
            IServiceProvider serviceProvider)
        {
            _navigationService = navigationService;
            _serviceProvider = serviceProvider;
        }

    }
}
