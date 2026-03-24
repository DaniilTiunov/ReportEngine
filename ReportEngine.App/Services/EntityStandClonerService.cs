using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.Services
{
    public class EntityStandClonerService
    {
        private readonly IProjectInfoRepository _projectRepository;

        public EntityStandClonerService(IProjectInfoRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<Stand> CloneStandEntity(Stand sourceStand, int projectId)
        {
            var standComponents = await GetAllStandsDataAsync(sourceStand);

            var newStand = new Stand
            {
                ProjectInfoId = projectId,
                KKSCode = standComponents.KKSCode,
                Design = standComponents.Design,
                BraceType = standComponents.BraceType,
                Devices = standComponents.Devices,
                Width = standComponents.Width,
                Comments = standComponents.Comments,
                Weight = standComponents.Weight,
                StandSummCost = standComponents.StandSummCost,
                DesigneStand = standComponents.DesigneStand,
                NN = standComponents.NN,
                MaterialLine = standComponents.MaterialLine,
                Armature = standComponents.Armature,
                TreeSocket = standComponents.TreeSocket,
                KMCH = standComponents.KMCH,
                ImageData = standComponents.ImageData,
                ImageType = standComponents.ImageType,

                StandFrames = CloneFrames(standComponents.StandFrames),
                StandAdditionalEquips = CloneAdditionalEquips(standComponents.StandAdditionalEquips),
                StandDrainages = CloneDrainages(standComponents.StandDrainages),
                StandElectricalComponent = CloneElectricalComponents(standComponents.StandElectricalComponent),
                //ObvyazkiInStand = CloneObvyazki(standComponents.ObvyazkiInStand)
            };

            return newStand;
        }

        public async Task<Stand> GetAllStandsDataAsync(Stand sourceStand)
        {
            var stand = sourceStand;

            stand.StandFrames = (await _projectRepository
                .GetAllFramesInStandAsync(stand.Id)).ToList();

            stand.ObvyazkiInStand = (await _projectRepository
                .GetAllObvyazkiInStandAsync(stand.Id)).ToList();

            stand.StandDrainages = (await _projectRepository
                .GetAllDrainagesInStandAsync(stand.Id)).ToList();

            stand.StandElectricalComponent = (await _projectRepository
                .GetAllElectricalComponentsInStandAsync(stand.Id)).ToList();

            stand.StandAdditionalEquips = (await _projectRepository
                .GetAllAdditionalEquipsInStandAsync(stand.Id)).ToList();

            return stand;
        }

        private List<StandFrame> CloneFrames(IEnumerable<StandFrame> frames)
        {
            return frames.Select(f => new StandFrame
            {
                Id = 0,
                FrameId = f.Id,
                Frame = f.Frame
            }).ToList();
        }

        private List<StandAdditionalEquip> CloneAdditionalEquips(IEnumerable<StandAdditionalEquip> equips)
        {
            return equips.Select(sae => new StandAdditionalEquip
            {
                Id = 0,
                AdditionalEquip = new FormedAdditionalEquip
                {
                    Id = 0,
                    Name = sae.AdditionalEquip.Name,
                    Purposes = sae.AdditionalEquip.Purposes.Select(p => new AdditionalEquipPurpose
                    {
                        Id = 0,
                        Purpose = p.Purpose,
                        Material = p.Material,
                        Quantity = p.Quantity,
                        Measure = p.Measure,
                        CostPerUnit = p.CostPerUnit,
                        ExportDays = p.ExportDays
                    }).ToList()
                },
                AdditionalEquipId = sae.AdditionalEquipId
            }).ToList();
        }

        private List<StandDrainage> CloneDrainages(IEnumerable<StandDrainage> drainages)
        {
            return drainages.Select(sd => new StandDrainage
            {
                Id = 0,
                Drainage = new FormedDrainage
                {
                    Id = 0,
                    Name = sd.Drainage.Name,
                    Purposes = sd.Drainage.Purposes.Select(p => new DrainagePurpose
                    {
                        Id = 0,
                        Purpose = p.Purpose,
                        Material = p.Material
                    }).ToList()
                }
            }).ToList();
        }

        private List<StandElectricalComponent> CloneElectricalComponents(IEnumerable<StandElectricalComponent> components)
        {
            return components.Select(sec => new StandElectricalComponent
            {
                Id = 0,
                ElectricalComponent = new FormedElectricalComponent
                {
                    Id = 0,
                    Name = sec.ElectricalComponent.Name,
                    Purposes = sec.ElectricalComponent.Purposes.Select(p => new ElectricalPurpose
                    {
                        Id = 0,
                        Purpose = p.Purpose,
                        Material = p.Material
                    }).ToList()
                }
            }).ToList();
        }

        private List<ObvyazkaInStand> CloneObvyazki(IEnumerable<ObvyazkaInStand> obvyazki)
        {
            return obvyazki.Select(obv => new ObvyazkaInStand
                {
                    Id = 0,
                    Armature = obv.Armature,
                    Clamp = obv.Clamp,
                    
                    ObvyazkaId = obv.ObvyazkaId,
                    Obvyazka = null,
                    AdditionalComponents = obv.AdditionalComponents
                .Select(ac => new ObvyazkaAdditionalEquipPurpose
                {
                    Id = 0,
                    Material = ac.Material,
                    Quantity = ac.Quantity,
                    Measure = ac.Measure,
                    CostPerUnit = ac.CostPerUnit,
                    ExportDays = ac.ExportDays
                }).ToList()
            }).ToList();
        }
    }
}
