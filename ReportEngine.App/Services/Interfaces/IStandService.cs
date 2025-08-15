using ReportEngine.App.Model.StandsModel;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.Services.Interfaces;

public interface IStandService
{
    Task LoadStandsDataAsync(IEnumerable<StandModel> standModels);
    Task AddFrameToStandAsync(int standId, int frameId);
    Task AddDrainageToStandAsync(int standId, int drainageId);
    Task AddCustomDrainageAsync(int standId, FormedDrainage customDrainage);
    Task AddObvyazkaToStandAsync(int standId, ObvyazkaInStand obvyazka);
    Task LoadObvyazkiInStandAsync(StandModel standModel);
}