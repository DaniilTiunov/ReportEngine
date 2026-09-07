using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.Dds.Enums;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.Dds;

public class DdsService : BackgroundService
{
    private readonly INotificationService _notificationService;
    private readonly IServiceProvider _serviceProvider;

    private volatile bool _isActive;

    public DdsService(
        INotificationService notificationService,
        IServiceProvider serviceProvider)
    {
        _notificationService = notificationService;
        _serviceProvider = serviceProvider;
    }

    public bool IsActive => _isActive;

    public Level CurrentLevel { get; private set; }

    public ProjectAction CurrentProjectAction { get; private set; }

    public void Enable(Level level)
    {
        CurrentLevel = level;
        _isActive = true;
    }

    public void Disable()
    {
        _isActive = false;
        CurrentProjectAction = ProjectAction.None;
        CurrentLevel = Level.None;
    }

    public void SetAction(ProjectAction projectAction)
    {
        CurrentProjectAction = projectAction;
    }

    public async Task Execute()
    {
        await ControlDdsActions(CurrentLevel, _isActive);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ControlDdsActions(CurrentLevel, IsActive);

            await Task.Delay(7000, stoppingToken);
        }
    }

    private async Task ControlDdsActions(Level level, bool isEnabled)
    {
        if (!isEnabled)
            return;

        switch (level)
        {
            case Level.First:
                var ex = GetRandomException();
                ShowErrorInUi(ex.Message);
                break;

            case Level.Second:
                await ControlProject();
                break;
        }
    }

    private async Task ControlProject()
    {
        var action = CurrentProjectAction;

        if (action == ProjectAction.None)
            return;

        var projectViewModel = _serviceProvider.GetRequiredService<ProjectViewModel>();

        if (projectViewModel is null)
            return;

        switch (action)
        {
            case ProjectAction.ClearCollection:
                await ClearCollection(projectViewModel);
                break;
            case ProjectAction.DeleteFromDb:
                await DeleteProjectAsync(projectViewModel);
                break;
        }
    }

    private async Task ClearCollection(ProjectViewModel projectViewModel)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var stands = projectViewModel.CurrentProjectModel.Stands;

            stands.Clear();

            CollectionRefreshHelper.SafeRefreshCollection(stands);
        });
    }

    private async Task DeleteProjectAsync(ProjectViewModel projectViewModel)
    {
        var projectRepository = _serviceProvider.GetRequiredService<IProjectInfoRepository>();

        await projectRepository.DeleteByIdAsync(projectViewModel.CurrentProjectModel.CurrentProjectId);
    }

    private void ShowErrorInUi(string message)
    {
        Application.Current.Dispatcher.BeginInvoke((Action)(() =>
        {
            try
            {
                _notificationService.ShowError(message);
            }
            catch (Exception ex)
            {
            }
        }));
    }

    private Exception GetRandomException()
    {
        return Random.Shared.Next(6) switch
        {
            0 => new InvalidOperationException("Calculation subsystem is in an invalid state."),
            1 => new TimeoutException("The operation timed out while waiting for subsystem response."),
            2 => new ObjectDisposedException("CalculationContext"),
            3 => new KeyNotFoundException("Calculation parameter was not found."),
            4 => new IOException("Unable to read data from the underlying resource."),
            _ => new InvalidCastException("Unable to cast object to required type.")
        };
    }
}