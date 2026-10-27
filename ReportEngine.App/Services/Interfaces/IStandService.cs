using ReportEngine.App.Model.StandsModel;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.Services.Interfaces;

public interface IStandService
{
    Task LoadStandsDataAsync(IEnumerable<StandModel> standModels);

    Task AddFrameToStandAsync(int standId, int frameId);
    Task AddFrameToStandByNameAsync(int standId, string frameName);

    Task AddDrainageToStandAsync(int standId, int drainageId);

    Task AddObvyazkaToStandAsync(int standId, ObvyazkaInStand obvyazka);

    Task LoadObvyazkiInStandsAsync(IEnumerable<StandModel> standModels);

    Task<IEnumerable<FormedFrame>> LoadAllAvailableFrameAsync();

    Task<IEnumerable<FormedDrainage>> LoadAllAvailableDrainagesAsync();

    Task<IEnumerable<FormedElectricalComponent>> LoadAllAvailableElectricalComponentsAsync();

    Task<IEnumerable<FormedAdditionalEquip>> LoadAllAvailableAdditionalEquipsAsync();

    Task AddCustomDrainageAsync(int standId, IEnumerable<DrainagePurpose> drainagePurposes);

    Task AddCustomElectricalComponentAsync(int standId, IEnumerable<ElectricalPurpose> electricalPurposes);

    Task AddCustomAdditionalEquipAsync(int standId, IEnumerable<AdditionalEquipPurpose> additionalEquipPurposes);

    Task LoadPurposesInStands(IEnumerable<StandModel> stands);

    Task<ObvyazkaInStand> CreateObvyazkaAsync(StandModel standModel, Obvyazka selectedObvyazka);

    Task UpdateElectricalPurposeAsync(ElectricalPurpose entity);

    Task DeleteElectricalPurposeAsync(int purposeId);

    Task UpdateAdditionalPurposeAsync(AdditionalEquipPurpose entity);

    Task DeleteAdditionalPurposeAsync(int purposeId);

    Task UpdateDrainagePurposeAsync(DrainagePurpose entity);

    Task DeleteDrainagePurposeAsync(int purposeId);

    Task FillStandFieldsFromObvyazka(StandModel stand, ObvyazkaInStand obv);

    Task UpdateStandWeight(StandModel stand);

    Task DeleteAdditionalPurposeFromObvAsync(ObvyazkaAdditionalEquipPurpose obv, StandModel standModel);

    Task UpdateAdditionalPurposeFromObvAsync(ObvyazkaAdditionalEquipPurpose obv, int obvyazkaInStand);

    Task SaveAllPurposesInStandAsync(StandModel stand);
}
