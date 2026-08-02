// <copyright file="OpenRouterResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.AI;

using System.Text.Json.Serialization;

/// <summary>

/// TODO: Describe OpenRouterResponse.

/// </summary>

public class OpenRouterResponse
{
    [JsonPropertyName("choices")]
    public List<Choice>? Choices { get; set; }
}

/// <summary>

/// TODO: Describe Choice.

/// </summary>

public class Choice
{
    [JsonPropertyName("message")]
    public ChoiceMessage? Message { get; set; }
}

/// <summary>

/// TODO: Describe ChoiceMessage.

/// </summary>

public class ChoiceMessage
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

