using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Services.Automation.Route;
using ReportEngine.App.ViewModels;
using ReportEngine.Contracts.Commands;
using ReportEngine.Contracts.Project;

namespace ReportEngine.App.Services.Automation.Handlers;

public class GetAppStateCommand : IKiparisCommand
{
    private readonly IServiceProvider _serviceProvider;

    public GetAppStateCommand(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public string Name => KiparisCommands.GetAppState;
    public async Task<JsonElement> ExecuteAsync(
        JsonElement? arguments, 
        CancellationToken cancellationToken)
    {
        var state = await Application.Current.Dispatcher.InvokeAsync(
            () =>
            {
                var viewModel = _serviceProvider
                    .GetRequiredService<ProjectViewModel>();

                var project = viewModel.CurrentProjectModel;

                var hasProject =
                    project is not null && project.CurrentProjectId != 0;

                return new AppStateResult
                {
                    IsReady = true,
                    CurrentProjectId =
                        hasProject ? project!.CurrentProjectId : null,
                    StandsCount =
                        hasProject ? project!.Stands.Count : null
                };
            },
            DispatcherPriority.Normal,
            cancellationToken);

        return JsonSerializer.SerializeToElement(state);
    }
}