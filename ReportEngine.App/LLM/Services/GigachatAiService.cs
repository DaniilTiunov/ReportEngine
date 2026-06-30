using System.IO;
using System.Net.Http;
using System.Text.Json;
using GigaChatAdapter;
using GigaChatAdapter.Auth;
using ReportEngine.App.LLM.Interfaces;
using ReportEngine.App.LLM.Models;

namespace ReportEngine.App.LLM.Services;

public class GigachatAiService : IAiChatService
{
    private string _apiKey;

    public GigachatAiService()
    {
        SetApiKey();
    }

    public async Task<string> SendMessageAsync(string message)
    {
        var authorization = new Authorization(_apiKey,
            RateScope.GIGACHAT_API_PERS);

        var authResult = await authorization.SendRequest();

        if (authResult.AuthorizationSuccess)
        {
            var completion = new Completion();

            var prompt = message;

            await authorization.UpdateToken();

            var settings = new CompletionSettings(
                "GigaChat:latest", 1, null, 1, 1500);

            var result = await completion.SendRequest(
                authorization.LastResponse.GigaChatAuthorizationResponse?.AccessToken,
                prompt,
                true,
                settings);

            if (result.RequestSuccessed)
            {
                var answer = "";

                foreach (var it in result.GigaChatCompletionResponse.Choices)
                    answer = it.Message.Content;

                return answer ?? "Запрос не успешен";
            }
        }

        return "Авторизация не удалась";
    }

    private void SetApiKey()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appSecrets.json");

        if (!File.Exists(path)) throw new Exception();

        var json = File.ReadAllText(path);
        var config = JsonSerializer.Deserialize<ApiConfig>(json);

        _apiKey = config.ApiKey ?? throw new Exception("Не удаётся найти API ключ");
    }
}
