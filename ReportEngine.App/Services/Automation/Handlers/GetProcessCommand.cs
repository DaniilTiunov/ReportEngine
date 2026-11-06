using System.Diagnostics;
using System.Text.Json;
using System.Windows.Threading;
using ReportEngine.App.Services.Automation.Route;
using ReportEngine.Contracts.Commands;
using ReportEngine.Contracts.Models.Core;
using static System.Windows.Application;

namespace ReportEngine.App.Services.Automation.Handlers;

public class GetProcessCommand : IKiparisCommand
{
    public string Name => new KiparisCommands(commandName: "get_process").CommandName;

    public async Task<JsonElement> ExecuteAsync(
        JsonElement? arguments,
        CancellationToken cancellationToken)
    {
        var process = await Current.Dispatcher.InvokeAsync(() =>
            {
                var currentProcess = Process.GetCurrentProcess();

                return new ProcessInfo
                {
                    ProcessName = currentProcess.ProcessName,
                    CurrentProcessId = currentProcess.Id
                };
            },
            DispatcherPriority.Normal,
            cancellationToken);

        return JsonSerializer.SerializeToElement(process);
    }
}