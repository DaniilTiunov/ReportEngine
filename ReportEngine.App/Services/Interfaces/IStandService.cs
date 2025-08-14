using ReportEngine.App.Model.StandsModel;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.Services.Interfaces;

public interface IStandService
{
    Task LoadStandDataAsync(StandModel standModel);
    Task AddFrameToStandAsync(int standId, FormedFrame frame);
    Task AddDrainageToStandAsync(int standId, FormedDrainage drainage);
    Task AddCustomDrainageAsync(int standId, FormedDrainage customDrainage);
    Task AddObvyazkaToStandAsync(int standId, ObvyazkaInStand obvyazka);
}