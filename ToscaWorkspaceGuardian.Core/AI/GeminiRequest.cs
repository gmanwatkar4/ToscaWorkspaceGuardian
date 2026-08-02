// <copyright file="GeminiRequest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.AI;

using System.Text.Json.Serialization;

/// <summary>

/// TODO: Describe GeminiRequest.

/// </summary>

public class GeminiRequest
{
    [JsonPropertyName("contents")]
    public List<GeminiContent> Contents { get; set; } = new();
}

/// <summary>

/// TODO: Describe GeminiContent.

/// </summary>

public class GeminiContent
{
    [JsonPropertyName("parts")]
    public List<GeminiPart> Parts { get; set; } = new();
}

/// <summary>

/// TODO: Describe GeminiPart.

/// </summary>

public class GeminiPart
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}

