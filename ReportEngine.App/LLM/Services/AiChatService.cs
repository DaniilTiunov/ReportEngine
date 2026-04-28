using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using ReportEngine.App.LLM.Models;
using ReportEngine.App.Services.Interfaces;

namespace ReportEngine.App.LLM.Services;

public class AiChatService
{
    ///
    private readonly HttpClient _httpClient;
    private readonly INotificationService _notificationService;

    private const string API_URL = "https://api.groq.com/openai/v1/chat/completions";
    private const string MODEL = "llama-3.3-70b-versatile";
    private string _apiKey;

    public AiChatService(
        HttpClient httpClient,
        INotificationService notificationService)
    {
        _httpClient = httpClient;
        _notificationService = notificationService;

        SetApiKey();
    }

    public void SetApiKey()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appSecrets");

        if (!File.Exists(path))
            throw new Exception("API key file not found");

        _apiKey = File.ReadAllText(path).Trim();
    }

    public async Task<string> SendMessageAsync(string message)
    {
        try
        {
            var request = new HttpRequestModel
            {
                Model = MODEL,
                Message = message
            };

            var httpRequest = HandleHttpRequest(HttpMethod.Post, request);

            var response = await HandleHttpResponse(httpRequest);

            return response;
        }
        catch (Exception e)
        {
            _notificationService.ShowError(e.Message);
            throw;
        }
    }

    private HttpRequestMessage HandleHttpRequest(
        HttpMethod method,
        HttpRequestModel request)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(method, API_URL);
            httpRequest.Headers.Add("Authorization", $"Bearer {_apiKey}");
            httpRequest.Content = JsonContent.Create(request);

            return httpRequest;
        }
        catch (Exception e)
        {
            _notificationService.ShowError(e.Message);
            throw;
        }
    }

    private async Task<string> HandleHttpResponse(HttpRequestMessage httpRequest)
    {
        try
        {
            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AiResponse>();

            return result?.Choices?.FirstOrDefault()?.Message?.Content ?? "No response";
        }
        catch (Exception e)
        {
            _notificationService.ShowError(e.Message);
            throw;
        }
    }
}


