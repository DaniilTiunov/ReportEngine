using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.Services.Cloners
{
    public class EntityProjectClonerService
    {
        private readonly IProjectInfoRepository _projectInfoRepository;
        private readonly EntityStandClonerService _entityStandClonerService;
        private readonly INotificationService _notificationService;

        public EntityProjectClonerService(
            IProjectInfoRepository projectInfoRepository,
            EntityStandClonerService entityStandClonerService,
            INotificationService notificationService)
        {
            _projectInfoRepository = projectInfoRepository;
            _entityStandClonerService = entityStandClonerService;
            _notificationService = notificationService;
        }

        public async Task CloneProjectEntity(ProjectInfo sourceProject)
        {
            var sourceStands = await _projectInfoRepository.GetProjectWithStandsAsync(sourceProject.Id);

            var newProject = new ProjectInfo
            {
                Id = 0,
                Company = sourceProject.Company,
                Cost = sourceProject.Cost,
                MarkMinus = sourceProject.MarkMinus,
                MarkPlus = sourceProject.MarkPlus,
                Manager = sourceProject.Manager,
                CreationDate = sourceProject.CreationDate,
                EndDate = sourceProject.EndDate,
                HumanCost = sourceProject.HumanCost,
                IsGalvanized = sourceProject.IsGalvanized,
                Description = sourceProject.Description,
                Object = sourceProject.Object,
                Number = sourceProject.Number + 1,
                OutOfProduction = sourceProject.OutOfProduction,
                OrderCustomer = sourceProject.OrderCustomer,
                RequestProduction = sourceProject.RequestProduction,
                StandCount = sourceProject.StandCount,
                StartDate = sourceProject.StartDate,
                Status = sourceProject.Status,
                Stands = new List<Stand>()
            };

            foreach (var stand in sourceStands)
            {
                var newStand = await _entityStandClonerService.CloneStandEntity(stand);

                newProject.Stands.Add(newStand);

            }

            await _projectInfoRepository.AddAsync(newProject);

            _notificationService.ShowInfo("Проект успешно скопирован!");
        }
    }
}
