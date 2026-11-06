using System.IO;
using System.Text;
using System.Text.Json;

namespace ReportEngine.App.Services.Automation.Pipe;

public class PipeMessageStream : IDisposable
{
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;

    public PipeMessageStream(Stream stream)
    {
        _reader = new StreamReader(
            stream,
            new UTF8Encoding(false),
            detectEncodingFromByteOrderMarks: false,
            bufferSize: 4096,
            leaveOpen: true);

        _writer = new StreamWriter(
            stream,
            new UTF8Encoding(false),
            bufferSize: 4096,
            leaveOpen: true);
    }

    public async Task<T?> ReadAsync<T>(
        CancellationToken cancellationToken)
    {
        var json = await _reader.ReadLineAsync(cancellationToken);

        if (json is null)
            return default;

        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task WriteAsync<T>(
        T message,
        CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(message);

        await _writer.WriteLineAsync(
            json.AsMemory(),
            cancellationToken);

        await _writer.FlushAsync(cancellationToken);
    }

    public void Dispose()
    {
        _reader.Dispose();
        _writer.Dispose();
    }
}