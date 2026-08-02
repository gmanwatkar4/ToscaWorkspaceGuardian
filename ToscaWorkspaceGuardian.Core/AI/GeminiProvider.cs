// <copyright file="GeminiProvider.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.AI;

using System.Text;
using System.Text.Json;

public class GeminiProvider : IAIProvider
{
    private readonly HttpClient httpClient;
    private readonly GeminiSettings settings;

    public GeminiProvider()
    {
        this.httpClient = new HttpClient();

        this.settings = new GeminiSettings
        {
            ApiKey = "...",
            Model = "gemini-2.5-flash",
        };
    }

    public async Task<AIResponse> GenerateAsync(
        AIRequest request,
        CancellationToken cancellationToken = default)
    {
        string url =
            $"https://generativelanguage.googleapis.com/v1beta/models/{this.settings.Model}:generateContent?key={this.settings.ApiKey}";

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
            },
        };

        string json = JsonSerializer.Serialize(body);

        using var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        using var response = await this.httpClient.PostAsync(
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

        string aiText = string.Empty;

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
                                .GetString() ?? string.Empty;
                        }
                    }
                }
            }
        }

        return new AIResponse
        {
            Content = aiText,
        };
    }
}
