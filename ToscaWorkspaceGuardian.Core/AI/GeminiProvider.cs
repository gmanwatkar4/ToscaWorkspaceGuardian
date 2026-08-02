using System.Text;
using System.Text.Json;


namespace ToscaWorkspaceGuardian.Core.AI;

public class GeminiProvider : IAIProvider
{
    private readonly HttpClient _httpClient;
    private readonly GeminiSettings _settings;

    public GeminiProvider()
    {
        _httpClient = new HttpClient();

        _settings = new GeminiSettings
        {
            ApiKey = "...",
            Model = "gemini-2.5-flash"
        };
    }

    public async Task<AIResponse> GenerateAsync(
        AIRequest request,
        CancellationToken cancellationToken = default)
    {
        string url =
            $"https://generativelanguage.googleapis.com/v1beta/models/{_settings.Model}:generateContent?key={_settings.ApiKey}";

        var body = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = request.Prompt
                        }
                    }
                }
            }
        };

        string json = JsonSerializer.Serialize(body);

        using var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        using var response = await _httpClient.PostAsync(
            url,
            content,
            cancellationToken);

        string responseText =
            await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Gemini API Error ({(int)response.StatusCode}){Environment.NewLine}{responseText}");
        }

        //------------------------------------------
        // Parse JSON
        //------------------------------------------

        using JsonDocument document =
            JsonDocument.Parse(responseText);

        string aiText = "";

        if (document.RootElement.TryGetProperty(
            "candidates",
            out var candidates))
        {
            if (candidates.GetArrayLength() > 0)
            {
                var first = candidates[0];

                if (first.TryGetProperty(
                    "content",
                    out var contentNode))
                {
                    if (contentNode.TryGetProperty(
                        "parts",
                        out var parts))
                    {
                        if (parts.GetArrayLength() > 0)
                        {
                            aiText =
                                parts[0]
                                .GetProperty("text")
                                .GetString() ?? "";
                        }
                    }
                }
            }
        }

        return new AIResponse
        {
            Content = aiText
        };
    }
}