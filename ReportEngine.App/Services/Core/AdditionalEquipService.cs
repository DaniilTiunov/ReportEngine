using ReportEngine.App.AppHelpers;
using ReportEngine.App.Model;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.Services.Core
{
    public class AdditionalEquipService
    {
        private readonly IStandService _standService;
        private readonly IProjectService _projectService;

        public AdditionalEquipService(
            IStandService standService,
            IProjectService projectService
            )
        {
            _standService = standService;
            _projectService = projectService;
        }
        public async Task CreateEquipsFromObvyzkaAsync(ProjectModel projectModel)
        {
            var obvEquips = GetObvyazkiAdditionalEquips(projectModel);

            var formedId = GetFormedAdditionalEquipId(projectModel);

            var newEquips = CreateEquipsFromObvyazkaEntities(obvEquips, formedId);

            await AddAdditionalEquipsToStandAsync(projectModel, newEquips);
        }

        private List<ObvyazkaAdditionalEquipPurpose> GetObvyazkiAdditionalEquips(ProjectModel projectModel)
        {
            return projectModel.SelectedStand.ObvyazkiInStand
                    .Where(obv => obv.AdditionalComponents != null)
                    .SelectMany(obv => obv.AdditionalComponents)
                    .ToList();
        }

        private int GetFormedAdditionalEquipId(ProjectModel projectModel)
        {
            var first = projectModel.SelectedStand.AllAdditionalEquipPurposesInStand.FirstOrDefault();
            return first?.FormedAdditionalEquipId ?? 0;
        }

        private List<AdditionalEquipPurpose> CreateEquipsFromObvyazkaEntities(
            IEnumerable<ObvyazkaAdditionalEquipPurpose> source,
            int formedAdditionalEquipId)
        {
            return source.Select(obv => new AdditionalEquipPurpose
            {
                FormedAdditionalEquipId = formedAdditionalEquipId,
                Purpose = obv.Purpose,
                Material = obv.Material,
                Quantity = obv.Quantity,
                CostPerUnit = obv.CostPerUnit,
                Measure = obv.Measure,
                ExportDays = obv.ExportDays,
            }).ToList();
        }

        private async Task AddAdditionalEquipsToStandAsync(
            ProjectModel projectModel,
            IEnumerable<AdditionalEquipPurpose> equips)
        {
            var collection = projectModel.SelectedStand.AllAdditionalEquipPurposesInStand;

            foreach (var equip in equips)
            {
                var exisitng = collection.FirstOrDefault(x =>
                    x.Purpose == equip.Purpose &&
                    x.Material == equip.Material);

                if (exisitng != null)
                {
                    exisitng.Quantity += equip.Quantity;

                    await _standService.UpdateAdditionalPurposeAsync(exisitng);
                }
                else
                {
                    var newEquip = new AdditionalEquipPurpose
                    {
                        FormedAdditionalEquipId = GetFormedAdditionalEquipId(projectModel),
                        Purpose = equip.Purpose,
                        Material = equip.Material,
                        Quantity = equip.Quantity,
                        CostPerUnit = equip.CostPerUnit,
                        Measure = equip.Measure,
                        ExportDays = equip.ExportDays,
                    };

                    collection.Add(newEquip);
                    await _standService.UpdateAdditionalPurposeAsync(newEquip);
                }
            }

            CollectionRefreshHelper.SafeRefreshCollection(collection);
        }
    }
}
