using System.Text.Json;

namespace ReportEngine.App.Services.Automation.Route;

public interface IKiparisCommand
{
    string Name { get; }

    Task<JsonElement> ExecuteAsync(
        JsonElement? arguments,
        CancellationToken cancellationToken);
}