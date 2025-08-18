using System.Collections.ObjectModel;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.Services.Core;

public class StandService : IStandService
{
    private readonly IFormedDrainagesRepository _formedDrainagesRepository;
    private readonly IFrameRepository _formedFrameRepository;
    private readonly INotificationService _notificationService;
    private readonly IProjectInfoRepository _projectRepository;

    public StandService(IProjectInfoRepository projectRepository,
        IFrameRepository frameRepository,
        IFormedDrainagesRepository drainagesRepository,
        INotificationService notificationService)
    {
        _projectRepository = projectRepository;
        _formedFrameRepository = frameRepository;
        _formedDrainagesRepository = drainagesRepository;
        _notificationService = notificationService;
    }

    public async Task LoadStandsDataAsync(IEnumerable<StandModel> standModels)
    {
        var frames = await _formedFrameRepository.GetAllAsync();
        var drainages = await _formedDrainagesRepository.GetAllWithPurposesAsync();

        foreach (var standModel in standModels)
        {
            var standFrames = await _projectRepository.GetAllFramesInStandAsync(standModel.Id);
            var standDrainages = await _projectRepository.GetAllDrainagesInStandAsync(standModel.Id);

            standModel.FramesInStand = new ObservableCollection<FormedFrame>(
                standFrames.Select(sf => sf.Frame)
            );
            standModel.DrainagesInStand = new ObservableCollection<FormedDrainage>(
                standDrainages.Select(sd => sd.Drainage)
            );
            standModel.AllAvailableFrames = new ObservableCollection<FormedFrame>(frames);
            standModel.AllAvailableDrainages = new ObservableCollection<FormedDrainage>(drainages);
        }
    }

    public async Task LoadObvyazkiInStandsAsync(IEnumerable<StandModel> standModels)
    {
        foreach (var standModel in standModels)
        {
            var obvyazkiInStand = await _projectRepository.GetAllObvyazkiInStandAsync(standModel.Id);
            standModel.ObvyazkiInStand = new ObservableCollection<ObvyazkaInStand>(obvyazkiInStand);
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
}