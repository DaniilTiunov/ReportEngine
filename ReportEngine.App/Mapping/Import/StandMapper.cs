using Mapster;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.Domain.Entities;
using ReportEngine.Import.Dto;

namespace ReportEngine.App.Mapping.Import;

public class StandMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<FrameDto, FormedFrame>()
            .Map(dest => dest.Designe, src => src.Description);
        
        config.NewConfig<StandDto, StandModel>()
            .Map(
                dest => dest.AllAdditionalEquipPurposesInStand,
                src => src.AdditionalPurposes)
            .Map(
                dest => dest.AllElectricalPurposesInStand,
                src => src.ElectricalPurposes)
            .Map(
                dest => dest.AllDrainagePurposesInStand,
                src => src.DrainagePurposes)
            .Map(dest => dest.FramesInStand,
                src => src.Frames)
            .Map(
                dest => dest.ObvyazkiInStand,
                src => src.StandObvs);
    }
}