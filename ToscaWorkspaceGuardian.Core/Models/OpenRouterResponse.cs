// <copyright file="OpenRouterResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.AI;

using System.Text.Json.Serialization;

public class OpenRouterResponse
{
    [JsonPropertyName("choices")]
    public List<Choice>? Choices { get; set; }
}

public class Choice
{
    [JsonPropertyName("message")]
    public ChoiceMessage? Message { get; set; }
}

public class ChoiceMessage
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
