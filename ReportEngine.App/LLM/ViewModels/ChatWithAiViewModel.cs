using System.Collections.ObjectModel;
using System.Windows.Input;
using ReportEngine.App.AsyncCommands;
using ReportEngine.App.LLM.Interfaces;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.LLM.ViewModels;

public class ChatWithAiViewModel : BaseViewModel
{
    private readonly IAiChatService _chatService;
    private string _input;

    public ICommand SendCommand { get; }

    public ChatWithAiViewModel(IAiChatService chatService)
    {
        _chatService = chatService;
        SendCommand = new AsyncRelayCommand(SendCommandExecuted, CanAllCommandsExecute);
    }

    public ObservableCollection<string> Messages { get; } = new();

    public string Input
    {
        get => _input;
        set => Set(ref _input, value);
    }

    private async Task SendCommandExecuted(object obj)
    {
        if (string.IsNullOrWhiteSpace(Input))
            return;

        Messages.Add($"""
                       Вы:
                      {Input}
                      """);

        var response = await _chatService.SendMessageAsync(Input);

        Input = string.Empty;

        Messages.Add($"""
                       ГигаЧат:
                      {response}
                      """);

    }

    public bool CanAllCommandsExecute(object obj)
    {
        return true;
    }
}
