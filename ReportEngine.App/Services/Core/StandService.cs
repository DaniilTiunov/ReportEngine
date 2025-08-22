using System.Collections.ObjectModel;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.Services.Core;

public class StandService : IStandService
{
    private readonly IFormedAdditionalEquipsRepository _formedAdditionalEquipsRepository;
    private readonly IFormedDrainagesRepository _formedDrainagesRepository;
    private readonly IFormedElectricalRepository _formedElectricalRepository;
    private readonly IFrameRepository _formedFrameRepository;
    private readonly INotificationService _notificationService;
    private readonly IProjectInfoRepository _projectRepository;

    public StandService(IProjectInfoRepository projectRepository,
        IFrameRepository frameRepository,
        IFormedDrainagesRepository drainagesRepository,
        INotificationService notificationService,
        IFormedAdditionalEquipsRepository formedAdditionalEquipsRepository,
        IFormedElectricalRepository formedElectricalRepository)
    {
        _projectRepository = projectRepository;
        _formedFrameRepository = frameRepository;
        _formedDrainagesRepository = drainagesRepository;
        _notificationService = notificationService;
        _formedAdditionalEquipsRepository = formedAdditionalEquipsRepository;
        _formedElectricalRepository = formedElectricalRepository;
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
        _notificationService.ShowInfo($"Рама успешно добавлена в стенд. {standId}");
    }

    public async Task AddDrainageToStandAsync(int standId, int drainageId)
    {
        await _projectRepository.AddDrainageToStandAsync(standId, drainageId);
        _notificationService.ShowInfo($"Дренаж успешно добавлен в стенд. {standId}");
    }

    public async Task AddCustomDrainageAsync(int standId, FormedDrainage customDrainage)
    {
        var entity = new FormedDrainage
        {
            Name = customDrainage.Name,
            Purposes = customDrainage.Purposes.Select(p => new DrainagePurpose
            {
                Purpose = p.Purpose,
                Material = p.Material,
                Quantity = p.Quantity
            }).ToList()
        };

        await _formedDrainagesRepository.AddAsync(entity);
        await _projectRepository.AddDrainageToStandAsync(standId, entity.Id);
        _notificationService.ShowInfo($"Собранный дренаж успешно добавлен! Id {standId}");
    }

    public async Task AddObvyazkaToStandAsync(int standId, ObvyazkaInStand obvyazka)
    {
        await _projectRepository.AddStandObvyazkaAsync(standId, obvyazka);
        _notificationService.ShowInfo($"Обвязка успешно добавлена в стенд. Id{standId} ");
    }

    public async Task AddCustomElectricalComponentAsync(int standId, FormedElectricalComponent customElectrical)
    {
        var entity = new FormedElectricalComponent
        {
            Name = customElectrical.Name,
            Purposes = customElectrical.Purposes.Select(p => new ElectricalPurpose
            {
                Purpose = p.Purpose,
                Material = p.Material,
                Quantity = p.Quantity
            }).ToList()
        };

        await _formedElectricalRepository.AddAsync(entity);
        await _projectRepository.AddElectricalComponentToStandAsync(standId, entity.Id);
        _notificationService.ShowInfo($"Собранный электрический компонент успешно добавлен! Id {standId}");
    }

    public async Task AddCustomAdditionalEquipAsync(int standId, FormedAdditionalEquip customEquip)
    {
        var entity = new FormedAdditionalEquip
        {
            Name = customEquip.Name,
            Purposes = customEquip.Purposes.Select(p => new AdditionalEquipPurpose
            {
                Purpose = p.Purpose,
                Material = p.Material,
                Quantity = p.Quantity
            }).ToList()
        };

        await _formedAdditionalEquipsRepository.AddAsync(entity);
        await _projectRepository.AddAdditionalEquipToStandAsync(standId, entity.Id);
        _notificationService.ShowInfo($"Собранное комплектующее успешно добавлено! Id {standId}");
    }
}