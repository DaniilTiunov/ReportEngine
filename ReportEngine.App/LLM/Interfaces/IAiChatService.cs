namespace ReportEngine.App.LLM.Interfaces;

public interface IAiChatService
{
    Task<string> SendMessageAsync(string message);
}
