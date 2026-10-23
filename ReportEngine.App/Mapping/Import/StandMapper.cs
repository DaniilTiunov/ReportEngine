using Mapster;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.Import.Dto;

namespace ReportEngine.App.Mapping.Import;

public class StandMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<StandDto, StandModel>()
            .Map(
                dest => dest.AllAdditionalEquipPurposesInStand,
                src => src.AdditionalPurposes)
            .Map(
                dest => dest.AllElectricalPurposesInStand,
                src => src.ElectricalPurposes)
            .Map(
                dest => dest.AllDrainagePurposesInStand,
                src => src.DrainagePurposes);
    }
}