// <copyright file="GeminiResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.AI;

using System.Text.Json.Serialization;

/// <summary>

/// TODO: Describe GeminiResponse.

/// </summary>

public class GeminiResponse
{
    [JsonPropertyName("candidates")]
    public List<GeminiCandidate>? Candidates { get; set; }
}

/// <summary>

/// TODO: Describe GeminiCandidate.

/// </summary>

public class GeminiCandidate
{
    [JsonPropertyName("content")]
    public GeminiContentResponse? Content { get; set; }
}

/// <summary>

/// TODO: Describe GeminiContentResponse.

/// </summary>

public class GeminiContentResponse
{
    [JsonPropertyName("parts")]
    public List<GeminiPartResponse>? Parts { get; set; }
}

/// <summary>

/// TODO: Describe GeminiPartResponse.

/// </summary>

public class GeminiPartResponse
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}

