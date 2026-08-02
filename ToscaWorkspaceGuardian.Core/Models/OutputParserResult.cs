// <copyright file="OutputParserResult.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Models;

/// <summary>

/// TODO: Describe OutputParserResult.

/// </summary>

public class OutputParserResult
{
    public bool Success { get; set; }

    public string RawOutput { get; set; } = string.Empty;

    public WorkspaceSummary WorkspaceSummary { get; set; } = new();
}

