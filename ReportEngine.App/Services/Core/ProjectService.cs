using ReportEngine.App.Model;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.ModelWrappers;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Enums;
using ReportEngine.Domain.Repositories;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;
using System.Collections.ObjectModel;

namespace ReportEngine.App.Services.Core;

public class ProjectService : IProjectService
{
    private readonly INotificationService _notificationService;
    private readonly IProjectInfoRepository _projectRepository;
    private readonly IContainerRepository _containerRepository;

    public ProjectService(
        IProjectInfoRepository projectRepository,
        INotificationService notificationService,
        IContainerRepository containerRepository)
    {
        _projectRepository = projectRepository;
        _notificationService = notificationService;
        _containerRepository = containerRepository;
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

    public async Task UpdateStandEntity(ProjectModel standModel)
    {
        foreach (var stand in standModel.Stands)
            await _projectRepository.UpdateStandAsync(StandDataConverter.ConvertToStandEntity(stand));
    }

    public async Task DeleteStandAsync(int projectId, int standId)
    {
        await _projectRepository.DeleteStandAsync(projectId, standId);
        _notificationService.ShowInfo($"Стенд с Id {standId} удалён из\n Проекта c Id {projectId}");
    }

    public async Task AddStandToProjectAsync(int projectId, StandModel standModel)
    {
        var standEntity = StandDataConverter.ConvertToStandEntity(standModel);
        var addedStand = await _projectRepository.AddStandAsync(projectId, standEntity);

        standModel.Id = addedStand.Id;
        standModel.ProjectId = addedStand.ProjectInfoId;
    }

    public async Task<ProjectModel> LoadProjectInfoAsync(int projectId)
    {
        var projectInfo = await _projectRepository.GetStandsByIdAsync(projectId);
        if (projectInfo == null)
            return null;

        var model = new ProjectModel
        {
            CurrentProjectId = projectInfo.Id,
            Number = projectInfo.Number,
            Description = projectInfo.Description,
            CreationDate = projectInfo.CreationDate.ToDateTime(TimeOnly.MinValue),
            Company = projectInfo.Company,
            Object = projectInfo.Object,
            StandCount = projectInfo.StandCount,
            Cost = projectInfo.Cost,
            Status = projectInfo.Status.ToString(),
            StartDate = projectInfo.StartDate.ToDateTime(TimeOnly.MinValue),
            OutOfProduction = projectInfo.OutOfProduction.ToDateTime(TimeOnly.MinValue),
            EndDate = projectInfo.EndDate.ToDateTime(TimeOnly.MinValue),
            OrderCustomer = projectInfo.OrderCustomer,
            RequestProduction = projectInfo.RequestProduction,
            MarkMinus = projectInfo.MarkMinus,
            MarkPlus = projectInfo.MarkPlus,
            IsGalvanized = projectInfo.IsGalvanized,
            Stands = new ObservableCollection<StandModel>()
        };

        if (projectInfo.Stands != null)
            foreach (var stand in projectInfo.Stands)
            {
                var standModel = StandDataConverter.ConvertToStandModel(stand);
                standModel.FramesInStand = new ObservableCollection<FormedFrame>(
                    stand.StandFrames?.Select(sf => sf.Frame) ?? Enumerable.Empty<FormedFrame>()
                );
                model.Stands.Add(standModel);
            }

        model.SelectedStand = model.Stands.FirstOrDefault();
        return model;
    }

    public async Task DeleteObvFromStandAsync(int standId, int obvyazkaInStandId)
    {
        await _projectRepository.DeleteObvFromStandAsync(standId, obvyazkaInStandId);
    }

    public async Task DeleteFrameFromStandAsync(int standId, int frameInStandId)
    {
        await _projectRepository.DeleteFrameFromStandAsync(standId, frameInStandId);
    }
}