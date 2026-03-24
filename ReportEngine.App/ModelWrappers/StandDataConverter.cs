using System.Collections.ObjectModel;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.ModelWrappers;

public static class StandDataConverter
{
    public static StandModel ConvertToStandModel(Stand stand)
    {
        return new StandModel
        {
            Id = stand.Id,
            ProjectId = stand.ProjectInfoId, // обязательно копируем ProjectId
            KKSCode = stand.KKSCode,
            Design = stand.Design,
            BraceType = stand.BraceType,
            Devices = stand.Devices,
            Width = stand.Width,
            Comments = stand.Comments,
            SerialNumber = stand.SerialNumber,
            Weight = stand.Weight,
            Number = stand.Number,
            StandSummCost = stand.StandSummCost,
            DesignStand = stand.DesigneStand,
            NN = stand.NN,
            MaterialLine = stand.MaterialLine,
            Armature = stand.Armature,
            TreeSocket = stand.TreeSocket,
            KMCH = stand.KMCH,
            ImageData = stand.ImageData,
            ImageType = stand.ImageType,
            AdditionalEquipsInStand = new ObservableCollection<FormedAdditionalEquip>(
                stand.StandAdditionalEquips
                        .Where(e => e.AdditionalEquip != null)
                        .Select(e => new FormedAdditionalEquip
                        {
                            Id = e.AdditionalEquip.Id,
                            Name = e.AdditionalEquip.Name,

                            Purposes = new ObservableCollection<AdditionalEquipPurpose>(
                                e.AdditionalEquip.Purposes.Select(p => new AdditionalEquipPurpose
                                {
                                    Purpose = p.Purpose,
                                    Material = p.Material,
                                    Quantity = p.Quantity,
                                    Measure = p.Measure,
                                    CostPerUnit = p.CostPerUnit,
                                    ExportDays = p.ExportDays
                                })
                            )
                        }))
        };
    }

    public static Stand ConvertToStandEntity(StandModel model)
    {
        return new Stand
        {
            Id = model.Id,
            ProjectInfoId = model.ProjectId, // обязательно копируем ProjectId
            KKSCode = model.KKSCode,
            Design = model.Design,
            BraceType = model.BraceType,
            Comments = model.Comments,
            Devices = model.Devices,
            Width = model.Width,
            Number = model.Number,
            DesigneStand = model.DesignStand,
            SerialNumber = model.SerialNumber,
            Weight = model.Weight,
            StandSummCost = model.StandSummCost,
            ObvyazkaType = model.ObvyazkaName,
            NN = model.NN,
            MaterialLine = model.MaterialLine,
            Armature = model.Armature,
            TreeSocket = model.TreeSocket,
            KMCH = model.KMCH,
            ImageData = model.ImageData,
            ImageType = model.ImageType,
            ObvyazkiInStand = model.ObvyazkiInStand,
            StandAdditionalEquips = model.AdditionalEquipsInStand
                .Select(equip => new StandAdditionalEquip
                {
                    AdditionalEquipId = equip.Id,
                    AdditionalEquip = equip is FormedAdditionalEquip fae ? fae : null
                })
                .ToList(),

            StandElectricalComponent = model.ElectricalComponentsInStand
                .Select(equip => new StandElectricalComponent
                {
                    ElectricalComponentId = equip.Id,
                    ElectricalComponent = equip is FormedElectricalComponent fe ? fe : null
                })
                .ToList(),

            StandDrainages = model.DrainagesInStand
                .Select(equip => new StandDrainage
                {
                    DrainageId = equip.Id,
                    Drainage = equip is FormedDrainage fd ? fd : null
                }).ToList(),

            StandFrames = model.FramesInStand
                .Select(equip => new StandFrame
                {
                    FrameId = equip.Id,
                    Frame = equip is FormedFrame fd ? fd : null

                }).ToList()          
        };
    }
}
