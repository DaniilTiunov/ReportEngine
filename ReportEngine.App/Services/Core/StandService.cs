using System.Collections.ObjectModel;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.Services.Core;

public class StandService : IStandService
{
    private readonly IProjectInfoRepository _projectRepository;
    private readonly IFrameRepository _formedFrameRepository;
    private readonly IFormedDrainagesRepository _formedDrainagesRepository;
    private readonly INotificationService _notificationService;

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

    public async Task LoadStandDataAsync(StandModel standModel)
    {
        var frames = await _formedFrameRepository.GetAllAsync();
        var drainages = await _formedDrainagesRepository.GetAllWithPurposesAsync();
        var standFrames = await _projectRepository.GetAllFramesInStandAsync(standModel.Id);
        var drainagesInStand = await _projectRepository.GetAllDrainagesInStandAsync(standModel.Id);

        // Получаем рамы из StandFrame
        standModel.FramesInStand = new ObservableCollection<FormedFrame>(
            standFrames.Select(sf => sf.Frame)
        );
        standModel.DrainagesInStand = new ObservableCollection<FormedDrainage>(drainagesInStand);
        standModel.AllAvailableFrames = new ObservableCollection<FormedFrame>(frames);
        standModel.AllAvailableDrainages = new ObservableCollection<FormedDrainage>(drainages);
    }

    public async Task LoadObvyazkiInStandAsync(StandModel standModel)
    {
        var obvyazkiInStand = await _projectRepository.GetAllObvyazkiInStandAsync(standModel.Id);
        standModel.ObvyazkiInStand = new ObservableCollection<ObvyazkaInStand>(obvyazkiInStand);
    }

    public async Task AddFrameToStandAsync(int standId, int frameId)
    {
        await _projectRepository.AddFrameToStandAsync(standId, frameId);
        _notificationService.ShowInfo("Рама успешно добавлена в стенд.");
    }

    public async Task AddDrainageToStandAsync(int standId, FormedDrainage drainage)
    {
        await _projectRepository.AddDrainageToStandAsync(standId, drainage);
        _notificationService.ShowInfo("Выбранный дренаж успешно добавлен в стенд.");
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

        await _projectRepository.AddDrainageToStandAsync(standId, entity);
        await _formedDrainagesRepository.AddAsync(entity);
        _notificationService.ShowInfo("Собранный дренаж успешно добавлен!");
    }

    public async Task AddObvyazkaToStandAsync(int standId, ObvyazkaInStand obvyazka)
    {
        await _projectRepository.AddStandObvyazkaAsync(standId, obvyazka);
        _notificationService.ShowInfo("Обвязка успешно добавлена в стенд.");
    }
}