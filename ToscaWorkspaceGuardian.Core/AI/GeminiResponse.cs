// <copyright file="GeminiResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.AI;

using System.Text.Json.Serialization;

public class GeminiResponse
{
    [JsonPropertyName("candidates")]
    public List<GeminiCandidate>? Candidates { get; set; }
}

public class GeminiCandidate
{
    [JsonPropertyName("content")]
    public GeminiContentResponse? Content { get; set; }
}

public class GeminiContentResponse
{
    [JsonPropertyName("parts")]
    public List<GeminiPartResponse>? Parts { get; set; }
}

public class GeminiPartResponse
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}
