using ReportEngine.Domain.Entities;

namespace ReportEngine.Domain.Repositories.Interfaces;

public interface IProjectInfoRepository : IBaseRepository<ProjectInfo>
{
    Task<Stand> AddStandAsync(int projectId, Stand stand);
    Task DeleteStandAsync(int projectId, int standId);
    Task UpdateStandAsync(Stand stand);
    Task<ProjectInfo> GetStandsByIdAsync(int projectId);
    Task AddStandObvyazkaAsync(int standId, ObvyazkaInStand standObvyazka);
    Task AddFrameToStandAsync(int standId, int frameId);
    Task<IEnumerable<StandFrame>> GetAllFramesInStandAsync(int standId);
    Task AddDrainageToStandAsync(int standId, int drainageId);
    Task<IEnumerable<StandDrainage>> GetAllDrainagesInStandAsync(int standId);
    Task<IEnumerable<ObvyazkaInStand>> GetAllObvyazkiInStandAsync(int standId);
    Task AddAdditionalEquipToStandAsync(int standId, int additionalEquipId);
    Task AddElectricalComponentToStandAsync(int standId, int electricalComponentId);
    Task<IEnumerable<StandElectricalComponent>> GetAllElectricalComponentsInStandAsync(int standId);
    Task<IEnumerable<StandAdditionalEquip>> GetAllAdditionalEquipsInStandAsync(int standId);
    Task DeleteObvFromStandAsync(int standId, int obvyazkaInStandId);
    Task DeleteFrameFromStandAsync(int standId, int standFrameId);
}