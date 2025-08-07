using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.ViewModels
{
    public class FormedFrameViewModel : BaseViewModel
    {
        private readonly IFrameRepository _formedFrameRepository;

        public FormedFrameViewModel(IFrameRepository formedFrameRepository)
        {
            _formedFrameRepository = formedFrameRepository;
        }
    }
}
