using ReportEngine.App.Model.StandsModel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.Services.Interfaces;

public interface IStandService
{
    Task LoadStandsDataAsync(IEnumerable<StandModel> standModels);

    Task LoadAllStandsDataAsync(int projectId, IEnumerable<StandModel> standModels);

    Task AddFrameToStandAsync(int standId, int frameId);

    Task AddDrainageToStandAsync(int standId, int drainageId);

    Task AddObvyazkaToStandAsync(int standId, ObvyazkaInStand obvyazka);

    Task LoadObvyazkiInStandsAsync(IEnumerable<StandModel> standModels);

    Task<IEnumerable<FormedFrame>> LoadAllAvailableFrameAsync();

    Task<IEnumerable<FormedDrainage>> LoadAllAvailableDrainagesAsync();

    Task<IEnumerable<FormedElectricalComponent>> LoadAllAvailableElectricalComponentsAsync();

    Task<IEnumerable<FormedAdditionalEquip>> LoadAllAvailableAdditionalEquipsAsync();

    Task AddCustomDrainageAsync(int standId, List<DrainagePurpose> drainagePurposes, FormedDrainage customDrainage);

    Task AddCustomElectricalComponentAsync(int standId, List<ElectricalPurpose> electricalPurpose, FormedElectricalComponent customElectrical);

    Task AddCustomAdditionalEquipAsync(int standId, List<AdditionalEquipPurpose> additionalEquipPurposes, FormedAdditionalEquip customEquip);

    Task LoadPurposesInStands(IEnumerable<StandModel> stands);

    Task<ObvyazkaInStand> CreateObvyazkaAsync(StandModel standModel, Obvyazka selectedObvyazka);

    Task UpdateElectricalPurposeAsync(ElectricalPurpose entity);

    Task DeleteElectricalPurposeAsync(int purposeId);

    Task UpdateAdditionalPurposeAsync(AdditionalEquipPurpose entity);

    Task DeleteAdditionalPurposeAsync(int purposeId);

    Task UpdateDrainagePurposeAsync(DrainagePurpose entity);

    Task DeleteDrainagePurposeAsync(int purposeId);

    void FillStandFieldsFromObvyazka(StandModel stand, ObvyazkaInStand obv);

    Task UpdateStandWeight(StandModel stand);

    Task DeleteAdditionalPurposeFromObvAsync(ObvyazkaAdditionalEquipPurpose obv, StandModel standModel);

    Task UpdateAdditionalPurposeFromObvAsync(ObvyazkaAdditionalEquipPurpose obv, int obvyazkaInStand);

    Task SaveAllPurposesInStandAsync(StandModel stand);
}
