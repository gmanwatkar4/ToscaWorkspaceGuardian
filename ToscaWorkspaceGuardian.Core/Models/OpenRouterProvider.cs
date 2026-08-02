using System.Text;
using System.Text.Json;


namespace ToscaWorkspaceGuardian.Core.AI;

public class OpenRouterProvider : IAIProvider
{
    private readonly HttpClient _httpClient;
    private readonly OpenRouterSettings _settings;

    public OpenRouterProvider(OpenRouterSettings settings)
    {
        _httpClient = new HttpClient();
        _settings = settings;
    }

    public async Task<AIResponse> GenerateAsync(
        AIRequest request,
        CancellationToken cancellationToken = default)
    {
        var body = new OpenRouterRequest
        {
            Model = _settings.Model,
            MaxTokens = 1000,
            Temperature = 0.2,
            Messages =
    {
        new OpenRouterMessage
        {
            Role = "user",
            Content = request.Prompt
        }
    }
        };

        string json = JsonSerializer.Serialize(body);

        using var message = new HttpRequestMessage(
            HttpMethod.Post,
            "https://openrouter.ai/api/v1/chat/completions");

        message.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                _settings.ApiKey);

        message.Headers.Add("HTTP-Referer", "https://workspaceguardian.local");
        message.Headers.Add("X-Title", "Workspace Guardian");

        message.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.SendAsync(
            message,
            cancellationToken);

        string responseText =
            await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(responseText);
        }

        var result =
            JsonSerializer.Deserialize<OpenRouterResponse>(responseText);

        return new AIResponse
        {
            Content =
                result?.Choices?
                    .FirstOrDefault()?
                    .Message?
                    .Content ?? ""
        };
    }
}