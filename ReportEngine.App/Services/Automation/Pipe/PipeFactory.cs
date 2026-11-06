using System.IO.Pipes;

namespace ReportEngine.App.Services.Automation.Pipe;

public class PipeFactory
{
    public NamedPipeServerStream Create()
    {
        return new NamedPipeServerStream(
            "kiparis.automation.v1",
            PipeDirection.InOut,
            1,
            PipeTransmissionMode.Byte,
            PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
    }
}