using ReportEngine.App.Model;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.ModelWrappers;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Enums;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;

namespace ReportEngine.App.Services.Core;

public class ProjectService : IProjectService
{
    private readonly INotificationService _notificationService;
    private readonly IProjectInfoRepository _projectRepository;

    public ProjectService(
        IProjectInfoRepository projectRepository,
        INotificationService notificationService)
    {
        _projectRepository = projectRepository;
        _notificationService = notificationService;
    }

    public async Task CreateProjectAsync(ProjectModel projectModel)
    {
        var project = projectModel.CreateNewProjectCard();

        await _projectRepository.AddAsync(project);
        projectModel.CurrentProjectId = project.Id;

        _notificationService.ShowInfo($"Новая карточка проекта создана! ID: {project.Id}");
    }

    public async Task UpdateProjectAsync(ProjectModel projectModel)
    {
        var projectInfo = new ProjectInfo
        {
            Id = projectModel.CurrentProjectId,
            Number = projectModel.Number,
            Description = projectModel.Description,
            CreationDate = DateOnly.FromDateTime(projectModel.CreationDate),
            Company = projectModel.Company,
            Object = projectModel.Object,
            StandCount = projectModel.StandCount,
            Cost = projectModel.Cost,
            HumanCost = projectModel.HumanCost,
            Manager = projectModel.Manager,
            Status = ComboBoxHelper.ComboBoxChangedValue<ProjectStatus>(projectModel.Status),
            StartDate = DateOnly.FromDateTime(projectModel.StartDate),
            OutOfProduction = DateOnly.FromDateTime(projectModel.OutOfProduction),
            EndDate = DateOnly.FromDateTime(projectModel.EndDate),
            OrderCustomer = projectModel.OrderCustomer,
            RequestProduction = projectModel.RequestProduction,
            MarkMinus = projectModel.MarkMinus,
            MarkPlus = projectModel.MarkPlus,
            IsGalvanized = projectModel.IsGalvanized
        };
        await _projectRepository.UpdateAsync(projectInfo);

        _notificationService.ShowInfo("Изменения успешно сохранены!");
    }

    public async Task AddStandToProjectAsync(int projectId, StandModel standModel)
    {
        var standEntity = StandDataConverter.ConvertToStandEntity(standModel);
        var addedStand = await _projectRepository.AddStandAsync(projectId, standEntity);

        standModel.Id = addedStand.Id;
        standModel.ProjectId = addedStand.ProjectInfoId;


        _notificationService.ShowInfo("Стенд добавлен");
    }
}