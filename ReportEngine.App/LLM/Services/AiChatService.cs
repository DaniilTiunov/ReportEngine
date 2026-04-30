using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ReportEngine.App.LLM.Interfaces;
using ReportEngine.App.LLM.Models;
using ReportEngine.App.Services.Interfaces;

namespace ReportEngine.App.LLM.Services;

public class AiChatService : IAiChatService
{
    private readonly HttpClient _httpClient;
    private readonly INotificationService _notificationService;

    private const string AUTH_URL = "https://ngw.devices.sberbank.ru:9443/api/v2/oauth";
    private const string API_URL = "https://gigachat.devices.sberbank.ru/api/v1/chat/completions";
    private const string MODEL = "GigaChat";

    private string _accessToken;
    private DateTime _tokenExpiry;
    private string _apiKey;

    public AiChatService(
        HttpClient httpClient,
        INotificationService notificationService)
    {
        _httpClient = httpClient;
        _notificationService = notificationService;

        SetApiKey();
    }

    private void SetApiKey()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appSecrets.json");

        if (!File.Exists(path))
        {
            _notificationService.ShowError($"Не удаётся найти файл с ключом");
            throw new Exception();
        }

        var json = File.ReadAllText(path);
        var config = JsonSerializer.Deserialize<ApiConfig>(json);

        _apiKey = config.ApiKey ??  throw new Exception("Не удаётся найти API ключ");
    }

     public async Task<string> SendMessageAsync(string message)
    {
        try
        {
            if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _tokenExpiry)
            {
                await GetAccessTokenAsync();
            }

            var request = new
            {
                model = MODEL,
                messages = new
                {
                    content = message
                }
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, API_URL);
            httpRequest.Headers.Add("Authorization", $"Bearer {_accessToken}");
            httpRequest.Headers.Add("Accept", "application/json");
            httpRequest.Content = JsonContent.Create(request);

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
            httpRequest.Headers.Add("Authorization", $"Bearer {_accessToken}");
            httpRequest.Content = JsonContent.Create(new
            {
                model = MODEL,
                messages = new
                {
                    content = request.Message
                }
            });

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

            // Ваша существующая структура AiResponse подходит для GigaChat
            var result = await response.Content.ReadFromJsonAsync<AiResponse>();
            return result?.Choices?.FirstOrDefault()?.Message?.Content ?? "No response";
        }
        catch (Exception e)
        {
            _notificationService.ShowError(e.Message);
            throw;
        }
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, AUTH_URL);

        request.Headers.Add("RqUID", Guid.NewGuid().ToString());
        request.Headers.Add("Authorization", $"Basic {_apiKey}");
        request.Headers.Add("Accept", "application/json");

        request.Content = new StringContent(
            "scope=GIGACHAT_API_PERS",
            Encoding.UTF8,
            "application/x-www-form-urlencoded"
        );

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        _accessToken = result.AccessToken;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(result.ExpiresIn - 60);

        return _accessToken;
    }
}


