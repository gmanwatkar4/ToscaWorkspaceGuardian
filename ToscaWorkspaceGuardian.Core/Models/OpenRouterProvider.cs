// <copyright file="OpenRouterProvider.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.AI;

using System.Text;
using System.Text.Json;

/// <summary>

/// TODO: Describe OpenRouterProvider.

/// </summary>

public class OpenRouterProvider : IAIProvider
{
    private readonly HttpClient httpClient;
    private readonly OpenRouterSettings settings;

    public OpenRouterProvider(OpenRouterSettings settings)
    {
        this.httpClient = new HttpClient();
        this.settings = settings;
    }

    public async Task<AIResponse> GenerateAsync(
        AIRequest request,
        CancellationToken cancellationToken = default)
    {
        var body = new OpenRouterRequest
        {
            Model = this.settings.Model,
            MaxTokens = 1000,
            Temperature = 0.2,
            Messages =
    {
        new OpenRouterMessage
        {
            Role = "user",
            Content = request.Prompt
        },
    },
        };

        string json = JsonSerializer.Serialize(body);

        using var message = new HttpRequestMessage(
            HttpMethod.Post,
            "https://openrouter.ai/api/v1/chat/completions");

        message.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                this.settings.ApiKey);

        message.Headers.Add("HTTP-Referer", "https://workspaceguardian.local");
        message.Headers.Add("X-Title", "Workspace Guardian");

        message.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        var response = await this.httpClient.SendAsync(
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
                    .Content ?? string.Empty,
        };
    }
}

