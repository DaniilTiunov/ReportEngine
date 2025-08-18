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
    Task LoadObvyazkiInStandsAsync(IEnumerable<StandModel> standModels);
    Task<IEnumerable<FormedFrame>> LoadAllAvailableFrameAsync();
    Task<IEnumerable<FormedDrainage>> LoadAllAvailableDrainagesAsync();
    Task<IEnumerable<FormedElectricalComponent>> LoadAllAvailableElectricalComponentsAsync();
    Task<IEnumerable<FormedAdditionalEquip>> LoadAllAvailableAdditionalEquipsAsync();
    Task AddElectricalComponentToStandAsync(int standId, int electricalComponentId);
    Task AddAdditionalEquipToStandAsync(int standId, int additionalEquipId);
}