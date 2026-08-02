// <copyright file="AnalysisResult.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Models;

public class AnalysisResult
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public WorkspaceSummary Summary { get; set; } = new();
}
