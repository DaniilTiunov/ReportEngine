using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using ReportEngine.App.Commands;
using ReportEngine.App.Extensions;
using ReportEngine.App.Services.Notification;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.ViewModels;

public class AuditViewModel : BaseViewModel
{
    private readonly ReAppContext _context;
    private readonly ExceptionService _exceptionService;
    private ObservableCollection<AuditEvent> _allEvents = new();
    private DateTime _filterFrom = DateTime.UtcNow.AddDays(-7);
    private DateTime _filterTo = DateTime.UtcNow;

    public AuditViewModel(
        ReAppContext context,
        ExceptionService exceptionService)
    {
        _context = context;
        _exceptionService = exceptionService;

        LoadAllEventsCommand = new RelayCommand(OnLoadAllEventsAsyncExecuted, CanAlwaysExecute);
        LoadFilteredEventsCommand = new RelayCommand(OnLoadFilteredEventsAsyncExecuted, CanAlwaysExecute);
    }

    public ObservableCollection<AuditEvent> AllEvents
    {
        get => _allEvents;
        set => Set(ref _allEvents, value);
    }

    public DateTime FilterFrom
    {
        get => _filterFrom;
        set => Set(ref _filterFrom, value);
    }

    public DateTime FilterTo
    {
        get => _filterTo;
        set => Set(ref _filterTo, value);
    }

    public ICommand LoadAllEventsCommand { get; set; }
    public ICommand LoadFilteredEventsCommand { get; set; }

    public bool CanAlwaysExecute(object? p)
    {
        return true;
    }

    public async void OnLoadAllEventsAsyncExecuted(object? p)
    {
        await _exceptionService.SafeExecuteAsync(LoadAllEventsAsync);
    }

    public async void OnLoadFilteredEventsAsyncExecuted(object? p)
    {
        await _exceptionService.SafeExecuteAsync(LoadFilteredEventsAsync);
    }

    public async Task LoadAllEventsAsync()
    {
        AllEvents.Clear();
        var events = await _context.Set<AuditEvent>().ToListAsync();

        AllEvents = events.ToObservable();
    }

    public async Task LoadFilteredEventsAsync()
    {
        AllEvents.Clear();
        var events = await _context.Set<AuditEvent>()
            .Where(e => FilterFrom <= e.Timestamp && e.Timestamp <= FilterTo)
            .ToListAsync();

        AllEvents = events.ToObservable();
    }
}
