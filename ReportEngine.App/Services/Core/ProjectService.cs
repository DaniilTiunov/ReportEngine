using Microsoft.VisualBasic;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.Model;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.ModelWrappers;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Enums;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;
using System.Collections.ObjectModel;

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

    public async Task CopyStandsAsync(ProjectModel projectModel)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var count = Convert.ToInt32(Interaction.InputBox("Введите количество копий", "Копирование стенда", "1"));
            var selectedStand = projectModel.SelectedStand;
            if (selectedStand == null)
            {
                _notificationService.ShowError("Стенд не выбран для копирования.");
                return;
            }

            var existingKKS = projectModel.Stands.Select(s => s.KKSCode).ToHashSet();
            var existingDesign = projectModel.Stands.Select(s => s.Design).ToHashSet();

            for (var i = 1; i <= count; i++)
            {
                // Генерируем уникальные значения
                var newKKS = $"{selectedStand.KKSCode}_copy{i}";
                var newDesign = $"{selectedStand.Design}_copy{i}";

                // Проверяем уникальность
                while (existingKKS.Contains(newKKS))
                    newKKS += "_new";
                while (existingDesign.Contains(newDesign))
                    newDesign += "_new";

                // Копируем стенд
                var newStand = new StandModel
                {
                    Design = newDesign,
                    KKSCode = newKKS,
                    ProjectId = projectModel.CurrentProjectId,
                    Armature = selectedStand.Armature,
                    BraceType = selectedStand.BraceType,
                    Devices = selectedStand.Devices,
                    KMCH = selectedStand.KMCH,
                    MaterialLine = selectedStand.MaterialLine,
                    NN = selectedStand.NN,
                    ObvyazkaName = selectedStand.ObvyazkaName,
                    SerialNumber = StandUniqNameHelper.SetUniqNameForStand(selectedStand),
                    TreeSocket = selectedStand.TreeSocket,
                    Weight = selectedStand.Weight,
                    Width = selectedStand.Width,
                    DesigneStand = selectedStand.DesigneStand,
                    FramesInStand = new ObservableCollection<FormedFrame>(
                        selectedStand.FramesInStand.Select(f => CloneFrame(f))
                    ),
                    DrainagesInStand = new ObservableCollection<FormedDrainage>(
                        selectedStand.DrainagesInStand.Select(d => CloneDrainage(d))
                    ),
                    AdditionalEquipsInStand = new ObservableCollection<FormedAdditionalEquip>(
                        selectedStand.AdditionalEquipsInStand.Select(a => CloneAdditionalEquip(a))
                    ),
                    AllDrainagePurposesInStand = new ObservableCollection<DrainagePurpose>(
                        selectedStand.AllDrainagePurposesInStand.Select(p => CloneDrainagePurpose(p))
                    ),
                    ElectricalComponentsInStand = new ObservableCollection<FormedElectricalComponent>(
                        selectedStand.ElectricalComponentsInStand.Select(e => CloneElectricalComponent(e))
                    ),
                    ObvyazkiInStand = new ObservableCollection<ObvyazkaInStand>(
                    selectedStand.ObvyazkiInStand.Select(obv =>
                            ObvyzkaModelWrapper.CloneForStand(obv, 0) // 0 или newStand.Id, если уже есть
                    ))
                };

                var newStandEntity = StandDataConverter.ConvertToStandEntity(newStand);
                var addedStandEntity =
                    await _projectRepository.AddStandAsync(projectModel.CurrentProjectId, newStandEntity);

                newStand.Id = addedStandEntity.Id;
                newStand.ProjectId = addedStandEntity.ProjectInfoId;
                newStand.InitializeDefaultPurposes();

                projectModel.Stands.Add(newStand);

                existingKKS.Add(newKKS);
                existingDesign.Add(newDesign);
            }

            _notificationService.ShowInfo($"Создано копий: {count}");
        });
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
            HumanCost = projectModel.HumanCost ?? 0.0f,
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
            HumanCost = projectInfo.HumanCost,
            Status = projectInfo.Status.ToString(),
            StartDate = projectInfo.StartDate.ToDateTime(TimeOnly.MinValue),
            OutOfProduction = projectInfo.OutOfProduction.ToDateTime(TimeOnly.MinValue),
            EndDate = projectInfo.EndDate.ToDateTime(TimeOnly.MinValue),
            OrderCustomer = projectInfo.OrderCustomer,
            RequestProduction = projectInfo.RequestProduction,
            Manager = projectInfo.Manager,
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

    public async Task UpdateObvInStandAsync(ProjectModel projectModel, Obvyazka selectedObvyazka)
    {
        var stand = projectModel.SelectedStand;
        var obv = stand.SelectedObvyazkaInStand;
        if (stand == null || obv == null)
            return;

        obv.ObvyazkaName = stand.ObvyazkaName;
        obv.MaterialLine = stand.MaterialLine;
        obv.MaterialLineCount = stand.MaterialLineCount;
        obv.MaterialLineMeasure = stand.MaterialLineMeasure;
        obv.Armature = stand.Armature;
        obv.ArmatureCount = stand.ArmatureCount;
        obv.ArmatureMeasure = stand.ArmatureMeasure;
        obv.TreeSocket = stand.TreeSocket;
        obv.TreeSocketMaterialCount = stand.TreeSocketMaterialCount;
        obv.TreeSocketMaterialMeasure = stand.TreeSocketMaterialMeasure;
        obv.KMCH = stand.KMCH;
        obv.KMCHCount = stand.KMCHCount;
        obv.KMCHMeasure = stand.KMCHMeasure;
        obv.NN = stand.NN;
        obv.FirstSensorType = stand.FirstSensorType;
        obv.FirstSensorKKS = stand.FirstSensorKKS;
        obv.FirstSensorMarkPlus = stand.FirstSensorMarkPlus;
        obv.FirstSensorMarkMinus = stand.FirstSensorMarkMinus;
        obv.FirstSensorDescription = stand.FirstSensorDescription;
        obv.SecondSensorType = stand.SecondSensorType;
        obv.SecondSensorKKS = stand.SecondSensorKKS;
        obv.SecondSensorMarkPlus = stand.SecondSensorMarkPlus;
        obv.SecondSensorMarkMinus = stand.SecondSensorMarkMinus;
        obv.SecondSensorDescription = stand.SecondSensorDescription;
        obv.ThirdSensorType = stand.ThirdSensorType;
        obv.ThirdSensorKKS = stand.ThirdSensorKKS;
        obv.ThirdSensorMarkPlus = stand.ThirdSensorMarkPlus;
        obv.ThirdSensorMarkMinus = stand.ThirdSensorMarkMinus;
        obv.ThirdSensorDescription = stand.ThirdSensorDescription;

        obv.LineLength = selectedObvyazka.LineLength;
        obv.ZraCount = selectedObvyazka.ZraCount;
        obv.Sensor = selectedObvyazka.Sensor;
        obv.SensorType = selectedObvyazka.SensorType;
        obv.Clamp = selectedObvyazka.Clamp;
        obv.WidthOnFrame = selectedObvyazka.WidthOnFrame;
        obv.OtherLineCount = selectedObvyazka.OtherLineCount;
        obv.Weight = selectedObvyazka.Weight;
        obv.TreeSocketCount = selectedObvyazka.TreeSocket;
        obv.HumanCost = selectedObvyazka.HumanCost;
        obv.ImageName = selectedObvyazka.ImageName;

        CollectionRefreshHelper.SafeRefreshCollection(projectModel.SelectedStand.ObvyazkiInStand);

        await _projectRepository.UpdateObvInStandAsync(projectModel.SelectedStand.Id,
            projectModel.SelectedStand.SelectedObvyazkaInStand);
        _notificationService.ShowInfo("Обвязка обновлена");
    }

    public async Task DeleteFrameFromStandAsync(ProjectModel projectModel)
    {
        var stand = projectModel.SelectedStand;
        var frame = stand.SelectedFrame;
        var standFrame = frame.StandFrames?.FirstOrDefault(sf => sf.StandId == stand.Id && sf.FrameId == frame.Id);

        stand.FramesInStand.Remove(frame);

        stand.SelectedFrame = null;

        projectModel.OnPropertyChanged(nameof(projectModel.SelectedStand.FramesInStand));

        if (standFrame != null)
            await _projectRepository.DeleteFrameFromStandAsync(standFrame.Id);
    }

    public async Task LoadAllObvyazkiInProject(ProjectModel projectModel)
    {
        var projectInfo = await _projectRepository.GetStandsByIdAsync(projectModel.CurrentProjectId);
        if (projectInfo?.Stands == null)
            return;

        var obvyazkiInStands = projectInfo.Stands
            .Where(s => s.ObvyazkiInStand != null)
            .SelectMany(s => s.ObvyazkiInStand);

        projectModel.ObvyazkiInProject.Clear();

        foreach (var obvyazkiInStand in obvyazkiInStands) projectModel.ObvyazkiInProject.Add(obvyazkiInStand);
    }

    private FormedFrame CloneFrame(FormedFrame frame)
    {
        if (frame == null) return null;
        return new FormedFrame
        {
            Name = frame.Name,
            FrameType = frame.FrameType,
            Width = frame.Width,
            Height = frame.Height,
            Depth = frame.Depth,
            Weight = frame.Weight,
            Designe = frame.Designe
        };
    }

    private FormedDrainage CloneDrainage(FormedDrainage drainage)
    {
        if (drainage == null) return null;
        return new FormedDrainage
        {
            Name = drainage.Name,
            Purposes = drainage.Purposes?.Select(CloneDrainagePurpose).ToList() ?? new List<DrainagePurpose>()
            // Клонируйте другие нужные свойства
        };
    }

    private DrainagePurpose CloneDrainagePurpose(DrainagePurpose purpose)
    {
        if (purpose == null) return null;
        return new DrainagePurpose
        {
            Purpose = purpose.Purpose,
            Material = purpose.Material,
            Quantity = purpose.Quantity,
            CostPerUnit = purpose.CostPerUnit,
            Measure = purpose.Measure,
            FormedDrainageId = 0 // Привязка будет установлена при добавлении в новый дренаж
        };
    }

    private FormedAdditionalEquip CloneAdditionalEquip(FormedAdditionalEquip equip)
    {
        if (equip == null) return null;
        return new FormedAdditionalEquip
        {
            Name = equip.Name,
            Purposes = equip.Purposes?.Select(CloneAdditionalEquipPurpose).ToList() ?? new List<AdditionalEquipPurpose>()
            // Клонируйте другие нужные свойства
        };
    }

    private AdditionalEquipPurpose CloneAdditionalEquipPurpose(AdditionalEquipPurpose purpose)
    {
        if (purpose == null) return null;
        return new AdditionalEquipPurpose
        {
            Purpose = purpose.Purpose,
            Material = purpose.Material,
            Quantity = purpose.Quantity,
            CostPerUnit = purpose.CostPerUnit,
            Measure = purpose.Measure,
            FormedAdditionalEquipId = 0
        };
    }

    private FormedElectricalComponent CloneElectricalComponent(FormedElectricalComponent component)
    {
        if (component == null) return null;
        return new FormedElectricalComponent
        {
            Name = component.Name,
            Purposes = component.Purposes?.Select(CloneElectricalPurpose).ToList() ?? new List<ElectricalPurpose>()
        };
    }

    private ElectricalPurpose CloneElectricalPurpose(ElectricalPurpose purpose)
    {
        if (purpose == null) return null;
        return new ElectricalPurpose
        {
            Purpose = purpose.Purpose,
            Material = purpose.Material,
            Quantity = purpose.Quantity,
            CostPerUnit = purpose.CostPerUnit,
            Measure = purpose.Measure,
            FormedElectricalComponentId = 0
        };
    }
}