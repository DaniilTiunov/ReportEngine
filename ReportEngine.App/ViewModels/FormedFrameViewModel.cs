using ReportEngine.App.Model;
using ReportEngine.Domain.Entities.Frame;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;
using System.Collections.ObjectModel;

namespace ReportEngine.App.ViewModels
{
    public class FormedFrameViewModel : BaseViewModel
    {
        private readonly IFrameRepository _formedFrameRepository;
        private readonly IGenericBaseRepository<FrameDetail, FrameDetail> _frameDetailRepository;
        private readonly IGenericBaseRepository<FrameRoll, FrameRoll> _frameRollRepository;
        private readonly IGenericBaseRepository<PillarEqiup, PillarEqiup> _pillarEqiupRepository;

        public FormedFrameModel FormedFrameModel { get; } = new();

        public FormedFrameViewModel(
            IFrameRepository formedFrameRepository,
            IGenericBaseRepository<FrameDetail, FrameDetail> frameDetailRepository,
            IGenericBaseRepository<FrameRoll, FrameRoll> frameRollRepository,
            IGenericBaseRepository<PillarEqiup, PillarEqiup> pillarEqiupRepository)
        {
            _frameDetailRepository = frameDetailRepository;
            _frameRollRepository = frameRollRepository;
            _pillarEqiupRepository = pillarEqiupRepository;
            _formedFrameRepository = formedFrameRepository;

            LoadDetailsData();
        }

        public async void LoadDetailsData()
        {
            await ExceptionHelper.SafeExecuteAsync( async () => {
                var details = await _frameDetailRepository.GetAllAsync();
                FormedFrameModel.FrameDetails = new ObservableCollection<FrameDetail>(details);
            });
        }
    }
}
