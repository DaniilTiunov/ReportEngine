using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.ModelWrappers;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.Services.Core;

public class StandService : IStandService
{
    private readonly IContainerRepository _containerRepository;
    private readonly IFormedAdditionalEquipsRepository _formedAdditionalEquipsRepository;
    private readonly IFormedDrainagesRepository _formedDrainagesRepository;
    private readonly IFormedElectricalRepository _formedElectricalRepository;
    private readonly IFrameRepository _formedFrameRepository;
    private readonly INotificationService _notificationService;
    private readonly ObvyazkaInStandRepository _obvyazkaInStandRepository;
    private readonly IProjectInfoRepository _projectRepository;
    private readonly ReAppContext _context;

    public StandService(IProjectInfoRepository projectRepository,
        IFrameRepository frameRepository,
        IFormedDrainagesRepository drainagesRepository,
        INotificationService notificationService,
        IFormedAdditionalEquipsRepository formedAdditionalEquipsRepository,
        IFormedElectricalRepository formedElectricalRepository,
        IContainerRepository containerRepository,
        ObvyazkaInStandRepository obvyazkaInStandRepository,
        ReAppContext context)
    {
        _obvyazkaInStandRepository = obvyazkaInStandRepository;
        _projectRepository = projectRepository;
        _formedFrameRepository = frameRepository;
        _formedDrainagesRepository = drainagesRepository;
        _notificationService = notificationService;
        _formedAdditionalEquipsRepository = formedAdditionalEquipsRepository;
        _formedElectricalRepository = formedElectricalRepository;
        _containerRepository = containerRepository;
        _context = context;
    }

    public async Task<IEnumerable<FormedFrame>> LoadAllAvailableFrameAsync()
    {
        return await _formedFrameRepository.GetAllAsync();
    }

    public async Task<IEnumerable<FormedDrainage>> LoadAllAvailableDrainagesAsync()
    {
        return await _formedDrainagesRepository.GetAllWithPurposesAsync();
    }

    public async Task<IEnumerable<FormedElectricalComponent>> LoadAllAvailableElectricalComponentsAsync()
    {
        return await _formedElectricalRepository.GetAllAsync();
    }

    public async Task<IEnumerable<FormedAdditionalEquip>> LoadAllAvailableAdditionalEquipsAsync()
    {
        return await _formedAdditionalEquipsRepository.GetAllAsync();
    }

    public async Task LoadObvyazkiInStandsAsync(IEnumerable<StandModel> standModels)
    {
        foreach (var standModel in standModels)
        {
            var obvyazkiInStand = await _projectRepository.GetAllObvyazkiInStandAsync(standModel.Id);
            standModel.ObvyazkiInStand = new ObservableCollection<ObvyazkaInStand>(obvyazkiInStand);
        }
    }

    public async Task LoadStandsDataAsync(IEnumerable<StandModel> standModels)
    {
        foreach (var standModel in standModels)
        {
            var standFrames = await _projectRepository.GetAllFramesInStandAsync(standModel.Id);
            var standDrainages = await _projectRepository.GetAllDrainagesInStandAsync(standModel.Id);
            var standElectricals = await _projectRepository.GetAllElectricalComponentsInStandAsync(standModel.Id);
            var standAdditionals = await _projectRepository.GetAllAdditionalEquipsInStandAsync(standModel.Id);

            standModel.FramesInStand.Clear();
            foreach (var frame in standFrames.Select(sf => sf.Frame))
                standModel.FramesInStand.Add(frame);

            standModel.DrainagesInStand.Clear();
            foreach (var drainage in standDrainages.Select(sd => sd.Drainage))
                standModel.DrainagesInStand.Add(drainage);

            standModel.ElectricalComponentsInStand.Clear();
            foreach (var electrical in standElectricals.Select(se => se.ElectricalComponent))
                standModel.ElectricalComponentsInStand.Add(electrical);

            standModel.AdditionalEquipsInStand.Clear();
            foreach (var additional in standAdditionals.Select(sa => sa.AdditionalEquip))
                standModel.AdditionalEquipsInStand.Add(additional);
        }
    }

    public void LoadPurposesInStands(IEnumerable<StandModel> stands)
    {
        foreach (var stand in stands)
        {
            stand.AllDrainagePurposesInStand = new ObservableCollection<DrainagePurpose>(
                stand.DrainagesInStand?.SelectMany(d => d.Purposes ?? Enumerable.Empty<DrainagePurpose>()) ??
                Enumerable.Empty<DrainagePurpose>());

            stand.AllElectricalPurposesInStand = new ObservableCollection<ElectricalPurpose>(
                stand.ElectricalComponentsInStand?.SelectMany(e =>
                    e.Purposes ?? Enumerable.Empty<ElectricalPurpose>()) ?? Enumerable.Empty<ElectricalPurpose>());

            stand.AllAdditionalEquipPurposesInStand = new ObservableCollection<AdditionalEquipPurpose>(
                stand.AdditionalEquipsInStand?.SelectMany(a =>
                    a.Purposes ?? Enumerable.Empty<AdditionalEquipPurpose>()) ??
                Enumerable.Empty<AdditionalEquipPurpose>());
        }
    }

    public async Task AddFrameToStandAsync(int standId, int frameId)
    {
        await _projectRepository.AddFrameToStandAsync(standId, frameId);
        _notificationService.ShowInfo($"Рама успешно добавлена в стенд");
    }

    public async Task AddDrainageToStandAsync(int standId, int drainageId)
    {
        await _projectRepository.AddDrainageToStandAsync(standId, drainageId);
        _notificationService.ShowInfo($"Дренаж успешно добавлен в стенд");
    }

    public async Task AddObvyazkaToStandAsync(int standId, ObvyazkaInStand obvyazka)
    {
        await _projectRepository.AddStandObvyazkaAsync(standId, obvyazka);
    }

    public async Task AddCustomDrainageAsync(int standId, List<DrainagePurpose> drainagePurposes, FormedDrainage customDrainage)
    {
        var entity = new FormedDrainage
        {
            Purposes = drainagePurposes
        };

        await _formedDrainagesRepository.AddAsync(entity);
        await _projectRepository.AddDrainageToStandAsync(standId, entity.Id);
    }

    public async Task AddCustomElectricalComponentAsync(int standId, List<ElectricalPurpose> electricalPurpose, FormedElectricalComponent customElectrical)
    {
        var entity = new FormedElectricalComponent
        {
            Purposes = electricalPurpose
        };

        await _formedElectricalRepository.AddAsync(entity);
        await _projectRepository.AddElectricalComponentToStandAsync(standId, entity.Id);
    }

    public async Task AddCustomAdditionalEquipAsync(int standId, List<AdditionalEquipPurpose> additionalEquipPurposes, FormedAdditionalEquip customEquip)
    {
        var entity = new FormedAdditionalEquip
        {
            Purposes = additionalEquipPurposes
        };

        await _formedAdditionalEquipsRepository.AddAsync(entity);
        await _projectRepository.AddAdditionalEquipToStandAsync(standId, entity.Id);
    }

    public Task<ObvyazkaInStand> CreateObvyazkaAsync(StandModel standModel, Obvyazka selectedObvyazka)
    {
        if (standModel == null)
        {
            _notificationService.ShowError("Стенд не выбран!");
            return Task.FromResult<ObvyazkaInStand>(null);
        }

        if (selectedObvyazka == null || selectedObvyazka.Id <= 0)
        {
            return Task.FromResult<ObvyazkaInStand>(null);
        }

        var entity = new ObvyazkaInStand
        {
            LineLength = selectedObvyazka.LineLength,
            ZraCount = selectedObvyazka.ZraCount,
            Sensor = selectedObvyazka.Sensor,
            SensorType = selectedObvyazka.SensorType,
            Clamp = selectedObvyazka.Clamp,
            WidthOnFrame = selectedObvyazka.WidthOnFrame,
            OtherLineCount = selectedObvyazka.OtherLineCount,
            Weight = selectedObvyazka.Weight,
            TreeSocketCount = selectedObvyazka.TreeSocket,
            HumanCost = selectedObvyazka.HumanCost,
            ImageName = selectedObvyazka.ImageName,
            ObvyazkaId = selectedObvyazka.Id,

            ObvyazkaName = standModel.ObvyazkaName,
            StandId = standModel.Id,
            NN = standModel.NN,
            MaterialLine = standModel.MaterialLine,
            MaterialLineCount = standModel.MaterialLineCount,
            MaterialLineMeasure = standModel.MaterialLineMeasure,
            MaterialLineCostPerUnit = standModel.MaterialLineCostPerUnit,
            MaterialLineExportDays = standModel.MaterialLineExportDays,
            Armature = standModel.Armature,
            ArmatureCount = standModel.ArmatureCount,
            ArmatureMeasure = standModel.ArmatureMeasure,
            ArmatureCostPerUnit = standModel.ArmatureCostPerUnit,
            ArmatureExportDays = standModel.ArmatureExportDays,
            TreeSocket = standModel.TreeSocket,
            TreeSocketMaterialCount = standModel.TreeSocketMaterialCount,
            TreeSocketMaterialMeasure = standModel.TreeSocketMaterialMeasure,
            TreeSocketMaterialCostPerUnit = standModel.TreeSocketMaterialCostPerUnit,
            TreeSocketExportDays = standModel.TreeSocketExportDays,
            KMCH = standModel.KMCH,
            KMCHCount = standModel.KMCHCount,
            KMCHMeasure = standModel.KMCHMeasure,
            KMCHCostPerUnit = standModel.KMCHCostPerUnit,
            KMCHExportDays = standModel.KMCHExportDays,
            FirstSensorType = standModel.FirstSensorType,
            FirstSensorKKS = standModel.FirstSensorKKS,
            FirstSensorMarkPlus = standModel.FirstSensorMarkPlus,
            FirstSensorMarkMinus = standModel.FirstSensorMarkMinus,
            FirstSensorDescription = standModel.FirstSensorDescription,
            SecondSensorType = standModel.SecondSensorType,
            SecondSensorKKS = standModel.SecondSensorKKS,
            SecondSensorMarkPlus = standModel.SecondSensorMarkPlus,
            SecondSensorMarkMinus = standModel.SecondSensorMarkMinus,
            SecondSensorDescription = standModel.SecondSensorDescription,
            ThirdSensorType = standModel.ThirdSensorType,
            ThirdSensorKKS = standModel.ThirdSensorKKS,
            ThirdSensorMarkPlus = standModel.ThirdSensorMarkPlus,
            ThirdSensorMarkMinus = standModel.ThirdSensorMarkMinus,
            ThirdSensorDescription = standModel.ThirdSensorDescription,
            AdditionalComponents = standModel.ObvyazkaAdditionalComponents
                                    .Select(component => new ObvyazkaAdditionalEquipPurpose
                                    {
                                        Purpose = component.Purpose,
                                        Material = component.Material,
                                        Quantity = component.Quantity,
                                        CostPerUnit = component.CostPerUnit,
                                        Measure = component.Measure,
                                        ExportDays = component.ExportDays,
                                        Id = component.Id
                                    })
                                    .ToList()
        };

        return Task.FromResult(entity);
    }
    public async void FillStandFieldsFromObvyazka(StandModel stand, ObvyazkaInStand obv)
    {
        if (stand == null || obv == null)
            return;

        stand.NN = obv.NN ?? 0;
        stand.ObvyazkaName = obv.ObvyazkaName;
        stand.MaterialLine = obv.MaterialLine;
        stand.MaterialLineCount = obv.MaterialLineCount;
        stand.MaterialLineMeasure = obv.MaterialLineMeasure;
        stand.Armature = obv.Armature;
        stand.ArmatureCount = obv.ArmatureCount;
        stand.ArmatureMeasure = obv.ArmatureMeasure;
        stand.TreeSocket = obv.TreeSocket;
        stand.TreeSocketMaterialCount = obv.TreeSocketMaterialCount;
        stand.TreeSocketMaterialMeasure = obv.TreeSocketMaterialMeasure;
        stand.KMCH = obv.KMCH;
        stand.KMCHCount = obv.KMCHCount;
        stand.KMCHMeasure = obv.KMCHMeasure;

        stand.MaterialLineCostPerUnit = obv.MaterialLineCostPerUnit;
        stand.TreeSocketMaterialCostPerUnit = obv.TreeSocketMaterialCostPerUnit;
        stand.KMCHCostPerUnit = obv.KMCHCostPerUnit;
        stand.ArmatureCostPerUnit = obv.ArmatureCostPerUnit;

        stand.FirstSensorType = obv.FirstSensorType;
        stand.FirstSensorKKS = obv.FirstSensorKKS;
        stand.FirstSensorMarkPlus = obv.FirstSensorMarkPlus;
        stand.FirstSensorMarkMinus = obv.FirstSensorMarkMinus;
        stand.FirstSensorDescription = obv.FirstSensorDescription;
        stand.SecondSensorType = obv.SecondSensorType;
        stand.SecondSensorKKS = obv.SecondSensorKKS;
        stand.SecondSensorMarkPlus = obv.SecondSensorMarkPlus;
        stand.SecondSensorMarkMinus = obv.SecondSensorMarkMinus;
        stand.SecondSensorDescription = obv.SecondSensorDescription;
        stand.ThirdSensorType = obv.ThirdSensorType;
        stand.ThirdSensorKKS = obv.ThirdSensorKKS;
        stand.ThirdSensorMarkPlus = obv.ThirdSensorMarkPlus;
        stand.ThirdSensorMarkMinus = obv.ThirdSensorMarkMinus;
        stand.ThirdSensorDescription = obv.ThirdSensorDescription;

        var additionalComponents = await GetAdditionalComponentsAsync(obv);

        stand.ObvyazkaAdditionalComponents.Clear();
        foreach (var additionalEqip in additionalComponents)
        {
            stand.ObvyazkaAdditionalComponents.Add(additionalEqip);
        }
        
    }
    private async Task<List<ObvyazkaAdditionalEquipPurpose>> GetAdditionalComponentsAsync(ObvyazkaInStand obv)
    {
        if (obv.AdditionalComponents != null)
        {
            return obv.AdditionalComponents.ToList();
        }

        if (obv.Id != 0)
        {
            return await _context.ObvyazkaAdditionalEquipPurpose
                .Where(a => a.ObvyazkaInStandId == obv.Id)
                .AsNoTracking()
                .ToListAsync();
        }

        return new List<ObvyazkaAdditionalEquipPurpose>();
    }

    public async Task DeleteAdditionalPurposeFromObvAsync(ObvyazkaAdditionalEquipPurpose obv, StandModel standModel)
    {
        if(obv == null || standModel == null)
        {
            _notificationService.ShowError("Доп. комплектующее или стенд не выбраны!");
            return;
        }

        standModel.ObvyazkaAdditionalComponents.Remove(obv);
        await _obvyazkaInStandRepository.DeleteObvyazkaPurposesAsync(obv.Id);
    }

    public async Task UpdateAdditionalPurposeFromObvAsync(ObvyazkaAdditionalEquipPurpose obv, int obvyazkaInStand)
    {
        if (obv == null)
        {
            _notificationService.ShowError("Доп. комплектующее или стенд не выбраны!");
            return;
        }

        await _obvyazkaInStandRepository.UpdateObvyazkaPurposesAsync(obv, obvyazkaInStand);
        _notificationService.ShowInfo("Комплектующие обновлены");
    }

    public async Task UpdateElectricalPurposeAsync(ElectricalPurpose entity)
    {
        if (entity == null) return;
        await _formedElectricalRepository.UpdateAsync(entity);
    }

    public async Task DeleteElectricalPurposeAsync(int purposeId)
    {
        await _formedElectricalRepository.DeletePurposeAsync(purposeId);
    }

    public async Task UpdateAdditionalPurposeAsync(AdditionalEquipPurpose entity)
    {
        if (entity == null) return;
        await _formedAdditionalEquipsRepository.UpdateAsync(entity);
    }

    public async Task DeleteAdditionalPurposeAsync(int purposeId)
    {
        await _formedAdditionalEquipsRepository.DeletePurposeAsync(purposeId);
    }

    public async Task UpdateDrainagePurposeAsync(DrainagePurpose entity)
    {
        if (entity == null) return;
        await _formedDrainagesRepository.UpdateAsync(entity);
    }

    public async Task DeleteDrainagePurposeAsync(int purposeId)
    {
        await _formedDrainagesRepository.DeletePurposeAsync(purposeId);
    }

    public async Task UpdateStandWeight(StandModel stand)
    {
        await _projectRepository.UpdateStandAsync(StandDataConverter.ConvertToStandEntity(stand));
    }
}
