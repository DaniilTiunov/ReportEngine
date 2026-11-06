using System.ComponentModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReportEngine.App.Services.Automation.Pipe;
using ReportEngine.App.Services.Automation.Route;
using ReportEngine.Contracts.Messages;

namespace ReportEngine.App.Services.Automation.Worker;

public class KiparisWorker : BackgroundService
{
    private readonly PipeFactory _pipeFactory;
    private readonly KiparisCommandHandler _commandHandler;
    private readonly ILogger<KiparisWorker> _logger;

    public KiparisWorker(
        PipeFactory pipeFactory,
        KiparisCommandHandler commandHandler,
        ILogger<KiparisWorker> logger)
    {
        _pipeFactory = pipeFactory;
        _commandHandler = commandHandler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessConnectionAsync(stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "Истекло время обработки подключения.");
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Ошибка обработки pipe-подключения.");

                // Не повторяем попытки без паузы при постоянной ошибке.
                try
                {
                    await Task.Delay(500, stoppingToken);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
    }

    private async Task ProcessConnectionAsync(
        CancellationToken stoppingToken)
    {
        await using var pipe = _pipeFactory.Create();

        // Ожидание клиента прерывается при остановке приложения.
        await pipe.WaitForConnectionAsync(stoppingToken);

        // Ограничиваем время уже установленного подключения.
        using var timeout =
            CancellationTokenSource.CreateLinkedTokenSource(
                stoppingToken);

        timeout.CancelAfter(TimeSpan.FromSeconds(30));

        var token = timeout.Token;

        using var messages = new PipeMessageStream(pipe);

        var request = await messages.ReadAsync<KiparisRequest>(token);

        // Клиент закрыл соединение или отправил JSON null.
        if (request is null)
            return;

        var response = await ExecuteCommandAsync(request, token);

        await messages.WriteAsync(response, token);
    }

    private async Task<KiparisResponse> ExecuteCommandAsync(
        KiparisRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _commandHandler.HandleAsync(
                request,
                cancellationToken);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Ошибка команды {Command}, запрос {RequestId}",
                request.Command,
                request.Id);

            return new KiparisResponse
            {
                ProtocolVersion = 1,
                Id = request.Id,
                Success = false,
                Error = new KiparisError
                {
                    Code = "internal_error",
                    Message = "Не удалось выполнить команду."
                }
            };
        }
    }
}