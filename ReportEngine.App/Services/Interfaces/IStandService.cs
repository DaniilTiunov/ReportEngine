using ReportEngine.App.Model.Container;
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
    Task AddCustomElectricalComponentAsync(int standId, FormedElectricalComponent customElectrical);
    Task AddCustomAdditionalEquipAsync(int standId, FormedAdditionalEquip customEquip);
    void LoadPurposesInStands(IEnumerable<StandModel> stands);
    Task<ObvyazkaInStand> CreateObvyazkaAsync(StandModel standModel, Obvyazka selectedObvyazka);
    Task UpdateElectricalPurposeAsync(ElectricalPurpose entity);
    Task DeleteElectricalPurposeAsync(int purposeId);
    Task UpdateAdditionalPurposeAsync(AdditionalEquipPurpose entity);
    Task DeleteAdditionalPurposeAsync(int purposeId);
    Task UpdateDrainagePurposeAsync(DrainagePurpose entity);
    Task DeleteDrainagePurposeAsync(int purposeId);

    Task<ContainerBatchModel> CreateBatchAsync(ContainerBatchModel batchModel);
    Task DeleteBatchAsync(int batchId);
    Task<IEnumerable<ContainerBatchModel>> GetBatchesByProjectAsync(int projectId);
    Task<ContainerBatchModel> GetBatchWithContainersAsync(int batchId);
    Task AddContainerToBatchAsync(int batchId, ContainerStandModel containerModel);
    Task RemoveContainerFromBatchAsync(int batchId, int containerId);
    Task DeleteContainerAsync(int containerId);
    Task AddStandToContainerAsync(int containerId, int standId);
    Task RemoveStandFromContainerAsync(int containerId, int standId);
}