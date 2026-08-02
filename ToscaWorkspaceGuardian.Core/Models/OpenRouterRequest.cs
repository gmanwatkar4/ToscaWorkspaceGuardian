// <copyright file="OpenRouterRequest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.AI;

using System.Text.Json.Serialization;

/// <summary>

/// TODO: Describe OpenRouterRequest.

/// </summary>

public class OpenRouterRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("messages")]
    public List<OpenRouterMessage> Messages { get; set; } = new();

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 1000;

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 0.2;
}

/// <summary>

/// TODO: Describe OpenRouterMessage.

/// </summary>

public class OpenRouterMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

