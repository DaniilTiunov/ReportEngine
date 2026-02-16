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

        public async Task<List<AdditionalEquipPurpose>> GetAdditionalEquipsAsync(ProjectModel projectModel)
        {
            return projectModel.SelectedStand.AllAdditionalEquipPurposesInStand.ToList();
        }

        public async Task<List<ObvyazkaAdditionalEquipPurpose>> GetObvyazkiAdditionalEquipsAsync(ProjectModel projectModel)
        {
            return projectModel.SelectedStand.ObvyazkaAdditionalComponents.ToList();
        }

        public async Task CreateEquipsFromObvyzkaAsync(ProjectModel projectModel)
        {
            var additionalEquipInStand = await GetAdditionalEquipsAsync(projectModel);
            var additionalEquipInObv   = await GetObvyazkiAdditionalEquipsAsync(projectModel);

            var newEquips = additionalEquipInObv.Select(obv => new AdditionalEquipPurpose
            {
                Purpose = obv.Purpose,
                Material = obv.Material,
                Quantity = obv.Quantity,
                CostPerUnit = obv.CostPerUnit,
                Measure = obv.Measure,
                ExportDays = obv.ExportDays,
            }).ToList();

            foreach(var equip  in newEquips)
            {
                projectModel.SelectedStand.AllAdditionalEquipPurposesInStand.Add(equip);
            }
        }
    }
}
