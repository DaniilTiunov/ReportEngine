using ReportEngine.Contracts.Messages;

namespace ReportEngine.App.Services.Automation.Route;

public class KiparisCommandHandler
{
    private readonly Dictionary<string, IKiparisCommand> _commands;
    private readonly IServiceProvider _serviceProvider;

    public KiparisCommandHandler(IEnumerable<IKiparisCommand> commands)
    {
        _commands = commands.ToDictionary(
            command => command.Name,
            StringComparer.Ordinal);
    }

    public async Task<KiparisResponse> HandleAsync(
        KiparisRequest request,
        CancellationToken cancellationToken)
    {
        if (request.ProtocolVersion != 1)
        {
            return Error(request, "unsupported_protocol_version",
                "Поддерживается версия протокола 1.");
        }

        if (request.Id == Guid.Empty ||
            string.IsNullOrWhiteSpace(request.Command))
        {
            return Error(request, "invalid_request",
                "Необходимо передать id и command.");
        }

        if (!_commands.TryGetValue(request.Command, out var command))
        {
            return Error(request, "unknown_command",
                "Неизвестная команда.");
        }

        var data = await command.ExecuteAsync(
            request.Arguments,
            cancellationToken);

        return new KiparisResponse
        {
            ProtocolVersion = 1,
            Id = request.Id,
            Success = true,
            Data = data
        };
    }
    
    private static KiparisResponse Error(
        KiparisRequest request,
        string code,
        string message)
    {
        return new KiparisResponse
        {
            ProtocolVersion = 1,
            Id = request.Id,
            Success = false,
            Error = new KiparisError
            {
                Code = code,
                Message = message
            }
        };
    }
}